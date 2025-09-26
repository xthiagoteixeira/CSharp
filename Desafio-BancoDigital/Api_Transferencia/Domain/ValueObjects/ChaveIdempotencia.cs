namespace Api_Transferencia.Domain.ValueObjects
{
    public class ChaveIdempotencia
    {
        public string Valor { get; private set; }

        public ChaveIdempotencia(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Chave de idempotência não pode ser vazia", nameof(valor));

            if (valor.Length > 100)
                throw new ArgumentException("Chave de idempotência não pode ter mais de 100 caracteres", nameof(valor));

            Valor = valor;
        }

        public override bool Equals(object obj)
        {
            if (obj is ChaveIdempotencia other)
                return Valor == other.Valor;
            return false;
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

        public override string ToString()
        {
            return Valor;
        }

        public static implicit operator string(ChaveIdempotencia chave)
        {
            return chave.Valor;
        }

        public static implicit operator ChaveIdempotencia(string valor)
        {
            return new ChaveIdempotencia(valor);
        }
    }
}