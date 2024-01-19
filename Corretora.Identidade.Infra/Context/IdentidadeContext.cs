using Corretora.Identidade.Core.Domain;
using Delivery.Core.Data;
using Delivery.Core.Messages;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace Corretora.Identidade.Infra.Context
{
    public class IdentidadeContext : IdentityDbContext, ISecurityKeyContext, IUnitOfWork
    {
        public IdentidadeContext(DbContextOptions<IdentidadeContext> options) : base(options) { }
        public DbSet<KeyMaterial> SecurityKeys { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<ValidationResult>();
            builder.Ignore<Event>();

            builder.ApplyConfigurationsFromAssembly(typeof(IdentidadeContext).Assembly);
            base.OnModelCreating(builder);
        }

        public async Task<bool> Commit()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
