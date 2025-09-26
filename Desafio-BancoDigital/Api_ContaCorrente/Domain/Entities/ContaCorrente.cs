using Api_ContaCorrente.Domain.ValueObjects;

namespace Api_ContaCorrente.Domain.Entities
{
    public class ContaCorrente
    {
        public int Id { get; private set; }
        public int Numero { get; private set; }
        public CPF CPF { get; private set; }
        public string SenhaHash { get; private set; }
        public decimal Saldo { get; private set; }
        public bool Ativa { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        // Construtor para criação de nova conta
        public ContaCorrente(CPF cpf, string senhaHash)
        {
            CPF = cpf ?? throw new ArgumentNullException(nameof(cpf));
            SenhaHash = senhaHash ?? throw new ArgumentNullException(nameof(senhaHash));
            Saldo = 0;
            Ativa = true;
            DataCriacao = DateTime.UtcNow;
        }

        // Construtor para hidratação do banco de dados
        public ContaCorrente(int id, int numero, CPF cpf, string senhaHash, decimal saldo, bool ativa, DateTime dataCriacao, DateTime? dataAtualizacao)
        {
            Id = id;
            Numero = numero;
            CPF = cpf;
            SenhaHash = senhaHash;
            Saldo = saldo;
            Ativa = ativa;
            DataCriacao = dataCriacao;
            DataAtualizacao = dataAtualizacao;
        }

        public void Debitar(decimal valor)
        {
            if (!Ativa)
                throw new InvalidOperationException("Conta inativa não pode realizar movimentações");

            if (valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            if (Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente");

            Saldo -= valor;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Creditar(decimal valor)
        {
            if (!Ativa)
                throw new InvalidOperationException("Conta inativa não pode realizar movimentações");

            if (valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            Saldo += valor;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Inativar()
        {
            if (!Ativa)
                throw new InvalidOperationException("Conta já está inativa");

            Ativa = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Ativar()
        {
            if (Ativa)
                throw new InvalidOperationException("Conta já está ativa");

            Ativa = true;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void AlterarSenha(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
                throw new ArgumentException("Nova senha não pode ser vazia");

            SenhaHash = novaSenhaHash;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void SetNumero(int numero)
        {
            if (numero <= 0)
                throw new ArgumentException("Número da conta deve ser maior que zero");

            Numero = numero;
        }

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id deve ser maior que zero");

            Id = id;
        }
    }
}