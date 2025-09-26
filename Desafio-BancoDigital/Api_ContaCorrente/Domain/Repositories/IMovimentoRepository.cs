using Api_ContaCorrente.Domain.Entities;

namespace Api_ContaCorrente.Domain.Repositories
{
    public interface IMovimentoRepository
    {
        Task<Movimento?> GetByIdAsync(int id);
        Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(int idContaCorrente);
        Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(int idContaCorrente, DateTime dataInicio, DateTime dataFim);
        Task<int> CreateAsync(Movimento movimento);
        Task UpdateAsync(Movimento movimento);
        Task DeleteAsync(int id);
    }
}