using MediatR;
using Api_ContaCorrente.Application.Commands;
using Api_ContaCorrente.Domain.Repositories;

namespace Api_ContaCorrente.Application.Handlers
{
    public class InativarContaHandler : IRequestHandler<InativarContaCommand, InativarContaResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;

        public InativarContaHandler(IContaCorrenteRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<InativarContaResponse> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar conta
                var conta = await _contaRepository.GetByIdAsync(request.ContaId);
                if (conta == null)
                {
                    return new InativarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta não encontrada",
                        Erros = new List<string> { "A conta especificada não existe" }
                    };
                }

                // Inativar conta
                try
                {
                    conta.Inativar();
                }
                catch (InvalidOperationException ex)
                {
                    return new InativarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Erro ao inativar conta",
                        Erros = new List<string> { ex.Message }
                    };
                }

                // Atualizar no banco
                await _contaRepository.UpdateAsync(conta);

                return new InativarContaResponse
                {
                    Sucesso = true,
                    Mensagem = "Conta inativada com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new InativarContaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}