using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Api_Transferencia.Application.Commands;
using Api_Transferencia.Application.DTOs;
using Api_Transferencia.Domain.Services;
using Api_Transferencia.Domain.Enums;

namespace Api_Transferencia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransferenciaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;

        public TransferenciaController(IMediator mediator, IJwtService jwtService)
        {
            _mediator = mediator;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Realizar transferência entre contas da mesma instituição
        /// </summary>
        /// <param name="request">Dados da transferência</param>
        /// <returns>Resultado da transferência</returns>
        [HttpPost("transferir")]
        public async Task<ActionResult<TransferenciaResponse>> Transferir([FromBody] TransferenciaRequest request)
        {
            try
            {
                // Extrair token do header
                var token = HttpContext.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last() ?? string.Empty;

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Token de autenticação não fornecido"
                    });
                }

                // Validar token
                if (!_jwtService.ValidateToken(token))
                {
                    return StatusCode(403, new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Token inválido ou expirado"
                    });
                }

                // Obter ID da conta do token
                var contaOrigemId = _jwtService.GetContaIdFromToken(token);
                if (!contaOrigemId.HasValue)
                {
                    return BadRequest(new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Não foi possível identificar a conta no token"
                    });
                }

                // Validações básicas
                if (string.IsNullOrWhiteSpace(request.ChaveIdempotencia))
                {
                    return BadRequest(new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Chave de idempotência é obrigatória"
                    });
                }

                if (request.ContaDestinoId <= 0)
                {
                    return BadRequest(new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "ID da conta de destino inválido",
                        TipoErro = TipoErroTransferencia.INVALID_ACCOUNT
                    });
                }

                if (request.Valor <= 0)
                {
                    return BadRequest(new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Apenas valores positivos podem ser transferidos",
                        TipoErro = TipoErroTransferencia.INVALID_VALUE
                    });
                }

                if (contaOrigemId.Value == request.ContaDestinoId)
                {
                    return BadRequest(new TransferenciaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Não é possível transferir para a mesma conta",
                        TipoErro = TipoErroTransferencia.INVALID_ACCOUNT
                    });
                }

                // Criar comando
                var command = new TransferirCommand(
                    request.ChaveIdempotencia,
                    request.ContaDestinoId,
                    request.Valor,
                    token,
                    contaOrigemId.Value
                );

                // Executar comando
                var response = await _mediator.Send(command);

                if (response.Sucesso)
                {
                    return NoContent(); // HTTP 204 conforme especificação
                }
                else
                {
                    return BadRequest(response); // HTTP 400 conforme especificação
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TransferenciaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno do servidor: {ex.Message}",
                    TipoErro = TipoErroTransferencia.TRANSFER_FAILED
                });
            }
        }

        /// <summary>
        /// Consultar histórico de transferências
        /// </summary>
        /// <param name="dataInicio">Data inicial do período (opcional)</param>
        /// <param name="dataFim">Data final do período (opcional)</param>
        /// <returns>Lista de transferências</returns>
        [HttpGet("historico")]
        public async Task<ActionResult<List<TransferenciaHistoricoDto>>> ConsultarHistorico(
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            try
            {
                // Extrair token do header
                var token = HttpContext.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last() ?? string.Empty;

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token de autenticação não fornecido");
                }

                // Validar token
                if (!_jwtService.ValidateToken(token))
                {
                    return StatusCode(403, "Token inválido ou expirado");
                }

                // Obter ID da conta do token
                var contaId = _jwtService.GetContaIdFromToken(token);
                if (!contaId.HasValue)
                {
                    return BadRequest("Não foi possível identificar a conta no token");
                }

                // Por enquanto retorna lista vazia - implementar query depois
                return Ok(new List<TransferenciaHistoricoDto>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}