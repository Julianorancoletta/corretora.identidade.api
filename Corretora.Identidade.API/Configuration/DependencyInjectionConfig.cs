using Corretora.Identidade.API.Services;
using Corretora.Identidade.CrossCutting.IoC;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IIdentidadeService, IdentidadeService>();

            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}
