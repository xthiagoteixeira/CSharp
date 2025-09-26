using MediatR;

namespace Api_ContaCorrente.Application.Queries
{
    public class ConsultarSaldoQuery : IRequest<ConsultarSaldoResponse>
    {
        public int ContaId { get; set; }

        public ConsultarSaldoQuery(int contaId)
        {
            ContaId = contaId;
        }
    }

    public class ConsultarSaldoResponse
    {
        public int ContaId { get; set; }
        public int Numero { get; set; }
        public string CPF { get; set; }
        public decimal Saldo { get; set; }
        public bool ContaAtiva { get; set; }
        public DateTime DataConsulta { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}