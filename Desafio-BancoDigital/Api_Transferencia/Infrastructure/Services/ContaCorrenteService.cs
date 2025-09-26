using Api_Transferencia.Domain.Services;
using System.Text.Json;

namespace Api_Transferencia.Infrastructure.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly HttpClient _httpClient;

        public ContaCorrenteService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // BaseAddress já está configurado no Program.cs
        }

        public async Task<bool> ValidarContaExisteAsync(int contaId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var url = $"/api/Conta/{contaId}";
                Console.WriteLine($"[DEBUG] Chamando: {_httpClient.BaseAddress}{url}");
                
                var response = await _httpClient.GetAsync(url);
                
                Console.WriteLine($"[DEBUG] StatusCode: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[DEBUG] Response Content: {content}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidarContaAtivaAsync(int contaId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"/api/Conta/{contaId}");
                
                if (!response.IsSuccessStatusCode)
                    return false;

                var content = await response.Content.ReadAsStringAsync();
                var conta = JsonSerializer.Deserialize<dynamic>(content);
                
                // Assumindo que a resposta tem um campo "ativa" ou similar
                return true; // Simplificado por enquanto
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RealizarDebitoAsync(int contaId, decimal valor, string chaveIdempotencia, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    contaId = contaId,
                    tipoMovimento = "D", // Débito
                    valor = valor,
                    chaveIdempotencia = chaveIdempotencia
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Movimento/movimentar", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] RealizarDebitoAsync Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RealizarCreditoAsync(int contaId, decimal valor, string chaveIdempotencia, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    contaId = contaId,
                    tipoMovimento = "C", // Crédito
                    valor = valor,
                    chaveIdempotencia = chaveIdempotencia
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Movimento/movimentar", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] RealizarCreditoAsync Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RealizarEstornoAsync(int contaId, decimal valor, string chaveIdempotencia, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    contaId = contaId,
                    tipoMovimento = "C", // Crédito para estornar débito anterior
                    valor = valor,
                    chaveIdempotencia = chaveIdempotencia
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Movimento/movimentar", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}