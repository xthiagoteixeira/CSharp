namespace Api_Transferencia.Domain.Services
{
    public interface IContaCorrenteService
    {
        Task<bool> ValidarContaExisteAsync(int contaId, string token);
        Task<bool> ValidarContaAtivaAsync(int contaId, string token);
        Task<bool> RealizarDebitoAsync(int contaId, decimal valor, string chaveIdempotencia, string token);
        Task<bool> RealizarCreditoAsync(int contaId, decimal valor, string chaveIdempotencia, string token);
        Task<bool> RealizarEstornoAsync(int contaId, decimal valor, string chaveIdempotencia, string token);
    }
}