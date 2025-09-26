using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Api_ContaCorrente.Application.Commands;
using Api_ContaCorrente.Application.Queries;

namespace Api_ContaCorrente.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovimentoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realizar movimentação na conta (débito ou crédito)
        /// </summary>
        /// <param name="command">Dados da movimentação</param>
        /// <returns>Resultado da movimentação</returns>
        [HttpPost("movimentar")]
        public async Task<ActionResult<MovimentarContaResponse>> MovimentarConta([FromBody] MovimentarContaCommand command)
        {
            var response = await _mediator.Send(command);
            
            if (response.Sucesso)
                return Ok(response);
            
            return BadRequest(response);
        }

        /// <summary>
        /// Consultar saldo da conta
        /// </summary>
        /// <param name="contaId">ID da conta</param>
        /// <returns>Saldo atual da conta</returns>
        [HttpGet("saldo/{contaId}")]
        public async Task<ActionResult<ConsultarSaldoResponse>> ConsultarSaldo(int contaId)
        {
            var query = new ConsultarSaldoQuery(contaId);
            var response = await _mediator.Send(query);
            
            if (response.Sucesso)
                return Ok(response);
            
            return BadRequest(response);
        }

        /// <summary>
        /// Consultar extrato da conta
        /// </summary>
        /// <param name="contaId">ID da conta</param>
        /// <param name="dataInicio">Data inicial do período (opcional)</param>
        /// <param name="dataFim">Data final do período (opcional)</param>
        /// <returns>Extrato da conta</returns>
        [HttpGet("extrato/{contaId}")]
        public async Task<ActionResult<ConsultarExtratoResponse>> ConsultarExtrato(
            int contaId, 
            [FromQuery] DateTime? dataInicio = null, 
            [FromQuery] DateTime? dataFim = null)
        {
            var query = new ConsultarExtratoQuery(contaId, dataInicio, dataFim);
            var response = await _mediator.Send(query);
            
            if (response.Sucesso)
                return Ok(response);
            
            return BadRequest(response);
        }
    }
}