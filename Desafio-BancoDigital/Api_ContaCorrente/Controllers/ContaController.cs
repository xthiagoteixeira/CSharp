using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Api_ContaCorrente.Application.Commands;
using Api_ContaCorrente.Domain.Repositories;

namespace Api_ContaCorrente.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IContaCorrenteRepository _contaRepository;

        public ContaController(IMediator mediator, IContaCorrenteRepository contaRepository)
        {
            _mediator = mediator;
            _contaRepository = contaRepository;
        }

        /// <summary>
        /// Cadastrar uma nova conta corrente
        /// </summary>
        /// <param name="command">Dados para cadastro da conta</param>
        /// <returns>Resultado do cadastro</returns>
        [HttpPost("cadastrar")]
        public async Task<ActionResult<CadastrarContaResponse>> CadastrarConta([FromBody] CadastrarContaCommand command)
        {
            var response = await _mediator.Send(command);
            
            if (response.Sucesso)
                return Ok(response);
            
            return BadRequest(response);
        }

        /// <summary>
        /// Login na conta corrente
        /// </summary>
        /// <param name="command">Credenciais de login</param>
        /// <returns>Token JWT se login bem-sucedido</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            
            if (response.Sucesso)
                return Ok(response);
            
            return Unauthorized(response);
        }

        /// <summary>
        /// Consultar dados de uma conta corrente
        /// </summary>
        /// <param name="contaId">ID da conta</param>
        /// <returns>Dados da conta</returns>
        [HttpGet("{contaId}")]
        [Authorize]
        public async Task<ActionResult> ConsultarConta(int contaId)
        {
            try
            {
                var conta = await _contaRepository.GetByIdAsync(contaId);
                
                if (conta == null)
                {
                    return NotFound(new { 
                        mensagem = "Conta não encontrada" 
                    });
                }

                return Ok(new { 
                    id = conta.Id,
                    numero = conta.Numero,
                    ativa = conta.Ativa,
                    saldo = conta.Saldo,
                    mensagem = "Conta encontrada" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    mensagem = $"Erro interno: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Inativar conta corrente
        /// </summary>
        /// <param name="contaId">ID da conta a ser inativada</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("inativar/{contaId}")]
        public async Task<ActionResult<InativarContaResponse>> InativarConta(int contaId)
        {
            var command = new InativarContaCommand(contaId);
            var response = await _mediator.Send(command);
            
            if (response.Sucesso)
                return Ok(response);
            
            return BadRequest(response);
        }
    }
}