using MediatR;
using Api_ContaCorrente.Application.Queries;
using Api_ContaCorrente.Domain.Repositories;

namespace Api_ContaCorrente.Application.Handlers
{
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, ConsultarSaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;

        public ConsultarSaldoHandler(IContaCorrenteRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<ConsultarSaldoResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar conta
                var conta = await _contaRepository.GetByIdAsync(request.ContaId);
                if (conta == null)
                {
                    return new ConsultarSaldoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta não encontrada",
                        Erros = new List<string> { "A conta especificada não existe" }
                    };
                }

                return new ConsultarSaldoResponse
                {
                    ContaId = conta.Id,
                    Numero = conta.Numero,
                    CPF = conta.CPF.FormatarCPF(),
                    Saldo = conta.Saldo,
                    ContaAtiva = conta.Ativa,
                    DataConsulta = DateTime.UtcNow,
                    Sucesso = true,
                    Mensagem = "Consulta realizada com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new ConsultarSaldoResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}