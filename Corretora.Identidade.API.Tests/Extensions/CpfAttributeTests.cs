using Corretora.Identidade.API.Extensions;
using Corretora.Identidade.API.Tests.Fixtures;
using Moq.AutoMock;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Tests.Extensions
{
    [ExcludeFromCodeCoverage]
    public class CpfAttributeTests
    {
        private readonly AutoMocker _mocker;
        private readonly CpfAttribute _cpfAttribute;

        public CpfAttributeTests()
        {
            _mocker = new AutoMocker();
            _cpfAttribute = _mocker.CreateInstance<CpfAttribute>();
        }

        [Trait("Categoria", "Corretora Identidade - Extensions")]
        [Fact(DisplayName = "Cpf Attribute Recebe Cpf Válido Retorna Sucesso")]
        public void CpfAttribute_RecebeCpfValido_RetornaSucesso()
        {
            // Act & Assert
            Assert.True(_cpfAttribute.IsValid(IdentidadeTestsFixture.GerarCpf()));
        }

        [Trait("Categoria", "Corretora Identidade - Extensions")]
        [Theory(DisplayName = "Cpf Attribute Recebe Cpf Vazio Ou Nulo Retorna Falha")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CpfAttribute_RecebeCpfVazioOuNulo_RetornaFalha(string cpf)
        {
            // Act & Assert
            Assert.False(_cpfAttribute.IsValid(cpf));
        }

        [Trait("Categoria", "Corretora Identidade - Extensions")]
        [Fact(DisplayName = "Cpf Attribute Recebe Cpf Inválido Retorna Falha")]
        public void CpfAttribute_RecebeCpfInvalido_RetornaFalha()
        {
            // Act & Assert
            Assert.False(_cpfAttribute.IsValid("123"));
        }
    }
}
