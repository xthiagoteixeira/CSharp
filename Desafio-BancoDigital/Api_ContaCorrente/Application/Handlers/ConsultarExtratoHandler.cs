using MediatR;
using Api_ContaCorrente.Application.Queries;
using Api_ContaCorrente.Domain.Repositories;

namespace Api_ContaCorrente.Application.Handlers
{
    public class ConsultarExtratoHandler : IRequestHandler<ConsultarExtratoQuery, ConsultarExtratoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ConsultarExtratoHandler(IContaCorrenteRepository contaRepository, IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<ConsultarExtratoResponse> Handle(ConsultarExtratoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar conta
                var conta = await _contaRepository.GetByIdAsync(request.ContaId);
                if (conta == null)
                {
                    return new ConsultarExtratoResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta não encontrada",
                        Erros = new List<string> { "A conta especificada não existe" }
                    };
                }

                // Buscar movimentos
                var movimentos = request.DataInicio.HasValue && request.DataFim.HasValue
                    ? await _movimentoRepository.GetByContaCorrenteIdAsync(request.ContaId, request.DataInicio.Value, request.DataFim.Value)
                    : await _movimentoRepository.GetByContaCorrenteIdAsync(request.ContaId);

                var movimentosDto = movimentos.Select(m => new MovimentoDto
                {
                    Id = m.Id,
                    TipoMovimento = m.TipoMovimento,
                    DescricaoTipo = m.TipoMovimento == "C" ? "Crédito" : "Débito",
                    Valor = m.Valor,
                    DataMovimento = m.DataMovimento
                }).OrderByDescending(m => m.DataMovimento).ToList();

                return new ConsultarExtratoResponse
                {
                    ContaId = conta.Id,
                    Numero = conta.Numero,
                    CPF = conta.CPF.FormatarCPF(),
                    SaldoAtual = conta.Saldo,
                    Movimentos = movimentosDto,
                    Sucesso = true,
                    Mensagem = "Extrato consultado com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new ConsultarExtratoResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}