using Corretora.Identidade.API.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Corretora.Identidade.API.Models
{
    public record struct AdicionarClaimModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Cpf(ErrorMessage = "O campo {0} está em formato inválido")]
        public string Cpf { get; init; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public IEnumerable<UsuarioClaimModel> Claims { get; init; }
    }
}
