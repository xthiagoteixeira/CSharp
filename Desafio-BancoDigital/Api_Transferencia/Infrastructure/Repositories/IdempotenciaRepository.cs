using Dapper;
using Api_Transferencia.Domain.Entities;
using Api_Transferencia.Domain.Repositories;
using Api_Transferencia.Domain.ValueObjects;
using Api_Transferencia.Infrastructure.Data;

namespace Api_Transferencia.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IdempotenciaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(Idempotencia idempotencia)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                INSERT INTO IDEMPOTENCIA (CHAVE_IDEMPOTENCIA, REQUISICAO, RESULTADO, DATA_CRIACAO)
                VALUES (:chave, :requisicao, :resultado, :dataCriacao)";

            await connection.ExecuteAsync(sql, new
            {
                chave = idempotencia.Chave,
                requisicao = idempotencia.Requisicao,
                resultado = idempotencia.Resultado,
                dataCriacao = idempotencia.DataCriacao
            });
        }

        public async Task<Idempotencia?> GetByChaveAsync(ChaveIdempotencia chave)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT CHAVE_IDEMPOTENCIA, REQUISICAO, RESULTADO, DATA_CRIACAO
                FROM IDEMPOTENCIA 
                WHERE CHAVE_IDEMPOTENCIA = :chave";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { chave = chave.Valor });

            if (result == null)
                return null;

            return new Idempotencia(
                result.CHAVE_IDEMPOTENCIA?.ToString() ?? string.Empty,
                result.REQUISICAO?.ToString() ?? string.Empty,
                result.RESULTADO?.ToString() ?? string.Empty
            );
        }

        public async Task<bool> ExistsAsync(ChaveIdempotencia chave)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT COUNT(1) FROM IDEMPOTENCIA WHERE CHAVE_IDEMPOTENCIA = :chave";
            var count = await connection.QuerySingleAsync<int>(sql, new { chave = chave.Valor });
            
            return count > 0;
        }
    }
}