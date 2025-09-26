using Api_ContaCorrente.Domain.Entities;
using Api_ContaCorrente.Domain.ValueObjects;

namespace Api_ContaCorrente.Domain.Repositories
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente?> GetByIdAsync(int id);
        Task<ContaCorrente?> GetByCPFAsync(CPF cpf);
        Task<ContaCorrente?> GetByNumeroAsync(int numero);
        Task<int> CreateAsync(ContaCorrente contaCorrente);
        Task UpdateAsync(ContaCorrente contaCorrente);
        Task DeleteAsync(int id);
        Task<bool> ExistsByCPFAsync(CPF cpf);
        Task<bool> ExistsByNumeroAsync(int numero);
        Task<int> GetNextNumeroAsync();
    }
}