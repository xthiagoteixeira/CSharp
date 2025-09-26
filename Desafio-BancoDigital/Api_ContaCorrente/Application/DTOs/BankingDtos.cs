namespace Api_ContaCorrente.Application.DTOs
{
    public class ContaDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string CPF { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public bool Ativa { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class MovimentoDto
    {
        public int Id { get; set; }
        public int IdContaCorrente { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
        public string DescricaoTipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
    }

    public class CadastrarContaDto
    {
        public string CPF { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string CPF { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class MovimentarContaDto
    {
        public int ContaId { get; set; }
        public string TipoMovimento { get; set; } = string.Empty; // C = Crédito, D = Débito
        public decimal Valor { get; set; }
    }
}