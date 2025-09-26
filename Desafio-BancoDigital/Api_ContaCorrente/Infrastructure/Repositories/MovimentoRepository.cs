using Dapper;
using Api_ContaCorrente.Domain.Entities;
using Api_ContaCorrente.Domain.Repositories;
using Api_ContaCorrente.Infrastructure.Data;

namespace Api_ContaCorrente.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MovimentoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Movimento?> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDMOVIMENTO, IDCONTACORRENTE, TIPOMOVIMENTO, VALOR, DATAMOVIMENTO
                FROM MOVIMENTO 
                WHERE IDMOVIMENTO = :id";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { id });

            if (result == null)
                return null;

            return new Movimento(
                result.IDMOVIMENTO,
                result.IDCONTACORRENTE,
                result.TIPOMOVIMENTO,
                result.VALOR,
                result.DATAMOVIMENTO
            );
        }

        public async Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(int idContaCorrente)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDMOVIMENTO, IDCONTACORRENTE, TIPOMOVIMENTO, VALOR, DATAMOVIMENTO
                FROM MOVIMENTO 
                WHERE IDCONTACORRENTE = :idContaCorrente
                ORDER BY DATAMOVIMENTO DESC";

            var results = await connection.QueryAsync(sql, new { idContaCorrente });

            return results.Select(r => new Movimento(
                r.IDMOVIMENTO,
                r.IDCONTACORRENTE,
                r.TIPOMOVIMENTO,
                r.VALOR,
                r.DATAMOVIMENTO
            )).ToList();
        }

        public async Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(int idContaCorrente, DateTime dataInicio, DateTime dataFim)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDMOVIMENTO, IDCONTACORRENTE, TIPOMOVIMENTO, VALOR, DATAMOVIMENTO
                FROM MOVIMENTO 
                WHERE IDCONTACORRENTE = :idContaCorrente
                  AND DATAMOVIMENTO >= :dataInicio
                  AND DATAMOVIMENTO <= :dataFim
                ORDER BY DATAMOVIMENTO DESC";

            var results = await connection.QueryAsync(sql, new 
            { 
                idContaCorrente, 
                dataInicio, 
                dataFim 
            });

            return results.Select(r => new Movimento(
                r.IDMOVIMENTO,
                r.IDCONTACORRENTE,
                r.TIPOMOVIMENTO,
                r.VALOR,
                r.DATAMOVIMENTO
            )).ToList();
        }

        public async Task<int> CreateAsync(Movimento movimento)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                INSERT INTO MOVIMENTO (IDMOVIMENTO, IDCONTACORRENTE, TIPOMOVIMENTO, VALOR, DATAMOVIMENTO)
                VALUES (MOVIMENTO_SEQ.NEXTVAL, :idContaCorrente, :tipoMovimento, :valor, :dataMovimento)";

            await connection.ExecuteAsync(sql, new
            {
                idContaCorrente = movimento.IdContaCorrente,
                tipoMovimento = movimento.TipoMovimento,
                valor = movimento.Valor,
                dataMovimento = movimento.DataMovimento
            });

            // Buscar o ID gerado
            var idSql = "SELECT MOVIMENTO_SEQ.CURRVAL FROM DUAL";
            var id = await connection.QuerySingleAsync<int>(idSql);
            
            return id;
        }

        public async Task UpdateAsync(Movimento movimento)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE MOVIMENTO 
                SET TIPOMOVIMENTO = :tipoMovimento, 
                    VALOR = :valor, 
                    DATAMOVIMENTO = :dataMovimento
                WHERE IDMOVIMENTO = :id";

            await connection.ExecuteAsync(sql, new
            {
                id = movimento.Id,
                tipoMovimento = movimento.TipoMovimento,
                valor = movimento.Valor,
                dataMovimento = movimento.DataMovimento
            });
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "DELETE FROM MOVIMENTO WHERE IDMOVIMENTO = :id";
            await connection.ExecuteAsync(sql, new { id });
        }
    }
}