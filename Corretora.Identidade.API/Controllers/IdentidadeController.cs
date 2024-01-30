using Corretora.Identidade.API.Models;
using Corretora.Identidade.API.Services;
using Delivery.WebAPI.Core.Controllers;
using Delivery.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Controllers
{
    [Route("api/[Controller]")]
    public sealed class IdentidadeController : MainController
    {
        private readonly IIdentidadeService _identidadeService;
        private readonly ILogger<IdentidadeController> _logger;

        [ExcludeFromCodeCoverage]
        public IdentidadeController(IIdentidadeService identidadeService, ILogger<IdentidadeController> logger)
        {
            _identidadeService = identidadeService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("registrar")]
        [ProducesResponseType(typeof(UsuarioResponstaLoginModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarUsuarioAsync(RegistrarUsuarioModel registrarUsuario)
        {
            _logger.LogInformation($"Registrando usuário CPF '{registrarUsuario.Cpf}'");
            var usuario = new IdentityUser
            {
                UserName = registrarUsuario.Cpf,
                Email = registrarUsuario.Email,
                EmailConfirmed = true // TODO: Adicionar confirmação de email
            };

            var resultado = await _identidadeService.ObterUserManager().CreateAsync(usuario, registrarUsuario.Senha!);

            if (resultado.Succeeded)
            {
                return CustomResponse(await _identidadeService.GerarJwtAsync(registrarUsuario.Cpf!));
            }

            _logger.LogError($"Falha ao registrar usuário CPF '{registrarUsuario.Cpf}'");
            foreach (var erro in resultado.Errors)
            {
                _logger.LogError($"Descrição do erro do CPF '{registrarUsuario.Cpf}' ==> {erro.Description}");
                AddErrorToStack(erro.Description);
            }

            return CustomResponse();
        }

        [AllowAnonymous]
        [HttpPost("autenticar")]
        [ProducesResponseType(typeof(UsuarioResponstaLoginModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AutenticarUsuarioAsync(AutenticarUsuarioModel autenticarUsuario)
        {
            _logger.LogInformation($"Autenticando usuário CPF '{autenticarUsuario.Cpf}'");
            var resultado = await _identidadeService.ObterSignInManager().PasswordSignInAsync(autenticarUsuario.Cpf, autenticarUsuario.Senha, false, true);

            if (resultado.Succeeded)
            {
                return CustomResponse(await _identidadeService.GerarJwtAsync(autenticarUsuario.Cpf));
            }

            _logger.LogError($"Falha ao autenticar usuário CPF '{autenticarUsuario.Cpf}'");
            if (resultado.IsLockedOut)
            {
                _logger.LogError($"Descrição do erro do CPF '{autenticarUsuario.Cpf}' ==> Usuário temporariamente bloqueado.");
                AddErrorToStack("Usuário temporariamente bloqueado.");
                return CustomResponse();
            }

            _logger.LogError($"Descrição do erro do CPF '{autenticarUsuario.Cpf}' ==> Usuário ou Senha incorretos.");
            AddErrorToStack("Usuário ou Senha incorretos.");
            return CustomResponse();
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(UsuarioResponstaLoginModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken)
        {
            _logger.LogInformation($"Refreshtoken '{refreshToken}'");
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                AddErrorToStack("Refresh Token não informado");
                return CustomResponse();
            }

            var token = await _identidadeService.ObterRedreshTokenAsync(Guid.Parse(refreshToken));
            _logger.LogInformation($"Refreshtoken usuário de nome '{token?.NomeUsuario}'");

            if (token == null)
            {
                _logger.LogError($"Falha refreshtoken '{refreshToken}' ==> Refresh Token expirado");
                AddErrorToStack("Refresh Token expirado");
                return CustomResponse();
            }

            return CustomResponse(await _identidadeService.GerarJwtAsync(token.NomeUsuario!));
        }

        [Authorize]
        [ClaimsAuthorize("Identidade", "Admin")]
        [HttpPost("adicionar-claims")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdicionarClaimsAsync(AdicionarClaimModel adicionarClaim)
        {
            _logger.LogInformation($"Adicionando claim para o CPF '{adicionarClaim.Cpf}'");
            var resultado = await _identidadeService.AdicionarClaimsAsync(adicionarClaim);

            if (!resultado)
            {
                _logger.LogError($"Falha ao adicionar claim para o CPF '{adicionarClaim.Cpf}' ==> Não foi possível adicionar claim para este usuário");
                AddErrorToStack("Não foi possível adicionar claim para este usuário");
                return CustomResponse();
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("validar-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidarToken()
        {
            return NoContent();
        }
    }
}
