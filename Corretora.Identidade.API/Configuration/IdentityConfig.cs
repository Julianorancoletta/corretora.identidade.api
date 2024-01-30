using Delivery.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using NetDevPack.Security.JwtExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Corretora.Identidade.Infra.Context;
using Delivery.Core.DatabaseFlavor;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using Corretora.Identidade.API.Extensions;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace Corretora.Identidade.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class IdentityConfig
    {
        public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppTokenSettings");
            services.Configure<AppTokenSettings>(appSettingsSection);

            services.AddJwksManager()
                .PersistKeysToDatabaseStore<IdentidadeContext>();

            services.ConfigureProviderForContext<IdentidadeContext>(ProviderConfiguration.DetectDatabase(configuration));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentidadeContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddJwtAsyncKeyConfigurationIdentidade(this IServiceCollection services, IConfiguration configuration)
        {
            //
            //services.Configure<AppSettings>(section);
            //var jwtSettings = section.Get<AppSettings>();
            //if (jwtSettings == null)
            //{
            //    throw new ArgumentNullException("jwtSettings", "Configuração de JWT não informada, necessário informar JwtSettings");
            //}

            //services.AddAuthentication(delegate (AuthenticationOptions o)
            //{
            //    o.DefaultAuthenticateScheme = "Bearer";
            //    o.DefaultChallengeScheme = "Bearer";
            //}).AddJwtBearer(delegate (JwtBearerOptions bearerOptions)
            //{
            //    bearerOptions.RequireHttpsMetadata = true;
            //    bearerOptions.SaveToken = true;
            //    //bearerOptions.SetJwksOptions(teste);
            //    bearerOptions.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
            //        ValidateIssuer = true,
            //        ValidateAudience = false,
            //        ValidAudience = jwtSettings.ValidoEm,
            //        ValidIssuer = jwtSettings.Emissor
            //    };
            //});

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor
                };
            });
        }
    }
}
