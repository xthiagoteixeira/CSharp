namespace Api_ContaCorrente.Domain.Entities
{
    public class Movimento
    {
        public int Id { get; private set; }
        public int IdContaCorrente { get; private set; }
        public string TipoMovimento { get; private set; } // C = Crédito, D = Débito
        public decimal Valor { get; private set; }
        public DateTime DataMovimento { get; private set; }

        // Construtor para criação de novo movimento
        public Movimento(int idContaCorrente, string tipoMovimento, decimal valor)
        {
            if (idContaCorrente <= 0)
                throw new ArgumentException("Id da conta corrente deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(tipoMovimento) || (tipoMovimento != "C" && tipoMovimento != "D"))
                throw new ArgumentException("Tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito)");

            if (valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            IdContaCorrente = idContaCorrente;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            DataMovimento = DateTime.UtcNow;
        }

        // Construtor para hidratação do banco de dados
        public Movimento(int id, int idContaCorrente, string tipoMovimento, decimal valor, DateTime dataMovimento)
        {
            Id = id;
            IdContaCorrente = idContaCorrente;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            DataMovimento = dataMovimento;
        }

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id deve ser maior que zero");

            Id = id;
        }

        public bool IsCredito => TipoMovimento == "C";
        public bool IsDebito => TipoMovimento == "D";
    }
}