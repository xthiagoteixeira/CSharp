using MediatR;
using Api_Transferencia.Application.DTOs;

namespace Api_Transferencia.Application.Commands
{
    public class TransferirCommand : IRequest<TransferenciaResponse>
    {
        public string ChaveIdempotencia { get; set; } = string.Empty;
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public string Token { get; set; } = string.Empty;
        public int ContaOrigemId { get; set; }

        public TransferirCommand(string chaveIdempotencia, int contaDestinoId, decimal valor, string token, int contaOrigemId)
        {
            ChaveIdempotencia = chaveIdempotencia;
            ContaDestinoId = contaDestinoId;
            Valor = valor;
            Token = token;
            ContaOrigemId = contaOrigemId;
        }
    }
}