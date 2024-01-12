using Corretora.Identidade.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace Corretora.Identidade.Infra.Context
{
    public class AppDbContext : IdentityDbContext, ISecurityKeyContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<KeyMaterial> SecurityKeys { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
