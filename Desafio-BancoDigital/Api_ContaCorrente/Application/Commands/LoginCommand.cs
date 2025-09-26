using MediatR;

namespace Api_ContaCorrente.Application.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string CPF { get; set; }
        public string Senha { get; set; }

        public LoginCommand(string cpf, string senha)
        {
            CPF = cpf;
            Senha = senha;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int ContaId { get; set; }
        public string CPF { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}