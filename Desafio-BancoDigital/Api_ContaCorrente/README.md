# API Conta Corrente - Banco Digital

Esta é uma API para gerenciamento de contas correntes desenvolvida com ASP.NET Core 8.0, seguindo os padrões **DDD (Domain-Driven Design)** e **CQRS (Command Query Responsibility Segregation)**.

## 🏗️ Arquitetura

O projeto está organizado seguindo os princípios do DDD:

```
Api_ContaCorrente/
├── Domain/                 # Camada de Domínio
│   ├── Entities/          # Entidades de negócio
│   ├── ValueObjects/      # Objetos de valor
│   ├── Repositories/      # Interfaces dos repositórios
│   └── Services/          # Interfaces dos serviços de domínio
├── Application/           # Camada de Aplicação
│   ├── Commands/          # Comandos (operações que alteram estado)
│   ├── Queries/           # Consultas (operações de leitura)
│   ├── Handlers/          # Manipuladores CQRS
│   └── DTOs/              # Objetos de transferência de dados
├── Infrastructure/        # Camada de Infraestrutura
│   ├── Data/              # Configurações de banco de dados
│   ├── Repositories/      # Implementações dos repositórios
│   └── Services/          # Implementações dos serviços
└── Controllers/           # Camada de Apresentação (API Controllers)
```

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Para criação da API REST
- **Oracle Database** - Banco de dados
- **Dapper** - Micro-ORM para acesso a dados
- **MediatR** - Implementação do padrão CQRS
- **JWT (JSON Web Tokens)** - Autenticação e autorização
- **BCrypt** - Hash de senhas
- **Swagger/OpenAPI** - Documentação da API

## 📊 Modelo de Dados

### Tabela CONTACORRENTE
- `ID` (Number) - Chave primária
- `NUMERO` (Number) - Número da conta corrente
- `CPF` (VARCHAR2(11)) - CPF do titular
- `SENHAHASH` (VARCHAR2(255)) - Senha criptografada
- `SALDO` (Number(10,2)) - Saldo atual
- `ATIVA` (Number(1)) - Status da conta (1=Ativa, 0=Inativa)
- `DATACRIACAO` (Date) - Data de criação
- `DATAATUALIZACAO` (Date) - Data da última atualização

### Tabela MOVIMENTO
- `ID` (Number) - Chave primária
- `IDCONTACORRENTE` (Number) - Referência à conta
- `TIPOMOVIMENTO` (CHAR(1)) - Tipo: 'C' (Crédito) ou 'D' (Débito)
- `VALOR` (Number(10,2)) - Valor da movimentação
- `DATAMOVIMENTO` (Date) - Data/hora da movimentação

## 🚀 Funcionalidades

### 1. **Cadastrar Conta Corrente**
- Recebe CPF e senha do usuário
- Valida o CPF (algoritmo oficial)
- Criptografa a senha com BCrypt
- Gera número sequencial da conta
- Retorna dados da conta criada

### 2. **Login**
- Autentica usuário por CPF e senha
- Gera token JWT para acesso às operações protegidas
- Verifica se a conta está ativa

### 3. **Movimentação de Conta**
- Permite débitos e créditos
- Valida saldo disponível para débitos
- Registra todas as movimentações
- Atualiza saldo da conta

### 4. **Consulta de Saldo**
- Retorna saldo atual e informações da conta
- Requer autenticação JWT

### 5. **Extrato de Movimentações**
- Lista todas as movimentações da conta
- Permite filtro por período
- Ordena por data (mais recente primeiro)

### 6. **Inativar Conta**
- Desativa a conta impedindo novas movimentações
- Mantém histórico de movimentações

## 🔧 Configuração

### Pré-requisitos
- .NET 8.0 SDK
- Oracle Database
- Visual Studio Code ou Visual Studio

### Configuração do Banco de Dados

1. Execute o script `setup_oracle.sql` para criar as tabelas e sequences
2. Configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=servidor:porta/servico;User Id=usuario;Password=senha;Pooling=true;"
  }
}
```

### Executar a Aplicação

```bash
# Restaurar pacotes
dotnet restore

# Executar a aplicação
dotnet run
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

## 📝 Endpoints da API

### Públicos (sem autenticação)
- `POST /api/conta/cadastrar` - Cadastrar nova conta
- `POST /api/conta/login` - Fazer login

### Protegidos (requer JWT)
- `POST /api/movimento/movimentar` - Realizar movimentação
- `GET /api/movimento/saldo/{contaId}` - Consultar saldo
- `GET /api/movimento/extrato/{contaId}` - Consultar extrato
- `PUT /api/conta/inativar/{contaId}` - Inativar conta

## 🧪 Testes

Use o arquivo `requests.http` com a extensão REST Client do VS Code para testar os endpoints.

## 🔒 Segurança

- **Senhas**: Criptografadas com BCrypt
- **JWT**: Tokens com expiração configurável
- **Validações**: CPF validado com algoritmo oficial
- **Autorização**: Endpoints protegidos por JWT

## 📋 Regras de Negócio

1. **CPF único** por conta corrente
2. **Validação de CPF** com algoritmo oficial brasileiro
3. **Senha mínima** de 6 caracteres
4. **Saldo não pode** ficar negativo
5. **Contas inativas** não podem realizar movimentações
6. **Todas as movimentações** são registradas para auditoria

## 🎯 Próximas Melhorias

- [ ] Implementar idempotência nas operações
- [ ] Adicionar logs estruturados
- [ ] Implementar cache para consultas
- [ ] Adicionar testes unitários e de integração
- [ ] Implementar rate limiting
- [ ] Adicionar métricas e monitoramento