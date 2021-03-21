using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KillReportBot.KillReports
{
    internal class KillReportWorker : BackgroundService
    {
        private readonly ILogger<KillReportWorker> _logger;
        private DiscordSocketClient _client;
        private CommandService _commandService;

        private ServiceConfiguration _configuration;
        private string _authToken;
        public KillReportWorker(ILogger<KillReportWorker> logger, IOptions<ServiceConfiguration> config)
        {
            _logger = logger;
            _configuration = config.Value;
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
            await _client.LoginAsync(TokenType.Bot, _authToken);
            await _client.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
