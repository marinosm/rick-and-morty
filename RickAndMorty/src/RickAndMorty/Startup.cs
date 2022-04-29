using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RickAndMortyApiClient;
using RickAndMortyApiClientDefault;
using RickAndMortyEngine;
using RickAndMortyEngineDefault;
using System;

namespace RickAndMorty
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
            services.AddHttpClient("default", config =>
            {
                config.BaseAddress = new Uri("https://rickandmortyapi.com/api/");
                config.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddTransient<IClient, Client>();
            services.AddTransient<ICharacterAndCoresidentsEngine, CharacterAndCoresidentsEngine>();
            services.AddTransient<ICharacterAndFirstEpisodeInfoEngine, CharacterAndFirstEpisodeInfoEngine>();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "RickAndMorty demo",
                    Description = "This API extends the functionality of https://rickandmortyapi.com/api/",
                    Contact = new OpenApiContact
                    {
                        Name = "Marinos Marommatis",
                        Url = new Uri("https://github.com/marinosm"),
                    }
                });
            });
        }

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
                endpoints.MapHealthChecks("/health");
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
