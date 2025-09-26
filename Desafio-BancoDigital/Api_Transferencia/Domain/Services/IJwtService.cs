namespace Api_Transferencia.Domain.Services
{
    public interface IJwtService
    {
        bool ValidateToken(string token);
        int? GetContaIdFromToken(string token);
        string GetClaimFromToken(string token, string claimType);
    }
}