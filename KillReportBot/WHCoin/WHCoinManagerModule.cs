using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KillReportBot.WHCoin
{
    public class WHCoinManagerModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<WHCoinManagerModule> _logger;
        private static Dictionary<string, int> _wallets;
        public WHCoinManagerModule(ILogger<WHCoinManagerModule> logger)
        {
            _logger = logger;
            if (_wallets == null)
            {
                _wallets = new Dictionary<string, int>();
            }
        }

        [Command("test")]
        [Summary("This is a test command")]
        public async Task Test()
        {
            _logger.LogInformation("This is a test.");
            await ReplyAsync("This is a test.");
        }

        [Command("init")]
        public async Task InitializeWallet()
        {
            if(_wallets.ContainsKey(Context.User.Username))
            {
                await ReplyAsync($"You already have a wallet with {_wallets[Context.User.Username]} coins.");
            }
            _wallets.Add(Context.User.Username, 1);
            await ReplyAsync($"Your whcoin wallet has ben initalized with {_wallets[Context.User.Username]} coin!");
        }

        [Command("Add")]
        public async Task AddCoins(IUser user, int coinsToAdd)
        {
            if (!_wallets.ContainsKey(Context.User.Username))
            {
                await ReplyAsync($"{user.Username} doesnt have a wallet.");
            }
            _wallets[user.Username] += coinsToAdd;
            await ReplyAsync($"{user.Mention} you have been given {coinsToAdd} whcoin. Your total is {_wallets[user.Username]} whcoin.");
        }

        [Command("Remove")]
        public async Task RemoveCoins(IUser user, int coinsToRemove)
        {
            if (!_wallets.ContainsKey(Context.User.Username))
            {
                await ReplyAsync($"{user.Username} doesnt have a wallet.");
            }
            _wallets[user.Username] -= coinsToRemove;
            await ReplyAsync($"{user.Mention} you have had {coinsToRemove} whcoin taken. Your total is {_wallets[user.Username]} whcoin.");
        }
    }
}
