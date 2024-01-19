using Delivery.Core.DomainObjects;

namespace Corretora.Identidade.Core.Domain
{
    public sealed class RefreshToken : Entity, IAggregateRoot
    {
        public Guid Token { get; init; } = Guid.NewGuid();
        public string? NomeUsuario { get; init; }
        public DateTime DataExpiracao { get; init; }
    }
}
