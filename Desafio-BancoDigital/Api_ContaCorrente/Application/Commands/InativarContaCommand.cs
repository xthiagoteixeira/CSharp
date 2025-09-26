using MediatR;

namespace Api_ContaCorrente.Application.Commands
{
    public class InativarContaCommand : IRequest<InativarContaResponse>
    {
        public int ContaId { get; set; }

        public InativarContaCommand(int contaId)
        {
            ContaId = contaId;
        }
    }

    public class InativarContaResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}