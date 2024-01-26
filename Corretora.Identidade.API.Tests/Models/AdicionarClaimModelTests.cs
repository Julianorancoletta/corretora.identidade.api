using Corretora.Identidade.API.Tests.Fixtures;
using Moq.AutoMock;
using System.ComponentModel.DataAnnotations;
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

        //private void SimulateValidation(object model)
        //{
        //    var validationContext = new ValidationContext(model, null, null);
        //    var validationResults = new List<ValidationResult>();
        //    Validator.TryValidateObject(model, validationContext, validationResults, true);
        //    foreach (var validationResult in validationResults)
        //    {
        //        _controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
        //    }
        //}
    }
}
