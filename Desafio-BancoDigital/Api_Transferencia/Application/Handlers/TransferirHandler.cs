using MediatR;
using Api_Transferencia.Application.Commands;
using Api_Transferencia.Application.DTOs;
using Api_Transferencia.Domain.Entities;
using Api_Transferencia.Domain.Repositories;
using Api_Transferencia.Domain.Services;
using Api_Transferencia.Domain.ValueObjects;
using Api_Transferencia.Domain.Enums;
using System.Text.Json;

namespace Api_Transferencia.Application.Handlers
{
    public class TransferirHandler : IRequestHandler<TransferirCommand, TransferenciaResponse>
    {
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IContaCorrenteService _contaCorrenteService;
        private readonly IJwtService _jwtService;

        public TransferirHandler(
            ITransferenciaRepository transferenciaRepository,
            IIdempotenciaRepository idempotenciaRepository,
            IContaCorrenteService contaCorrenteService,
            IJwtService jwtService)
        {
            _transferenciaRepository = transferenciaRepository;
            _idempotenciaRepository = idempotenciaRepository;
            _contaCorrenteService = contaCorrenteService;
            _jwtService = jwtService;
        }

        public async Task<TransferenciaResponse> Handle(TransferirCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validar token JWT
                if (!_jwtService.ValidateToken(request.Token))
                {
                    return new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Token inválido ou expirado"
                    };
                }

                // 2. Verificar idempotência
                var chaveIdempotencia = new ChaveIdempotencia(request.ChaveIdempotencia);
                var resultadoExistente = await _idempotenciaRepository.GetByChaveAsync(chaveIdempotencia);
                if (resultadoExistente != null)
                {
                    return JsonSerializer.Deserialize<TransferenciaResponse>(resultadoExistente.Resultado) 
                        ?? new TransferenciaResponse { Sucesso = false, Mensagem = "Erro ao processar resposta anterior" };
                }

                // 3. Validações de negócio
                var validationResponse = await ValidarTransferencia(request);
                if (!validationResponse.Sucesso)
                {
                    await SalvarIdempotencia(chaveIdempotencia, request, validationResponse);
                    return validationResponse;
                }

                // 4. Realizar transferência
                var transferResponse = await RealizarTransferencia(request, chaveIdempotencia);
                
                // 5. Salvar resultado para idempotência
                await SalvarIdempotencia(chaveIdempotencia, request, transferResponse);
                
                return transferResponse;
            }
            catch (Exception ex)
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}",
                    TipoErro = TipoErroTransferencia.TRANSFER_FAILED
                };
            }
        }

        private async Task<TransferenciaResponse> ValidarTransferencia(TransferirCommand request)
        {
            // Validar valor
            if (request.Valor <= 0)
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Apenas valores positivos podem ser transferidos",
                    TipoErro = TipoErroTransferencia.INVALID_VALUE
                };
            }

            // Validar conta de origem existe
            if (!await _contaCorrenteService.ValidarContaExisteAsync(request.ContaOrigemId, request.Token))
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Conta de origem não encontrada",
                    TipoErro = TipoErroTransferencia.INVALID_ACCOUNT
                };
            }

            // Validar conta de origem ativa
            if (!await _contaCorrenteService.ValidarContaAtivaAsync(request.ContaOrigemId, request.Token))
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Conta de origem não está ativa",
                    TipoErro = TipoErroTransferencia.INACTIVE_ACCOUNT
                };
            }

            // Validar conta de destino existe
            if (!await _contaCorrenteService.ValidarContaExisteAsync(request.ContaDestinoId, request.Token))
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Conta de destino não encontrada",
                    TipoErro = TipoErroTransferencia.INVALID_ACCOUNT
                };
            }

            // Validar conta de destino ativa
            if (!await _contaCorrenteService.ValidarContaAtivaAsync(request.ContaDestinoId, request.Token))
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Conta de destino não está ativa",
                    TipoErro = TipoErroTransferencia.INACTIVE_ACCOUNT
                };
            }

            return new TransferenciaResponse { Sucesso = true };
        }

        private async Task<TransferenciaResponse> RealizarTransferencia(TransferirCommand request, ChaveIdempotencia chaveIdempotencia)
        {
            // 1. Realizar débito na conta de origem
            var debitoSucesso = await _contaCorrenteService.RealizarDebitoAsync(
                request.ContaOrigemId, request.Valor, request.ChaveIdempotencia, request.Token);

            if (!debitoSucesso)
            {
                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = "Falha ao realizar débito na conta de origem",
                    TipoErro = TipoErroTransferencia.INSUFFICIENT_FUNDS
                };
            }

            // 2. Realizar crédito na conta de destino
            var creditoSucesso = await _contaCorrenteService.RealizarCreditoAsync(
                request.ContaDestinoId, request.Valor, request.ChaveIdempotencia, request.Token);

            if (!creditoSucesso)
            {
                // Realizar estorno na conta de origem
                var estornoSucesso = await _contaCorrenteService.RealizarEstornoAsync(
                    request.ContaOrigemId, request.Valor, $"ESTORNO_{request.ChaveIdempotencia}", request.Token);

                return new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = estornoSucesso 
                        ? "Falha ao creditar conta de destino. Estorno realizado com sucesso."
                        : "Falha ao creditar conta de destino. ERRO CRÍTICO: Falha no estorno!",
                    TipoErro = estornoSucesso ? TipoErroTransferencia.TRANSFER_FAILED : TipoErroTransferencia.REVERSAL_FAILED
                };
            }

            // 3. Persistir transferência
            var transferencia = new Transferencia(
                request.ContaOrigemId,
                request.ContaDestinoId,
                new Valor(request.Valor),
                chaveIdempotencia
            );

            var transferenciaId = await _transferenciaRepository.CreateAsync(transferencia);

            return new TransferenciaResponse
            {
                Sucesso = true,
                Mensagem = "Transferência realizada com sucesso",
                TransferenciaId = transferenciaId,
                DataTransferencia = DateTime.UtcNow
            };
        }

        private async Task SalvarIdempotencia(ChaveIdempotencia chave, TransferirCommand request, TransferenciaResponse response)
        {
            var requisicaoJson = JsonSerializer.Serialize(request);
            var resultadoJson = JsonSerializer.Serialize(response);
            
            var idempotencia = new Idempotencia(chave, requisicaoJson, resultadoJson);
            await _idempotenciaRepository.CreateAsync(idempotencia);
        }
    }
}