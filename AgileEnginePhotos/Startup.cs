using System;
using API.Middleware;
using API.Services;
using API.Services.Interfaces;
using BackgroundTasksSample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddMemoryCache();
            services.AddControllers();

            services.AddSingleton<AuthenticationHandler>();
            services.AddSingleton<IAgileEnginePhotosService, AgileEnginePhotosService>();
            
            services.AddHttpClient<IAgileEnginePhotosService, AgileEnginePhotosService>("agileEngineClient", c =>
            {
                c.BaseAddress = new Uri("http://interview.agileengine.com/");
            })
            .AddHttpMessageHandler<AuthenticationHandler>();

            services.AddHostedService<RefreshCacheHostedService>(); // This will refresh the cache every certain period of time

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgileEnginePhotos API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgileEnginePhotos API v1"));
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
