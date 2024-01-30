using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Models
{
    public record struct UsuarioClaimModel
    {
        public UsuarioClaimModel(string valor, string tipo)
        {
            Valor = valor;
            Tipo = tipo;
        }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Valor { get; init; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Tipo { get; init; }
    }
}
