using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.BLL.Extensions;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models;
using TelegramBot.BLL.Services;

namespace TelegramBot.BLL.IocConfiguration
{
    public static class InfrastructureIocConfig
    {
        public static IServiceCollection AddAppSettingsValues(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<BotConfiguration>(Configuration.GetSection(nameof(BotConfiguration)));
            return services;
        }

        public static IServiceCollection AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICommandService, CommandService>();
            services.AddTransient<IBotService, BotService>();
            services.AddTransient<ITelegramBotService, TelegramBotService>();
            services.AddTransient<IDbProblemsService, DbProblemsService>();
            services.AddTransient<IDbSubmissionsService, DbSubmissionsService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IMessageHandleService, MessageHandleService>();
            services.AddTransient<ISolvewayAuthorizationService, SolvewayAuthorizationService>();
            services.AddTelegramBotClient(configuration);
            return services;
        }
    }
}