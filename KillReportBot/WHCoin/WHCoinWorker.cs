using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace KillReportBot.WHCoin
{
    internal class WHCoinWorker : BackgroundService
    {
        private readonly ILogger<WHCoinWorker> _logger;
        private readonly IServiceProvider _provider;
        private DiscordSocketClient _client;
        private CommandService _commandService;

        private ServiceConfiguration _configuration;
        private string _authToken;
        public WHCoinWorker(ILogger<WHCoinWorker> logger, IOptions<ServiceConfiguration> config, IServiceProvider provider)
        {
            _logger = logger;
            _configuration = config.Value;
            _provider = provider;
            _authToken = System.IO.File.ReadAllText(Environment.ExpandEnvironmentVariables(_configuration.AuthTokenPath));
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            _client.Log += Log;
            _commandService.Log += Log;
        }

        private Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(message.ToString());
                    break;
                case LogSeverity.Error:
                    _logger.LogError(message.ToString());
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(message.ToString());
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(message.ToString());
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(message.ToString());
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(message.ToString());
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Config Discord client
            await _client.LoginAsync(TokenType.Bot, _authToken);
            await _client.StartAsync();
            _client.MessageReceived += HandleCommandAsync;

            // Configure command service
            await _commandService.AddModuleAsync(typeof(WHCoinManagerModule), _provider);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if(message == null) 
            {
                // Dont process system messages.
                return; 
            }

            int arg = 0;

            if(!(message.HasCharPrefix('!', ref arg) ||
                message.HasMentionPrefix(_client.CurrentUser, ref arg)) ||
                message.Author.IsBot)
            {
                // Do not process if the command from a bot, or doesnt ahve the appropriate prefix.
                return;
            }

            // Create command context with the client.
            SocketCommandContext context = new SocketCommandContext(_client, message);

            await _commandService.ExecuteAsync(context, arg, _provider);
        }
    }
}
