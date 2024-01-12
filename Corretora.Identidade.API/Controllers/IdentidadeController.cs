using Corretora.Identidade.API.Application.Services;
using Corretora.Identidade.API.Models;
using Delivery.WebAPI.Core.Controllers;
using Delivery.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Corretora.Identidade.API.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    public sealed class IdentidadeController : MainController
    {
        private readonly IIdentidadeService _identidadeService;

        public IdentidadeController(IIdentidadeService identidadeService)
        {
            _identidadeService = identidadeService;
        }

        [AllowAnonymous]
        [HttpPost("registrar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registrar(RegistrarUsuarioModel registrarUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = registrarUsuario.Cpf,
                Email = registrarUsuario.Email,
                EmailConfirmed = true // TODO: Adicionar confirmação de email
            };

            var resultado = await _identidadeService.ObterUserManager().CreateAsync(usuario, registrarUsuario.Senha!);

            if (resultado.Succeeded)
            {
                return CustomResponse(await _identidadeService.GerarJwt(registrarUsuario.Cpf!));
            }

            foreach (var erro in resultado.Errors)
            {
                AddErrorToStack(erro.Description);
            }

            return CustomResponse();
        }

        [AllowAnonymous]
        [HttpPost("autenticar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Autenticar(AutenticarUsuarioModel autenticarUsuario)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);

            var resultado = await _identidadeService.ObterSignInManager().PasswordSignInAsync(autenticarUsuario.Cpf, autenticarUsuario.Senha, false, true);

            if (resultado.Succeeded)
            {
                return CustomResponse(await _identidadeService.GerarJwt(autenticarUsuario.Cpf));
            }

            if (resultado.IsLockedOut)
            {
                AddErrorToStack("Usuário temporariamente bloqueado.");
                return CustomResponse();
            }

            AddErrorToStack("Usuário ou Senha incorretos.");
            return CustomResponse();
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                AddErrorToStack("Refresh Token não informado");
                return CustomResponse();
            }

            var token = await _identidadeService.ObterRedreshToken(Guid.Parse(refreshToken));

            if (token == null)
            {
                AddErrorToStack("Refresh Token expirado");
                return CustomResponse();
            }

            return CustomResponse(await _identidadeService.GerarJwt(token.NomeUsuario!));
        }

        [ClaimsAuthorize("Identidade", "Admin")]
        [HttpPost("adicionar-claims")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarClaims(AdicionarClaimModel adicionarClaim)
        {
            var resultado = await _identidadeService.AdicionarClaims(adicionarClaim);

            if (!resultado)
            {
                AddErrorToStack("Não foi possível adicionar claim para este usuário");
                return CustomResponse();
            }

            return NoContent();
        }
    }
}
