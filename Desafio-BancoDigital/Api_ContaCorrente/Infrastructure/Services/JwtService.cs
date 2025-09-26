using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Api_ContaCorrente.Domain.Entities;
using Api_ContaCorrente.Domain.Services;

namespace Api_ContaCorrente.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtService(IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JWT");
            _secretKey = jwtSection.GetValue<string>("SecretKey") ?? "MinhaChaveSecretaSuperSegura123456789";
            _issuer = jwtSection.GetValue<string>("Issuer") ?? "Api_ContaCorrente";
            _audience = jwtSection.GetValue<string>("Audience") ?? "Api_ContaCorrente_Users";
            _expirationMinutes = jwtSection.GetValue<int>("ExpirationMinutes", 60);
            
            Console.WriteLine($"JwtService - SecretKey: {_secretKey}");
            Console.WriteLine($"JwtService - Issuer: {_issuer}");
            Console.WriteLine($"JwtService - Audience: {_audience}");
            Console.WriteLine($"JwtService - ExpirationMinutes: {_expirationMinutes}");
        }

        public string GenerateToken(ContaCorrente contaCorrente)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, contaCorrente.Id.ToString()),
                new Claim("ContaId", contaCorrente.Id.ToString()),
                new Claim("Numero", contaCorrente.Numero.ToString()),
                new Claim("CPF", contaCorrente.CPF.Numero),
                new Claim(ClaimTypes.Name, contaCorrente.CPF.FormatarCPF()),
                new Claim("Ativa", contaCorrente.Ativa.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int? GetContaIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var contaIdClaim = principal.FindFirst("ContaId");
                if (contaIdClaim != null && int.TryParse(contaIdClaim.Value, out int contaId))
                {
                    return contaId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}