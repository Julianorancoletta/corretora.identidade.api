using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Models
{
    [ExcludeFromCodeCoverage]
    public record struct UsuarioTokenModel(
        string Id,
        string Cpf,
        string Email,
        IEnumerable<UsuarioClaimModel> Claims);
}
