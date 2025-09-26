using Api_Transferencia.Domain.Entities;
using Api_Transferencia.Domain.ValueObjects;

namespace Api_Transferencia.Domain.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task CreateAsync(Idempotencia idempotencia);
        Task<Idempotencia?> GetByChaveAsync(ChaveIdempotencia chave);
        Task<bool> ExistsAsync(ChaveIdempotencia chave);
    }
}