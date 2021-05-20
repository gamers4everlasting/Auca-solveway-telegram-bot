using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TelegramBot.Configurations
{
    /// <summary>
    /// Localization ioc configs
    /// </summary>
    public static class LocalizationSettings
    {
        /// <summary>
        ///     Add localization service
        ///     Usage in query: ?culture=ru-RU
        ///     Usage in header: Accept-Language : ru-RU
        /// </summary>
        /// <param name="services"></param>
        /// <param name="defaultCulture"></param>
        /// <param name="supportedCultures"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomLocalization(this IServiceCollection services, string defaultCulture,
            params string[] supportedCultures)
        {
            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultureInfos = supportedCultures.Select(x => new CultureInfo(x)).ToList();

                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultureInfos;
                options.SupportedUICultures = supportedCultureInfos;
            });

            return services;
        }

        /// <summary>
        /// Use localization method
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomLocalization(this IApplicationBuilder app)
        {
            var requestLocalizationOptions =
                app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()?.Value;

            if (requestLocalizationOptions != null)
            {
                app.UseRequestLocalization(requestLocalizationOptions);
            }

            return app;
        }
    }
}