using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Models
{
    [ExcludeFromCodeCoverage]
    public record struct UsuarioClaimModel(
        string Valor,
        string Tipo);
}
