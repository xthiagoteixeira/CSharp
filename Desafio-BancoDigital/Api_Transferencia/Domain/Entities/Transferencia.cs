using Api_Transferencia.Domain.ValueObjects;

namespace Api_Transferencia.Domain.Entities
{
    public class Transferencia
    {
        public int Id { get; private set; }
        public int ContaOrigemId { get; private set; }
        public int ContaDestinoId { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public Valor Valor { get; private set; }
        public ChaveIdempotencia ChaveIdempotencia { get; private set; }

        // Construtor para criação de nova transferência
        public Transferencia(int contaOrigemId, int contaDestinoId, Valor valor, ChaveIdempotencia chaveIdempotencia)
        {
            if (contaOrigemId <= 0)
                throw new ArgumentException("ID da conta de origem deve ser maior que zero", nameof(contaOrigemId));
            
            if (contaDestinoId <= 0)
                throw new ArgumentException("ID da conta de destino deve ser maior que zero", nameof(contaDestinoId));
            
            if (contaOrigemId == contaDestinoId)
                throw new ArgumentException("Conta de origem e destino não podem ser iguais");

            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
            Valor = valor ?? throw new ArgumentNullException(nameof(valor));
            ChaveIdempotencia = chaveIdempotencia ?? throw new ArgumentNullException(nameof(chaveIdempotencia));
            DataMovimento = DateTime.UtcNow;
        }

        // Construtor para hidratação do banco de dados
        public Transferencia(int id, int contaOrigemId, int contaDestinoId, DateTime dataMovimento, 
            Valor valor, ChaveIdempotencia chaveIdempotencia)
        {
            Id = id;
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
            DataMovimento = dataMovimento;
            Valor = valor;
            ChaveIdempotencia = chaveIdempotencia;
        }

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id deve ser maior que zero");
            Id = id;
        }
    }
}