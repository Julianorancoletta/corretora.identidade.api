using Corretora.Identidade.Infra.Interfaces;
using Corretora.Identidade.Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Corretora.Identidade.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
    }
}
