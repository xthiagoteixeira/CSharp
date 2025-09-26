using System.Text.RegularExpressions;

namespace Api_ContaCorrente.Domain.ValueObjects
{
    public class CPF
    {
        public string Numero { get; private set; }

        public CPF(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("CPF não pode ser vazio");

            var cpfLimpo = LimparCPF(numero);
            
            if (!ValidarCPF(cpfLimpo))
                throw new ArgumentException("CPF inválido");

            Numero = cpfLimpo;
        }

        private string LimparCPF(string cpf)
        {
            return Regex.Replace(cpf, @"[^\d]", "");
        }

        private bool ValidarCPF(string cpf)
        {
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
                return false;

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);

            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cpf[10].ToString()) == digitoVerificador2;
        }

        public override string ToString()
        {
            return Numero;
        }

        public string FormatarCPF()
        {
            return $"{Numero.Substring(0, 3)}.{Numero.Substring(3, 3)}.{Numero.Substring(6, 3)}-{Numero.Substring(9, 2)}";
        }

        public override bool Equals(object obj)
        {
            if (obj is CPF outroCpf)
                return Numero == outroCpf.Numero;
            return false;
        }

        public override int GetHashCode()
        {
            return Numero.GetHashCode();
        }
    }
}