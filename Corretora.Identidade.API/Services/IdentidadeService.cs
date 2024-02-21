using Corretora.Identidade.API.Extensions;
using Corretora.Identidade.API.Models;
using Corretora.Identidade.Core.Domain;
using Corretora.Identidade.Infra.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Corretora.Identidade.API.Services
{
    public sealed class IdentidadeService : IIdentidadeService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppTokenSettings _appTokenSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwksService;
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentidadeService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppTokenSettings> appTokenSettings, IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService, IHttpContextAccessor contextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appTokenSettings = appTokenSettings.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _jwksService = jwtService;
            _contextAccessor = contextAccessor;
        }

        public async Task<UsuarioResponstaLoginModel> GerarJwtAsync(string cpf)
        {
            var usuario = await _userManager.FindByNameAsync(cpf);
            var claims = await _userManager.GetClaimsAsync(usuario);

            var identityClaims = await ObterClaimsUsuario(claims, usuario);
            var accessToken = await GerarToken(identityClaims);

            var refreshToken = await GerarRefreshToken(cpf);

            return ObterRespostaToken(accessToken, usuario, claims, refreshToken);
        }

        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);

            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, usuario.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var roleUsuario in roles)
            {
                claims.Add(new Claim("role", roleUsuario));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);
            return identityClaims;
        }

        private async Task<string> GerarToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = await _jwksService.GetCurrentSigningCredentials();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}",
                SigningCredentials = key
            };

            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        private UsuarioResponstaLoginModel ObterRespostaToken(string accessToken, IdentityUser usuario, IEnumerable<Claim> claims, RefreshToken refreshToken)
        {
            return new UsuarioResponstaLoginModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiraEm = TimeSpan.FromHours(1).TotalSeconds,
                UsuarioToken = new UsuarioTokenModel
                {
                    Id = usuario.Id,
                    Cpf = usuario.UserName!,
                    Email = usuario.Email!,
                    Claims = claims.Select(c => new UsuarioClaimModel(c.Type, c.Value))
                }
            };
        }

        private static long ToUnixEpochDate(DateTime data)
        {
            var dataPadrao = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return (long)Math.Round((data.ToUniversalTime() - dataPadrao).TotalSeconds);
        }

        private async Task<RefreshToken> GerarRefreshToken(string cpf)
        {
            var refreshToken = new RefreshToken
            {
                NomeUsuario = cpf,
                DataExpiracao = DateTime.UtcNow.AddHours(_appTokenSettings.HorasExpiracaoRefreshToken)
            };

            _refreshTokenRepository.DeleteByCpf(cpf);
            await _refreshTokenRepository.AddAsync(refreshToken);

            await _refreshTokenRepository.UnitOfWork.Commit();
            return refreshToken;
        }

        public async Task<RefreshToken?> ObterRedreshTokenAsync(Guid refreshToken)
        {
            var token = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(refreshToken);

            return token != null && token.DataExpiracao.ToLocalTime() > DateTime.Now
                ? token
                : null;
        }

        public async Task<bool> AdicionarClaimsAsync(AdicionarClaimModel adicionarClaim)
        {
            var usuario = await _userManager.FindByNameAsync(adicionarClaim.Cpf);

            if (usuario == null)
            {
                return false;
            }

            var claims = new List<Claim>();
            foreach (var claim in adicionarClaim.Claims)
            {
                claims.Add(new Claim(claim.Tipo, claim.Valor));
            }

            var resultado = await _userManager.AddClaimsAsync(usuario, claims);
            if (resultado.Succeeded)
            {
                return true;
            }

            return false;
        }

        public SignInManager<IdentityUser> ObterSignInManager()
        {
            return _signInManager;
        }

        public UserManager<IdentityUser> ObterUserManager()
        {
            return _userManager;
        }
    }
}
