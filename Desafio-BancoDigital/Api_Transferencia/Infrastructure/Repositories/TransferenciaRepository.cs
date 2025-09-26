using Dapper;
using Api_Transferencia.Domain.Entities;
using Api_Transferencia.Domain.Repositories;
using Api_Transferencia.Domain.ValueObjects;
using Api_Transferencia.Infrastructure.Data;

namespace Api_Transferencia.Infrastructure.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TransferenciaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Transferencia transferencia)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                INSERT INTO TRANSFERENCIA (IDTRANSFERENCIA, IDCONTACORRENTE_ORIGEM, IDCONTACORRENTE_DESTINO, DATAMOVIMENTO, VALOR)
                VALUES (TRANSFERENCIA_SEQ.NEXTVAL, :contaOrigemId, :contaDestinoId, :dataMovimento, :valor)";

            await connection.ExecuteAsync(sql, new
            {
                contaOrigemId = transferencia.ContaOrigemId,
                contaDestinoId = transferencia.ContaDestinoId,
                dataMovimento = transferencia.DataMovimento,
                valor = transferencia.Valor.Quantia
            });

            // Buscar o ID gerado
            var idSql = "SELECT TRANSFERENCIA_SEQ.CURRVAL FROM DUAL";
            var id = await connection.QuerySingleAsync<int>(idSql);
            
            return id;
        }

        public async Task<Transferencia?> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDTRANSFERENCIA, IDCONTACORRENTE_ORIGEM, IDCONTACORRENTE_DESTINO, DATAMOVIMENTO, VALOR
                FROM TRANSFERENCIA 
                WHERE IDTRANSFERENCIA = :id";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { id });

            if (result == null)
                return null;

            return new Transferencia(
                Convert.ToInt32(result.IDTRANSFERENCIA),
                Convert.ToInt32(result.IDCONTACORRENTE_ORIGEM),
                Convert.ToInt32(result.IDCONTACORRENTE_DESTINO),
                Convert.ToDateTime(result.DATAMOVIMENTO),
                new Valor(Convert.ToDecimal(result.VALOR)),
                new ChaveIdempotencia($"TRANSFER_{result.IDTRANSFERENCIA}")
            );
        }

        public async Task<IEnumerable<Transferencia>> GetByContaOrigemAsync(int contaOrigemId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDTRANSFERENCIA, IDCONTACORRENTE_ORIGEM, IDCONTACORRENTE_DESTINO, DATAMOVIMENTO, VALOR
                FROM TRANSFERENCIA 
                WHERE IDCONTACORRENTE_ORIGEM = :contaOrigemId
                ORDER BY DATAMOVIMENTO DESC";

            var results = await connection.QueryAsync(sql, new { contaOrigemId });

            return results.Select(result => new Transferencia(
                Convert.ToInt32(result.IDTRANSFERENCIA),
                Convert.ToInt32(result.IDCONTACORRENTE_ORIGEM),
                Convert.ToInt32(result.IDCONTACORRENTE_DESTINO),
                Convert.ToDateTime(result.DATAMOVIMENTO),
                new Valor(Convert.ToDecimal(result.VALOR)),
                new ChaveIdempotencia($"TRANSFER_{result.IDTRANSFERENCIA}")
            ));
        }

        public async Task<IEnumerable<Transferencia>> GetByContaDestinoAsync(int contaDestinoId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDTRANSFERENCIA, IDCONTACORRENTE_ORIGEM, IDCONTACORRENTE_DESTINO, DATAMOVIMENTO, VALOR
                FROM TRANSFERENCIA 
                WHERE IDCONTACORRENTE_DESTINO = :contaDestinoId
                ORDER BY DATAMOVIMENTO DESC";

            var results = await connection.QueryAsync(sql, new { contaDestinoId });

            return results.Select(result => new Transferencia(
                Convert.ToInt32(result.IDTRANSFERENCIA),
                Convert.ToInt32(result.IDCONTACORRENTE_ORIGEM),
                Convert.ToInt32(result.IDCONTACORRENTE_DESTINO),
                Convert.ToDateTime(result.DATAMOVIMENTO),
                new Valor(Convert.ToDecimal(result.VALOR)),
                new ChaveIdempotencia($"TRANSFER_{result.IDTRANSFERENCIA}")
            ));
        }

        public async Task<IEnumerable<Transferencia>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDTRANSFERENCIA, IDCONTACORRENTE_ORIGEM, IDCONTACORRENTE_DESTINO, DATAMOVIMENTO, VALOR
                FROM TRANSFERENCIA 
                WHERE DATAMOVIMENTO BETWEEN :dataInicio AND :dataFim
                ORDER BY DATAMOVIMENTO DESC";

            var results = await connection.QueryAsync(sql, new { dataInicio, dataFim });

            return results.Select(result => new Transferencia(
                Convert.ToInt32(result.IDTRANSFERENCIA),
                Convert.ToInt32(result.IDCONTACORRENTE_ORIGEM),
                Convert.ToInt32(result.IDCONTACORRENTE_DESTINO),
                Convert.ToDateTime(result.DATAMOVIMENTO),
                new Valor(Convert.ToDecimal(result.VALOR)),
                new ChaveIdempotencia($"TRANSFER_{result.IDTRANSFERENCIA}")
            ));
        }

        public async Task<bool> ExistsByChaveIdempotenciaAsync(ChaveIdempotencia chave)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT COUNT(1) FROM IDEMPOTENCIA WHERE CHAVE_IDEMPOTENCIA = :chave";
            var count = await connection.QuerySingleAsync<int>(sql, new { chave = chave.Valor });
            
            return count > 0;
        }
    }
}