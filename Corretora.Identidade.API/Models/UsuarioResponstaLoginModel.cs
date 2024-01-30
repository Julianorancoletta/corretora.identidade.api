using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Models
{
    [ExcludeFromCodeCoverage]
    public record struct UsuarioResponstaLoginModel(
        string AccessToken,
        Guid RefreshToken,
        double ExpiraEm,
        UsuarioTokenModel UsuarioToken
        );
}
