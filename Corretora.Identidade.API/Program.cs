using Corretora.Identidade.API.Configuration;
using Delivery.WebAPI.Configuration;
using Delivery.WebAPI.Core.Identity;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Corretora.Identidade.API
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
            builder.Services.AddIdentityConfiguration(builder.Configuration);
            builder.Services.AddJwtAsyncKeyConfiguration(builder.Configuration);
            builder.Services.AddApiConfiguration();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerConfiguration(new(
                "Corretora Identidade API",
                "Esta API faz parte do projeto corretora, projeto em grupo de alunos da FIAP",
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            builder.Services.RegisterServices();

            var app = builder.Build();

            app.UseSwaggerConfiguration();

            app.UseApiConfiguration(app.Environment);

            app.Run();
        }
    }
}
