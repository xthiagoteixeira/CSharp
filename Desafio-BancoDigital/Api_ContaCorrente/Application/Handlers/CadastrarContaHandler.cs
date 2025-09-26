using MediatR;
using Api_ContaCorrente.Application.Commands;
using Api_ContaCorrente.Domain.Entities;
using Api_ContaCorrente.Domain.Repositories;
using Api_ContaCorrente.Domain.Services;
using Api_ContaCorrente.Domain.ValueObjects;

namespace Api_ContaCorrente.Application.Handlers
{
    public class CadastrarContaHandler : IRequestHandler<CadastrarContaCommand, CadastrarContaResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IPasswordService _passwordService;

        public CadastrarContaHandler(IContaCorrenteRepository contaRepository, IPasswordService passwordService)
        {
            _contaRepository = contaRepository;
            _passwordService = passwordService;
        }

        public async Task<CadastrarContaResponse> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar CPF
                CPF cpf;
                try
                {
                    cpf = new CPF(request.CPF);
                }
                catch (ArgumentException ex)
                {
                    return new CadastrarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF inválido",
                        Erros = new List<string> { ex.Message }
                    };
                }

                // Verificar se CPF já está cadastrado
                if (await _contaRepository.ExistsByCPFAsync(cpf))
                {
                    return new CadastrarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF já cadastrado",
                        Erros = new List<string> { "Já existe uma conta para este CPF" }
                    };
                }

                // Validar senha
                if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6)
                {
                    return new CadastrarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Senha inválida",
                        Erros = new List<string> { "A senha deve ter pelo menos 6 caracteres" }
                    };
                }

                // Hash da senha
                var senhaHash = _passwordService.HashPassword(request.Senha);

                // Criar conta
                var conta = new ContaCorrente(cpf, senhaHash);
                
                // Gerar número da conta
                var numeroConta = await _contaRepository.GetNextNumeroAsync();
                conta.SetNumero(numeroConta);

                // Salvar no banco
                var contaId = await _contaRepository.CreateAsync(conta);
                conta.SetId(contaId);

                return new CadastrarContaResponse
                {
                    Id = contaId,
                    Numero = numeroConta,
                    CPF = cpf.FormatarCPF(),
                    Sucesso = true,
                    Mensagem = "Conta criada com sucesso"
                };
            }
            catch (InvalidOperationException ex)
            {
                // Erros de negócio (CPF duplicado, etc.)
                return new CadastrarContaResponse
                {
                    Sucesso = false,
                    Mensagem = "Dados já existem no sistema",
                    Erros = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                // Tratar especificamente erros Oracle ORA-00001
                if (ex.Message.Contains("ORA-00001") || ex.Message.Contains("restrição exclusiva"))
                {
                    return new CadastrarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF ou dados já cadastrados",
                        Erros = new List<string> { "Este CPF já possui uma conta no sistema ou houve conflito de dados. Tente novamente." }
                    };
                }

                return new CadastrarContaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}