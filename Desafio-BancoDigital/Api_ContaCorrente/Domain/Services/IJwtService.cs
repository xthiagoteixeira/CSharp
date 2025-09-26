using Api_ContaCorrente.Domain.Entities;

namespace Api_ContaCorrente.Domain.Services
{
    public interface IJwtService
    {
        string GenerateToken(ContaCorrente contaCorrente);
        bool ValidateToken(string token);
        int? GetContaIdFromToken(string token);
    }
}