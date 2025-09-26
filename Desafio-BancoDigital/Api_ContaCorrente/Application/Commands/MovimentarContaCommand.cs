using MediatR;

namespace Api_ContaCorrente.Application.Commands
{
    public class MovimentarContaCommand : IRequest<MovimentarContaResponse>
    {
        public int ContaId { get; set; }
        public string TipoMovimento { get; set; } // C = Crédito, D = Débito
        public decimal Valor { get; set; }

        public MovimentarContaCommand(int contaId, string tipoMovimento, decimal valor)
        {
            ContaId = contaId;
            TipoMovimento = tipoMovimento;
            Valor = valor;
        }
    }

    public class MovimentarContaResponse
    {
        public int MovimentoId { get; set; }
        public decimal SaldoAtual { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}