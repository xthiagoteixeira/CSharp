using Api_Transferencia.Domain.Enums;

namespace Api_Transferencia.Application.DTOs
{
    public class TransferenciaResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public TipoErroTransferencia? TipoErro { get; set; }
        public int? TransferenciaId { get; set; }
        public DateTime? DataTransferencia { get; set; }
    }

    public class TransferenciaRequest
    {
        public string ChaveIdempotencia { get; set; } = string.Empty;
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
    }

    public class TransferenciaHistoricoDto
    {
        public int Id { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
        public string ChaveIdempotencia { get; set; } = string.Empty;
    }
}