using Corretora.Identidade.API.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Models
{
    [ExcludeFromCodeCoverage]
    public record struct AutenticarUsuarioModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Cpf(ErrorMessage = "O campo {0} está em formato inválido")]
        public string Cpf { get; init; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 8)]
        public string Senha { get; init; }
    }
}
