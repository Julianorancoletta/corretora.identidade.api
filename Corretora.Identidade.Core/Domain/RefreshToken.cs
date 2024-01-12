namespace Corretora.Identidade.API.Models
{
    public record class RefreshToken
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid Token { get; init; } = Guid.NewGuid();
        public string? NomeUsuario { get; init; }
        public DateTime DataExpiracao { get; init; }
    }
}
