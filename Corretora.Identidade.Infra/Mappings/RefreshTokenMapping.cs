using Corretora.Identidade.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corretora.Identidade.Infra.Mappings
{
    public sealed class RefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Token).IsRequired();
            builder.Property(r => r.NomeUsuario).IsRequired();
            builder.Property(r => r.DataExpiracao).IsRequired();
        }
    }
}
