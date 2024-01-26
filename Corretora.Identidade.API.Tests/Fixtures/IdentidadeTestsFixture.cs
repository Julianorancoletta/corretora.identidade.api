using Bogus;
using Bogus.DataSets;
using System.Diagnostics.CodeAnalysis;

namespace Corretora.Identidade.API.Tests.Fixtures
{
    [ExcludeFromCodeCoverage]
    [CollectionDefinition(nameof(IdentidadeTestsFixture))]
    public sealed class IdentidadeCollection : ICollectionFixture<IdentidadeTestsFixture> { }

    [ExcludeFromCodeCoverage]
    public sealed class IdentidadeTestsFixture
    {
        public IEnumerable<UsuarioTestClass> GerarDadosUsuarios(int quantidade)
        {
            var faker = new Faker("pt_BR");

            var usuarioFaker = new Faker<UsuarioTestClass>("pt_BR")
                .RuleFor(u => u.Cpf, () => GerarCpf())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Senha, f => f.Internet.PasswordCustom())
                .RuleFor(u => u.SenhaConfirmacao, (f, u) => u.Senha);
            return usuarioFaker.Generate(quantidade);
        }

        public static string GerarCpf()
        {
            var random = new Random();
            var cpfDigits = Enumerable.Range(0, 9)
                .Select(_ => random.Next(10))
                .ToArray();

            var cpf = string.Join("", cpfDigits);

            cpf += CalcularModulo11Cpf(cpf, 9);
            cpf += CalcularModulo11Cpf(cpf, 10);

            return cpf;
        }

        private static int CalcularModulo11Cpf(string cpf, int position)
        {
            var total = 0;
            for (var i = 0; i < position; i++)
            {
                total += int.Parse(cpf[i].ToString()) * (position + 1 - i);
            }

            var remainder = total % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }
    }

    public static class BogusExtensions
    {
        public static string PasswordCustom(this Internet internet)
        {
            var number = internet.Random.Replace("#");
            var letter = internet.Random.Replace("?");
            var lowerLetter = letter.ToLower();
            var symbol = internet.Random.Char((char)33, (char)47);
            var padding = internet.Random.String2(4);
            return new string(internet.Random.Shuffle($"{number}{letter}{lowerLetter}{symbol}{padding}").ToArray());
        }
    }

    [ExcludeFromCodeCoverage]
    public sealed class UsuarioTestClass
    {
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string SenhaConfirmacao { get; set; }
    }

}
