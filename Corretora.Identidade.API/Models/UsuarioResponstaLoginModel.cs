namespace Corretora.Identidade.API.Models
{
    public record struct UsuarioResponstaLoginModel(
        string AccessToken,
        Guid RefreshToken,
        double ExpiraEm,
        UsuarioTokenModel UsuarioToken
        );
}
