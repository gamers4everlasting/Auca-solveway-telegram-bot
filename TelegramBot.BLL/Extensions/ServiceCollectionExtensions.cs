using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBot.BLL.Models;

namespace TelegramBot.BLL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegramBotClient(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var option = new BotConfiguration();
            configuration.Bind(nameof(BotConfiguration), option);
            var client = new TelegramBotClient(option.Key);
            var webHook = $"{option.Url}/api/update";
            client.SetWebhookAsync(webHook).Wait();

            return serviceCollection
                .AddTransient<ITelegramBotClient>(x => client);
        }
    }
}