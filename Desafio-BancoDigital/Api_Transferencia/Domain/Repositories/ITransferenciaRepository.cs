using Api_Transferencia.Domain.Entities;
using Api_Transferencia.Domain.ValueObjects;

namespace Api_Transferencia.Domain.Repositories
{
    public interface ITransferenciaRepository
    {
        Task<int> CreateAsync(Transferencia transferencia);
        Task<Transferencia?> GetByIdAsync(int id);
        Task<IEnumerable<Transferencia>> GetByContaOrigemAsync(int contaOrigemId);
        Task<IEnumerable<Transferencia>> GetByContaDestinoAsync(int contaDestinoId);
        Task<IEnumerable<Transferencia>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<bool> ExistsByChaveIdempotenciaAsync(ChaveIdempotencia chave);
    }
}