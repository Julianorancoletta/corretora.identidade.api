using Corretora.Identidade.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace Corretora.Identidade.API.Extensions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class CpfAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var mensagemErro = string.Format(ErrorMessageString, validationContext?.DisplayName ?? "");

            var cpf = value?.ToString();
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return new ValidationResult(mensagemErro, new[] { validationContext?.MemberName ?? ""});
            }

            return Cpf.Validar(cpf) ? ValidationResult.Success : new ValidationResult(mensagemErro, new[] { validationContext?.MemberName ?? ""});
        }
    }
}
