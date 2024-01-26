﻿using Corretora.Identidade.API.Extensions;
using Corretora.Identidade.Infra.Context;
using Delivery.Core.DatabaseFlavor;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

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
    }
}
