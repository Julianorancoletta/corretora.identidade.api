using Corretora.Identidade.API.Controllers;
using Corretora.Identidade.API.Models;
using Corretora.Identidade.API.Services;
using Corretora.Identidade.API.Tests.Fixtures;
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

        [Trait("Categoria", "Corretora Identidade - Controller")]
        [Fact(DisplayName = "Registrar Usuário Post Válido Retorna 200")]
        public async Task RegistrarUsuario_PostValido_Retorna200OkResponseAsync()
        {
            // Arrange
            var responseMock = new UsuarioResponstaLoginModel();

            var identityResultMock = _mocker.GetMock<IdentityResult>();
            var propertyInfo = typeof(IdentityResult).GetProperty("Succeeded");
            propertyInfo?.SetValue(identityResultMock.Object, true);

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
            var result = await _controller.RegistrarUsuarioAsync(new RegistrarUsuarioModel());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            identidadeServiceMock.Verify(s => s.GerarJwtAsync(It.IsAny<string>()), Times.Once);
            var objectResult = result as OkObjectResult;
            var response = objectResult?.Value as UsuarioResponstaLoginModel?;
            Assert.Equal(response, responseMock);
        }

        [Trait("Categoria", "Corretora Identidade - Controller")]
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
            var result = await _controller.RegistrarUsuarioAsync(new RegistrarUsuarioModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            var objectResult = result as BadRequestObjectResult;
            var response = objectResult?.Value as ValidationProblemDetails;
            Assert.IsType<ValidationProblemDetails>(response);
        }

        [Trait("Categoria", "Corretora Identidade - Controller")]
        [Fact(DisplayName = "Autenticar Usuário Post Válido Retorna 200")]
        public async Task AutenticarUsuario_PostValido_Retorna200OkResponseAsync()
        {
            // Arrange
            var responseMock = new UsuarioResponstaLoginModel();

            var signInResultMock = _mocker.GetMock<Microsoft.AspNetCore.Identity.SignInResult>();
            var propertyInfo = typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("Succeeded");
            propertyInfo?.SetValue(signInResultMock.Object, true);

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

        [Trait("Categoria", "Corretora Identidade - Controller")]
        [Fact(DisplayName = "Autenticar Usuário Bloqueado Retorna 400")]
        public async Task AutenticarUsuario_UsuarioBloqueado_Retorna400BadRequestResponseAsync()
        {
            // Arrange
            var signInResultMock = _mocker.GetMock<Microsoft.AspNetCore.Identity.SignInResult>();
            var propertyInfo = typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("IsLockedOut");
            propertyInfo?.SetValue(signInResultMock.Object, true);

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

        [Trait("Categoria", "Corretora Identidade - Controller")]
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

        [Trait("Categoria", "Corretora Identidade - Controller")]
        [Fact(DisplayName = "Refresh Token Post Válido Retorna 200")]
        public async Task RefreshToken_PostValido_Retorna200OkResponseAsync()
        {
            // Arrange
            var responseMock = new UsuarioResponstaLoginModel();

            var signInResultMock = _mocker.GetMock<Microsoft.AspNetCore.Identity.SignInResult>();
            var propertyInfo = typeof(Microsoft.AspNetCore.Identity.SignInResult).GetProperty("Succeeded");
            propertyInfo?.SetValue(signInResultMock.Object, true);

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
    }
}
