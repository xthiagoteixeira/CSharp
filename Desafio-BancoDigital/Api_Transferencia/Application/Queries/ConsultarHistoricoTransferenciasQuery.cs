using MediatR;
using Api_Transferencia.Application.DTOs;

namespace Api_Transferencia.Application.Queries
{
    public class ConsultarHistoricoTransferenciasQuery : IRequest<List<TransferenciaHistoricoDto>>
    {
        public int ContaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Token { get; set; } = string.Empty;

        public ConsultarHistoricoTransferenciasQuery(int contaId, string token, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            ContaId = contaId;
            Token = token;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }
    }
}