using Corretora.Identidade.API.Models;
using Corretora.Identidade.API.Tests.Fixtures;
using Moq.AutoMock;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Tests.Models
{
    [ExcludeFromCodeCoverage]
    [Collection(nameof(IdentidadeCollection))]
    public class AdicionarClaimModelTests : IClassFixture<IdentidadeTestsFixture>
    {
        private readonly AutoMocker _mocker;
        private readonly IdentidadeTestsFixture _fixture;

        public AdicionarClaimModelTests(IdentidadeTestsFixture fixture)
        {
            _mocker = new AutoMocker();
            _fixture = fixture;
        }

        [Trait("Categoria", "Corretora Identidade - Models")]
        [Theory(DisplayName = "Adicionar Claims Model Data Annotation Valido Retorna Sucesso")]
        [InlineData(false)]
        [InlineData(true)]
        public void AdicionarClaimsModel_DataAnnotationValido_RetornaSucesso(bool cpfComMenosDeOnzeDigitos)
        {
            // Arrange
            var model = new AdicionarClaimModel(cpfComMenosDeOnzeDigitos ? IdentidadeTestsFixture.GerarCpfComPrimeiroDigitoZero() : IdentidadeTestsFixture.GerarCpf(), new List<UsuarioClaimModel>());

            // Act
            var results = IdentidadeTestsFixture.SimularValidacaoModel(model);

            // Assert
            Assert.Empty(results.Select(result => result.ErrorMessage));
        }

        [Trait("Categoria", "Corretora Identidade - Models")]
        [Theory(DisplayName = "Adicionar Claims Model Cpf Com Espaços Em Branco Retorna Sucesso E Cpf É Aparado")]
        [InlineData(false)]
        [InlineData(true)]
        public void AdicionarClaimsModel_CpfComEspacosEmBranco_RetornaSucessoECpfEhAparado(bool cpfComMenosDeOnzeDigitos)
        {
            // Arrange
            var cpf = cpfComMenosDeOnzeDigitos ? IdentidadeTestsFixture.GerarCpfComPrimeiroDigitoZero() : IdentidadeTestsFixture.GerarCpf();
            var model = new AdicionarClaimModel($" {cpf} ", new List<UsuarioClaimModel>());

            // Act
            var results = IdentidadeTestsFixture.SimularValidacaoModel(model);

            // Assert
            Assert.Empty(results.Select(result => result.ErrorMessage));
            Assert.DoesNotContain(" ", cpf);
        }

        [Trait("Categoria", "Corretora Identidade - Models")]
        [Theory(DisplayName = "Adicionar Claims Model Cpf Vazio Ou Nulo Retorna Erro")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void AdicionarClaimsModel_CpfVazioOuNulo_RetornaErro(string cpf)
        {
            // Arrange
            var model = new AdicionarClaimModel(cpf, new List<UsuarioClaimModel>());

            // Act
            var results = IdentidadeTestsFixture.SimularValidacaoModel(model);

            // Assert
            var error = results.Select(result => result.ErrorMessage).First();
            Assert.Equal($"O campo {nameof(AdicionarClaimModel.Cpf)} é obrigatório", error);
        }

        [Trait("Categoria", "Corretora Identidade - Models")]
        [Fact(DisplayName = "Adicionar Claims Model Cpf Invalido Retorna Erro")]
        public void AdicionarClaimsModel_CpfInvalido_RetornaErro()
        {
            // Arrange
            var model = new AdicionarClaimModel("123456789", new List<UsuarioClaimModel>());

            // Act
            var results = IdentidadeTestsFixture.SimularValidacaoModel(model);

            // Assert
            var error = results.Select(result => result.ErrorMessage).First();
            Assert.Equal($"O campo {nameof(AdicionarClaimModel.Cpf)} está em formato inválido", error);
        }

        [Trait("Categoria", "Corretora Identidade - Models")]
        [Theory(DisplayName = "Adicionar Claims Model Cpf Vazio Ou Nulo Retorna Erro")]
        [InlineData("0000", false)]
        [InlineData("0000", true)]
        public void AdicionarClaimsModel_CpfValidoComCaracteresAntes_RetornaErro(string espaco, bool cpfComMenosDeOnzeDigitos)
        {
            // Arrange
            var cpf = cpfComMenosDeOnzeDigitos ? IdentidadeTestsFixture.GerarCpfComPrimeiroDigitoZero() : IdentidadeTestsFixture.GerarCpf();
            var model = new AdicionarClaimModel($"{espaco}{cpf}", new List<UsuarioClaimModel>());

            // Act
            var results = IdentidadeTestsFixture.SimularValidacaoModel(model);

            // Assert
            var error = results.Select(result => result.ErrorMessage).First();
            Assert.Equal($"O campo {nameof(AdicionarClaimModel.Cpf)} está em formato inválido", error);
        }

        //[Trait("Categoria", "Corretora Identidade - Models")]
        //[Fact(DisplayName = "Adicionar Claims Model Claims Vazias Retorna Erro")]
        //public void AdicionarClaimsModel_ClaimsVazias_RetornaErro()
        //{
        //    // Arrange
        //    var model = new AdicionarClaimModel(IdentidadeTestsFixture.GerarCpf(), null);

        //    // Act
        //    var results = IdentidadeTestsFixture.SimularValidacaoModel(model);

        //    // Assert
        //    var error = results.Select(result => result.ErrorMessage).First();
        //    Assert.Equal($"O campo {nameof(AdicionarClaimModel.Cpf)} está em formato inválido", error);
        //}
    }
}
