using Dapper;
using Api_ContaCorrente.Domain.Entities;
using Api_ContaCorrente.Domain.Repositories;
using Api_ContaCorrente.Domain.ValueObjects;
using Api_ContaCorrente.Infrastructure.Data;

namespace Api_ContaCorrente.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ContaCorrenteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ContaCorrente?> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDCONTACORRENTE, NUMERO, CPF, SENHA, SALDO, ATIVO
                FROM CONTACORRENTE 
                WHERE IDCONTACORRENTE = :id";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { id });

            if (result == null)
                return null;

            var cpf = new CPF(result.CPF.ToString());
            return new ContaCorrente(
                Convert.ToInt32(result.IDCONTACORRENTE),
                Convert.ToInt32(result.NUMERO),
                cpf,
                result.SENHA?.ToString() ?? string.Empty,
                Convert.ToDecimal(result.SALDO),
                Convert.ToInt32(result.ATIVO) == 1,
                DateTime.Now,
                null
            );
        }

        public async Task<ContaCorrente?> GetByCPFAsync(CPF cpf)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDCONTACORRENTE, NUMERO, CPF, SENHA, SALDO, ATIVO
                FROM CONTACORRENTE 
                WHERE CPF = :cpf";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { cpf = Convert.ToInt64(cpf.Numero) });

            if (result == null)
                return null;

            return new ContaCorrente(
                Convert.ToInt32(result.IDCONTACORRENTE),
                Convert.ToInt32(result.NUMERO),
                cpf,
                result.SENHA?.ToString() ?? string.Empty,
                Convert.ToDecimal(result.SALDO),
                Convert.ToInt32(result.ATIVO) == 1,
                DateTime.Now,
                null
            );
        }

        public async Task<ContaCorrente?> GetByNumeroAsync(int numero)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT IDCONTACORRENTE, NUMERO, CPF, SENHA, SALDO, ATIVO
                FROM CONTACORRENTE 
                WHERE NUMERO = :numero";

            var result = await connection.QueryFirstOrDefaultAsync(sql, new { numero = numero.ToString() });

            if (result == null)
                return null;

            var cpf = new CPF(result.CPF.ToString());
            return new ContaCorrente(
                Convert.ToInt32(result.IDCONTACORRENTE),
                Convert.ToInt32(result.NUMERO),
                cpf,
                result.SENHA?.ToString() ?? string.Empty,
                Convert.ToDecimal(result.SALDO),
                Convert.ToInt32(result.ATIVO) == 1,
                DateTime.Now,
                null
            );
        }

        public async Task<int> CreateAsync(ContaCorrente contaCorrente)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            try
            {
                // Primeiro, verificar se o CPF já existe
                var cpfExistsSql = "SELECT COUNT(1) FROM CONTACORRENTE WHERE CPF = :cpf";
                var cpfExists = await connection.QuerySingleAsync<int>(cpfExistsSql, new { cpf = Convert.ToInt64(contaCorrente.CPF.Numero) });
                
                if (cpfExists > 0)
                {
                    throw new InvalidOperationException($"CPF {contaCorrente.CPF.Numero} já está cadastrado");
                }

                // Verificar se o NUMERO já existe
                var numeroExistsSql = "SELECT COUNT(1) FROM CONTACORRENTE WHERE NUMERO = :numero";
                var numeroExists = await connection.QuerySingleAsync<int>(numeroExistsSql, new { numero = contaCorrente.Numero.ToString() });
                
                if (numeroExists > 0)
                {
                    throw new InvalidOperationException($"Número da conta {contaCorrente.Numero} já existe");
                }

                // Inserir o registro
                var sql = @"
                    INSERT INTO CONTACORRENTE (IDCONTACORRENTE, NUMERO, CPF, SENHA, SALDO, ATIVO)
                    VALUES (CONTACORRENTE_SEQ.NEXTVAL, :numero, :cpf, :senha, :saldo, :ativo)";

                await connection.ExecuteAsync(sql, new
                {
                    numero = contaCorrente.Numero.ToString(),
                    cpf = Convert.ToInt64(contaCorrente.CPF.Numero),
                    senha = contaCorrente.SenhaHash,
                    saldo = contaCorrente.Saldo,
                    ativo = contaCorrente.Ativa ? 1 : 0
                });

                // Buscar o ID gerado
                var idSql = "SELECT CONTACORRENTE_SEQ.CURRVAL FROM DUAL";
                var id = await connection.QuerySingleAsync<int>(idSql);
                
                return id;
            }
            catch (Exception ex) when (ex.Message.Contains("ORA-00001"))
            {
                // Erro de constraint única - fornecer mensagem mais clara
                throw new InvalidOperationException("Já existe uma conta com este CPF ou número. Verifique os dados informados.", ex);
            }
        }

        public async Task UpdateAsync(ContaCorrente contaCorrente)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE CONTACORRENTE 
                SET SALDO = :saldo, 
                    ATIVO = :ativo,
                    SENHA = :senha
                WHERE IDCONTACORRENTE = :id";

            await connection.ExecuteAsync(sql, new
            {
                id = contaCorrente.Id,
                saldo = contaCorrente.Saldo,
                ativo = contaCorrente.Ativa ? 1 : 0,
                senha = contaCorrente.SenhaHash
            });
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "DELETE FROM CONTACORRENTE WHERE IDCONTACORRENTE = :id";
            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task<bool> ExistsByCPFAsync(CPF cpf)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT COUNT(1) FROM CONTACORRENTE WHERE CPF = :cpf";
            var count = await connection.QuerySingleAsync<int>(sql, new { cpf = Convert.ToInt64(cpf.Numero) });
            
            return count > 0;
        }

        public async Task<bool> ExistsByNumeroAsync(int numero)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT COUNT(1) FROM CONTACORRENTE WHERE NUMERO = :numero";
            var count = await connection.QuerySingleAsync<int>(sql, new { numero = numero.ToString() });
            
            return count > 0;
        }

        public async Task<int> GetNextNumeroAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT NVL(MAX(TO_NUMBER(NUMERO)), 0) + 1 FROM CONTACORRENTE";
            var nextNumero = await connection.QuerySingleAsync<int>(sql);
            
            return nextNumero;
        }
    }
}