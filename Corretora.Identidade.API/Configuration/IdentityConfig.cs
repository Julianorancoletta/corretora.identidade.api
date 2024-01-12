using Corretora.Identidade.API.Extensions;
using Corretora.Identidade.Infra.Context;
using Delivery.Core.DatabaseFlavor;
using Microsoft.AspNetCore.Identity;

namespace Corretora.Identidade.API.Configuration
{
    public static class IdentityConfig
    {
        public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppTokenSettings");
            services.Configure<AppTokenSettings>(appSettingsSection);

            services.AddJwksManager()
                .PersistKeysToDatabaseStore<AppDbContext>();

            services.ConfigureProviderForContext<AppDbContext>(ProviderConfiguration.DetectDatabase(configuration));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
