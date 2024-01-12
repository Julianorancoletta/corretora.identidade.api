using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetDevPack.Security.JwtExtensions;

namespace Corretora.Identidade.API.FuturoBuildingBlock.Identidade
{
    public static class JwtConfig
    {
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = configuration.Get<AppSettings>();
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings), "Configuração não informada para API de identidade");
            }

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true;
                bearerOptions.SetJwksOptions(new JwkOptions(appSettings.JwksUrl));
            });
        }
    }
}
