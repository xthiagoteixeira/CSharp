namespace Api_Transferencia.Domain.Entities
{
    public class Idempotencia
    {
        public string Chave { get; private set; }
        public string Requisicao { get; private set; }
        public string Resultado { get; private set; }
        public DateTime DataCriacao { get; private set; }

        public Idempotencia(string chave, string requisicao, string resultado)
        {
            if (string.IsNullOrWhiteSpace(chave))
                throw new ArgumentException("Chave de idempotência não pode ser vazia", nameof(chave));
            
            if (string.IsNullOrWhiteSpace(requisicao))
                throw new ArgumentException("Requisição não pode ser vazia", nameof(requisicao));
            
            if (string.IsNullOrWhiteSpace(resultado))
                throw new ArgumentException("Resultado não pode ser vazio", nameof(resultado));

            Chave = chave;
            Requisicao = requisicao;
            Resultado = resultado;
            DataCriacao = DateTime.UtcNow;
        }
    }
}