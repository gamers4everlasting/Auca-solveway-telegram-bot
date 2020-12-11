using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.BLL.Models;

namespace TelegramBot.BLL.Services
{
    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            Client = 
                new TelegramBotClient(_config.Key);
        }

        public TelegramBotClient Client { get; }
    }
}