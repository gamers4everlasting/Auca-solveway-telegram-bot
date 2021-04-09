using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using TelegramBot.BLL.IocConfiguration;
using TelegramBot.DAL.EF;
using TelegramBot.Dto.Helper;

namespace TelegramBot
{
    public class Startup
    {
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddAppSettingsValues(Configuration);
            services.AddServiceDependencies(Configuration);
            services.AddControllers()
                .AddNewtonsoftJson(options =>
            {
            });
            services.AddHttpClient(ApiConstants.ClientName, client =>
            {
                client.BaseAddress = new Uri("https://api.solveway.club/");
            }).AddTransientHttpErrorPolicy(x => 
                x.WaitAndRetryAsync(3, _=> TimeSpan.FromMilliseconds(300)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}