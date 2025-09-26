using MediatR;

namespace Api_ContaCorrente.Application.Commands
{
    public class CadastrarContaCommand : IRequest<CadastrarContaResponse>
    {
        public string CPF { get; set; }
        public string Senha { get; set; }

        public CadastrarContaCommand(string cpf, string senha)
        {
            CPF = cpf;
            Senha = senha;
        }
    }

    public class CadastrarContaResponse
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string CPF { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}