using Corretora.Identidade.API.Models;
using Corretora.Identidade.Core.Domain;
using Microsoft.AspNetCore.Identity;

namespace Corretora.Identidade.API.Application.Services
{
    public interface IIdentidadeService
    {
        Task<UsuarioResponstaLoginModel> GerarJwt(string cpf);
        Task<RefreshToken?> ObterRedreshToken(Guid refreshToken);
        Task<bool> AdicionarClaims(AdicionarClaimModel adicionarClaim);
        SignInManager<IdentityUser> ObterSignInManager();
        UserManager<IdentityUser> ObterUserManager();
    }
}
