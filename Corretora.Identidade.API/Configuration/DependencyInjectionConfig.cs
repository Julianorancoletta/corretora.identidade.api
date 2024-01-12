using Corretora.Identidade.API.Application.Services;
using Corretora.Identidade.CrossCutting.IoC;

namespace Corretora.Identidade.API.Configuration
{
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
