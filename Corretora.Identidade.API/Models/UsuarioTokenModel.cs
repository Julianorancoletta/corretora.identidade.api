namespace Corretora.Identidade.API.Models
{
    public record struct UsuarioTokenModel(
        string Id,
        string Cpf,
        string Email,
        IEnumerable<UsuarioClaimModel> Claims);
}
