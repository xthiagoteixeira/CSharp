using MediatR;
using Api_ContaCorrente.Application.Commands;
using Api_ContaCorrente.Domain.Repositories;
using Api_ContaCorrente.Domain.Entities;

namespace Api_ContaCorrente.Application.Handlers
{
    public class MovimentarContaHandler : IRequestHandler<MovimentarContaCommand, MovimentarContaResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public MovimentarContaHandler(IContaCorrenteRepository contaRepository, IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<MovimentarContaResponse> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar conta
                var conta = await _contaRepository.GetByIdAsync(request.ContaId);
                if (conta == null)
                {
                    return new MovimentarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Conta não encontrada",
                        Erros = new List<string> { "A conta especificada não existe" }
                    };
                }

                // Validar tipo de movimento
                if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                {
                    return new MovimentarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Tipo de movimento inválido",
                        Erros = new List<string> { "Tipo deve ser 'C' (Crédito) ou 'D' (Débito)" }
                    };
                }

                // Validar valor
                if (request.Valor <= 0)
                {
                    return new MovimentarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Valor inválido",
                        Erros = new List<string> { "O valor deve ser maior que zero" }
                    };
                }

                // Criar movimento
                var movimento = new Movimento(request.ContaId, request.TipoMovimento, request.Valor);

                // Executar movimentação na conta
                try
                {
                    if (request.TipoMovimento == "D")
                    {
                        conta.Debitar(request.Valor);
                    }
                    else
                    {
                        conta.Creditar(request.Valor);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return new MovimentarContaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Erro na movimentação",
                        Erros = new List<string> { ex.Message }
                    };
                }

                // Salvar movimento
                var movimentoId = await _movimentoRepository.CreateAsync(movimento);

                // Atualizar conta
                await _contaRepository.UpdateAsync(conta);

                return new MovimentarContaResponse
                {
                    MovimentoId = movimentoId,
                    SaldoAtual = conta.Saldo,
                    Sucesso = true,
                    Mensagem = $"Movimentação de {(request.TipoMovimento == "C" ? "crédito" : "débito")} realizada com sucesso"
                };
            }
            catch (Exception ex)
            {
                return new MovimentarContaResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}