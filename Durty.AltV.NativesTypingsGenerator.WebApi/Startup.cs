using System;
using Durty.AltV.NativesTypingsGenerator.NativeDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Durty.AltV.NativesTypingsGenerator.WebApi
{
    public class Startup
    {
        private const string AltVNativeDbJsonSourceUrl = "https://natives.altv.mp/natives";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton(provider => new NativeDbDownloader(AltVNativeDbJsonSourceUrl));
            services.AddSingleton(provider => new NativeDbCacheService(provider.GetService<NativeDbDownloader>(), TimeSpan.FromHours(1)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
