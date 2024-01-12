namespace Corretora.Identidade.Core.Domain
{
    public readonly record struct Cpf
    {
        public const int TAMANHO_MAXIMO = 11;

        public Cpf(string numero)
        {
            Numero = numero;
        }

        public readonly string Numero { get; }

        public static bool Validar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            cpf = ObterApenasNumeros(cpf?.ToString() ?? string.Empty);

            if (cpf.Length > TAMANHO_MAXIMO)
                return false;

            while (cpf.Length < TAMANHO_MAXIMO)
                cpf = '0' + cpf;

            if (TodosDigitosSaoIguais(cpf))
                return false;

            var multiplier1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplier2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);

            var soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplier1[i];
            }

            var rersultado = soma % TAMANHO_MAXIMO;
            rersultado = rersultado < 2 ? 0 : TAMANHO_MAXIMO - rersultado;

            var digito = rersultado.ToString();
            tempCpf = tempCpf + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplier2[i];
            }

            rersultado = soma % TAMANHO_MAXIMO;
            rersultado = rersultado < 2 ? 0 : TAMANHO_MAXIMO - rersultado;

            digito = rersultado.ToString();
            tempCpf = tempCpf + digito;

            return cpf.EndsWith(tempCpf);
        }

        private static string ObterApenasNumeros(string cpf)
        {
            return new string(cpf.Where(char.IsDigit)?.ToArray());
        }

        private static bool TodosDigitosSaoIguais(string cpf)
        {
            var primeiroDigito = cpf[0];
            return cpf.All(d => d == primeiroDigito);
        }
    }
}
