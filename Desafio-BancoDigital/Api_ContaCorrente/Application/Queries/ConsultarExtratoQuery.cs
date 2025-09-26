using MediatR;

namespace Api_ContaCorrente.Application.Queries
{
    public class ConsultarExtratoQuery : IRequest<ConsultarExtratoResponse>
    {
        public int ContaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public ConsultarExtratoQuery(int contaId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            ContaId = contaId;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }
    }

    public class ConsultarExtratoResponse
    {
        public int ContaId { get; set; }
        public int Numero { get; set; }
        public string CPF { get; set; }
        public decimal SaldoAtual { get; set; }
        public List<MovimentoDto> Movimentos { get; set; } = new();
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new();
    }

    public class MovimentoDto
    {
        public int Id { get; set; }
        public string TipoMovimento { get; set; }
        public string DescricaoTipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
    }
}