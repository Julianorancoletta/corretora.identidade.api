using Corretora.Identidade.API.Controllers;
using Corretora.Identidade.API.Models;
using Corretora.Identidade.API.Services;
using Corretora.Identidade.API.Tests.Fixtures;
using Corretora.Identidade.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Tests.Controllers
{
    [ExcludeFromCodeCoverage]
    [Collection(nameof(IdentidadeCollection))]
    public class IdentidadeControllerTests : IClassFixture<IdentidadeTestsFixture>
    {
        private readonly AutoMocker _mocker;
        private readonly IdentidadeTestsFixture _fixture;
        private readonly IdentidadeController _controller;

        public IdentidadeControllerTests(IdentidadeTestsFixture fixture)
        {
            _mocker = new AutoMocker();
            _fixture = fixture;
            _controller = _mocker.CreateInstance<IdentidadeController>();
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Registrar Usuário Post Válido Retorna 200")]
        public async Task RegistrarUsuario_PostValido_Retorna200OkResponseAsync()
        {
            // Arrange
            var responseMock = new UsuarioResponstaLoginModel();

            var identityResultMock = _mocker.GetMock<IdentityResult>();
            IdentidadeTestsFixture.SetProtectedProperty<IdentityResult, bool>("Succeeded", identityResultMock.Object, true);

            var userManagerMock = _mocker.GetMock<UserManager<IdentityUser>>();
            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    return identityResultMock.Object;
                });

            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(s => s.ObterUserManager()).Returns(userManagerMock.Object);
            identidadeServiceMock.Setup(s => s.GerarJwtAsync(It.IsAny<string>())).ReturnsAsync(responseMock);

            // Act
            var result = await _controller.RegistrarUsuarioAsync(new RegistrarUsuarioModel("", "", "", ""));

            // Assert
            Assert.IsType<OkObjectResult>(result);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            identidadeServiceMock.Verify(s => s.GerarJwtAsync(It.IsAny<string>()), Times.Once);
            var objectResult = result as OkObjectResult;
            var response = objectResult?.Value as UsuarioResponstaLoginModel?;
            Assert.Equal(response, responseMock);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Registrar Usuário Post Inválido Retorna 400")]
        public async Task RegistrarUsuario_PostInvalido_Retorna400BadRequestAsync()
        {
            // Arrange
            var userManagerMock = _mocker.GetMock<UserManager<IdentityUser>>();
            userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    return IdentityResult.Failed(new IdentityError[]
                    {
                        new IdentityError
                        {
                            Code = "200",
                            Description = "Teste"
                        }
                    });
                });

            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(s => s.ObterUserManager()).Returns(userManagerMock.Object);

            // Act
            var result = await _controller.RegistrarUsuarioAsync(new RegistrarUsuarioModel("", "", "", ""));

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            var objectResult = result as BadRequestObjectResult;
            var response = objectResult?.Value as ValidationProblemDetails;
            Assert.IsType<ValidationProblemDetails>(response);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Autenticar Usuário Post Válido Retorna 200")]
        public async Task AutenticarUsuario_PostValido_Retorna200OkResponseAsync()
        {
            // Arrange
            var responseMock = new UsuarioResponstaLoginModel();

            var signInResultMock = _mocker.GetMock<Microsoft.AspNetCore.Identity.SignInResult>();
            IdentidadeTestsFixture.SetProtectedProperty<Microsoft.AspNetCore.Identity.SignInResult, bool>("Succeeded", signInResultMock.Object, true);

            var signInManagerMock = _mocker.GetMock<SignInManager<IdentityUser>>();
            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(() =>
                {
                    return signInResultMock.Object;
                });

            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(s => s.ObterSignInManager()).Returns(signInManagerMock.Object);
            identidadeServiceMock.Setup(s => s.GerarJwtAsync(It.IsAny<string>())).ReturnsAsync(responseMock);

            // Act
            var result = await _controller.AutenticarUsuarioAsync(new AutenticarUsuarioModel());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            signInManagerMock.Verify(u => u.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true), Times.Once);
            identidadeServiceMock.Verify(s => s.GerarJwtAsync(It.IsAny<string>()), Times.Once);
            var objectResult = result as OkObjectResult;
            var response = objectResult?.Value as UsuarioResponstaLoginModel?;
            Assert.Equal(response, responseMock);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Autenticar Usuário Bloqueado Retorna 400")]
        public async Task AutenticarUsuario_UsuarioBloqueado_Retorna400BadRequestResponseAsync()
        {
            // Arrange
            var signInResultMock = _mocker.GetMock<Microsoft.AspNetCore.Identity.SignInResult>();
            IdentidadeTestsFixture.SetProtectedProperty<Microsoft.AspNetCore.Identity.SignInResult, bool>("IsLockedOut", signInResultMock.Object, true);

            var signInManagerMock = _mocker.GetMock<SignInManager<IdentityUser>>();
            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(() =>
                {
                    return signInResultMock.Object;
                });

            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(s => s.ObterSignInManager()).Returns(signInManagerMock.Object);

            // Act
            var result = await _controller.AutenticarUsuarioAsync(new AutenticarUsuarioModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            signInManagerMock.Verify(u => u.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true), Times.Once);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Autenticar Usuário Credenciais Inválidas Retorna 400")]
        public async Task AutenticarUsuario_CredenciaisInvalidas_Retorna400BadRequestResponseAsync()
        {
            // Arrange
            var signInManagerMock = _mocker.GetMock<SignInManager<IdentityUser>>();
            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(() =>
                {
                    return _mocker.GetMock<Microsoft.AspNetCore.Identity.SignInResult>().Object;
                });

            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(s => s.ObterSignInManager()).Returns(signInManagerMock.Object);

            // Act
            var result = await _controller.AutenticarUsuarioAsync(new AutenticarUsuarioModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            signInManagerMock.Verify(u => u.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true), Times.Once);
            var objectResult = result as BadRequestObjectResult;
            var response = objectResult?.Value as ValidationProblemDetails;
            Assert.Contains("Usuário ou Senha incorretos.", response!.Errors["Messages"]);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Refresh Token Post Válido Retorna 200")]
        public async Task RefreshToken_PostValido_Retorna200OkResponseAsync()
        {
            // Arrange
            var responseMock = new UsuarioResponstaLoginModel();

            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(s => s.ObterRedreshTokenAsync(It.IsAny<Guid>())).ReturnsAsync(new RefreshToken());
            identidadeServiceMock.Setup(s => s.GerarJwtAsync(It.IsAny<string>())).ReturnsAsync(responseMock);

            // Act
            var result = await _controller.RefreshTokenAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            identidadeServiceMock.Verify(s => s.ObterRedreshTokenAsync(It.IsAny<Guid>()), Times.Once);
            identidadeServiceMock.Verify(s => s.GerarJwtAsync(It.IsAny<string>()), Times.Once);
            var objectResult = result as OkObjectResult;
            var response = objectResult?.Value as UsuarioResponstaLoginModel?;
            Assert.Equal(response, responseMock);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Theory(DisplayName = "Refresh Token Token Em Branco Ou Nulo Retorna 400")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RefreshToken_TokenEmBrancoOuNulo_Retorna400BadRequestAsync(string token)
        {
            // Arrange
            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();

            // Act
            var result = await _controller.RefreshTokenAsync(token);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var objectResult = result as BadRequestObjectResult;
            var response = objectResult?.Value as ValidationProblemDetails;
            Assert.Contains("Refresh Token não informado", response!.Errors["Messages"]);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Refresh Token Com Token Expirado Retorna 400")]
        public async Task RefreshToken_TokenExpirado_Retorna400BadRequestAsync()
        {
            // Arrange
            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();

            // Act
            var result = await _controller.RefreshTokenAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            identidadeServiceMock.Verify(s => s.ObterRedreshTokenAsync(It.IsAny<Guid>()), Times.Once);
            var objectResult = result as BadRequestObjectResult;
            var response = objectResult?.Value as ValidationProblemDetails;
            Assert.Contains("Refresh Token expirado", response!.Errors["Messages"]);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Adicionar Claims Para Usuário Post Válido Retorna 204")]
        public async Task AdicionarClaims_PostValido_Retorna204NoContentResponseAsync()
        {
            // Arrange
            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();
            identidadeServiceMock.Setup(i => i.AdicionarClaimsAsync(It.IsAny<AdicionarClaimModel>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AdicionarClaimsAsync(new AdicionarClaimModel());

            // Assert
            Assert.IsType<NoContentResult>(result);
            identidadeServiceMock.Verify(s => s.AdicionarClaimsAsync(It.IsAny<AdicionarClaimModel>()), Times.Once);
        }

        [Trait("Categoria", "Corretora Identidade - Controllers")]
        [Fact(DisplayName = "Adicionar Claims Para Usuário Falha Ao Adicionar Claim Retorna 400")]
        public async Task AdicionarClaims_FalhaAoAdicionarClaim_Retorna400BadRequestAsync()
        {
            // Arrange
            var identidadeServiceMock = _mocker.GetMock<IIdentidadeService>();

            // Act
            var result = await _controller.AdicionarClaimsAsync(new AdicionarClaimModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            identidadeServiceMock.Verify(s => s.AdicionarClaimsAsync(It.IsAny<AdicionarClaimModel>()), Times.Once);
            var objectResult = result as BadRequestObjectResult;
            var response = objectResult?.Value as ValidationProblemDetails;
            Assert.Contains("Não foi possível adicionar claim para este usuário", response!.Errors["Messages"]);
        }
    }
}
