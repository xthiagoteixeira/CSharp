namespace Api_Transferencia.Domain.ValueObjects
{
    public class Valor
    {
        public decimal Quantia { get; private set; }

        public Valor(decimal quantia)
        {
            if (quantia <= 0)
                throw new ArgumentException("Valor deve ser maior que zero", nameof(quantia));

            Quantia = quantia;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Valor other)
                return Quantia == other.Quantia;
            return false;
        }

        public override int GetHashCode()
        {
            return Quantia.GetHashCode();
        }

        public override string ToString()
        {
            return Quantia.ToString("C2");
        }

        public static implicit operator decimal(Valor valor)
        {
            return valor.Quantia;
        }

        public static implicit operator Valor(decimal quantia)
        {
            return new Valor(quantia);
        }
    }
}