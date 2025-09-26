using MediatR;
using Api_ContaCorrente.Application.Commands;
using Api_ContaCorrente.Domain.Repositories;
using Api_ContaCorrente.Domain.Services;
using Api_ContaCorrente.Domain.ValueObjects;

namespace Api_ContaCorrente.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public LoginHandler(IContaCorrenteRepository contaRepository, IPasswordService passwordService, IJwtService jwtService)
        {
            _contaRepository = contaRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar CPF
                CPF cpf;
                try
                {
                    cpf = new CPF(request.CPF);
                }
                catch (ArgumentException)
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF ou senha inválidos",
                        Erros = new List<string> { "Credenciais inválidas" }
                    };
                }

                // Buscar conta por CPF
                var conta = await _contaRepository.GetByCPFAsync(cpf);
                if (conta == null)
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF ou senha inválidos",
                        Erros = new List<string> { "Credenciais inválidas" }
                    };
                }

                // Verificar se a conta está ativa
                if (!conta.Ativa)
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta inativa",
                        Erros = new List<string> { "Esta conta foi desativada" }
                    };
                }

                // Verificar senha
                if (!_passwordService.VerifyPassword(request.Senha, conta.SenhaHash))
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF ou senha inválidos",
                        Erros = new List<string> { "Credenciais inválidas" }
                    };
                }

                // Gerar token JWT
                var token = _jwtService.GenerateToken(conta);

                return new LoginResponse
                {
                    Token = token,
                    ContaId = conta.Id,
                    CPF = conta.CPF.FormatarCPF(),
                    Sucesso = true,
                    Mensagem = "Login realizado com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}