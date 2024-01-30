using Delivery.WebAPI.Core.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            services.AddHealthChecks();

        }

        public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthConfiguration();

            app.UseCors("Total");

            app.MapControllers();

            app.UseJwksDiscovery();

            app.MapHealthChecks("/healthz");
        }
    }
}
