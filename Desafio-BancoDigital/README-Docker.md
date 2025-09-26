# 🏦 Banco Digital - Ambiente Docker

Este projeto contém a infraestrutura completa para executar o sistema Banco Digital em containers Docker, incluindo:

- **Api_ContaCorrente**: API para gestão de contas correntes
- **Api_Transferencia**: API para processamento de transferências
- **Oracle Database**: Banco de dados principal
- **Apache Kafka**: Sistema de mensageria
- **Kafka UI**: Interface para gerenciamento do Kafka

## 📋 Pré-requisitos

- Docker Desktop instalado e executando
- Docker Compose v3.8 ou superior
- PowerShell (Windows) ou Bash (Linux/Mac)
- Pelo menos 4GB de RAM disponível
- Acesso à internet para download das imagens

## 🚀 Como usar

### Opção 1: Script Automatizado (Recomendado)

#### Windows (PowerShell):
```powershell
# Configuração inicial completa
.\docker-manage.ps1 -Action setup

# Iniciar serviços
.\docker-manage.ps1 -Action up

# Ver status
.\docker-manage.ps1 -Action status

# Ver logs
.\docker-manage.ps1 -Action logs -Service api-conta-corrente

# Parar serviços
.\docker-manage.ps1 -Action down
```

#### Linux/Mac (Bash):
```bash
# Dar permissão de execução
chmod +x docker-manage.sh

# Configuração inicial completa
./docker-manage.sh setup

# Iniciar serviços
./docker-manage.sh up

# Ver status
./docker-manage.sh status

# Ver logs
./docker-manage.sh logs api-conta-corrente

# Parar serviços
./docker-manage.sh down
```

### Opção 2: Docker Compose Manual

```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver status
docker-compose ps

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down

# Rebuild das aplicações
docker-compose build --no-cache
```

## 🌐 URLs dos Serviços

Após a inicialização, os serviços estarão disponíveis em:

| Serviço | URL | Descrição |
|---------|-----|-----------|
| API Conta Corrente | http://localhost:5222 | Gestão de contas e movimentações |
| API Transferência | http://localhost:5037 | Processamento de transferências |
| Kafka UI | http://localhost:8090 | Interface do Kafka |
| Oracle Database | localhost:1521 | Banco de dados (XE) |

## 🗄️ Banco de Dados

### Credenciais Oracle:
- **Host**: localhost
- **Port**: 1521
- **SID**: XE
- **User**: SYSTEM
- **Password**: BancoDigital123

### Contas de Teste Pré-cadastradas:

| ID | CPF | Nome | Saldo Inicial | Senha |
|----|-----|------|---------------|-------|
| 1 | 12345678901 | João Silva | R$ 5.000,00 | 123456 |
| 2 | 19632055888 | Maria Santos | R$ 4.500,00 | 123456 |
| 3 | 98765432100 | Pedro Oliveira | R$ 3.000,00 | 123456 |

## 🧪 Testando o Sistema

### 1. Login (obter token JWT):
```bash
curl -X POST http://localhost:5222/api/conta/login \
  -H "Content-Type: application/json" \
  -d '{"cpf":"19632055888","senha":"123456"}'
```

### 2. Consultar Saldo:
```bash
curl -X GET http://localhost:5222/api/movimento/saldo/2 \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 3. Fazer Transferência:
```bash
curl -X POST http://localhost:5037/api/transferencia/transferir \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{"contaDestinoId":3,"valor":100.00,"chaveIdempotencia":"test-001"}'
```

## 📊 Monitoramento

### Verificar Status dos Containers:
```bash
docker-compose ps
```

### Ver Logs em Tempo Real:
```bash
# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f api-conta-corrente
docker-compose logs -f api-transferencia
docker-compose logs -f oracle-db
docker-compose logs -f kafka
```

### Verificar Saúde dos Serviços:
```bash
# API Conta Corrente
curl http://localhost:5222/health

# API Transferência
curl http://localhost:5037/health
```

## 🛠️ Comandos Úteis

### Rebuild Completo:
```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

### Limpeza Completa:
```bash
docker-compose down -v --rmi all
docker system prune -f
```

### Conectar ao Banco Oracle:
```bash
docker exec -it oracle-banco-digital sqlplus system/BancoDigital123@XE
```

### Acessar Container da API:
```bash
# API Conta Corrente
docker exec -it api-conta-corrente bash

# API Transferência
docker exec -it api-transferencia bash
```

## 🔧 Solução de Problemas

### Problemas Comuns:

1. **Porta já em uso**:
   ```bash
   # Verificar processos usando as portas
   netstat -ano | findstr :5222
   netstat -ano | findstr :5037
   ```

2. **Oracle não inicializa**:
   - Aguarde 2-3 minutos para inicialização completa
   - Verifique logs: `docker-compose logs oracle-db`
   - Reinicie se necessário: `docker-compose restart oracle-db`

3. **APIs não conectam ao banco**:
   - Verifique se o Oracle está healthy: `docker-compose ps`
   - Reinicie as APIs: `docker-compose restart api-conta-corrente api-transferencia`

4. **Kafka não funciona**:
   - Verifique se Zookeeper está rodando: `docker-compose logs zookeeper`
   - Reinicie Kafka: `docker-compose restart kafka`

### Logs Detalhados:
```bash
# Ver logs com timestamp
docker-compose logs -t -f [service-name]

# Ver apenas últimas linhas
docker-compose logs --tail=50 [service-name]
```

## 🏗️ Arquitetura

```
┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   API Gateway   │
│   (Futuro)      │    │   (Futuro)      │
└─────────────────┘    └─────────────────┘
         │                       │
         └───────────┬───────────┘
                     │
    ┌────────────────┴────────────────┐
    │                                 │
┌───▼────────────┐              ┌────▼──────────┐
│ Api_ContaCorrente │              │ Api_Transferencia │
│    (Port 5222)     │              │    (Port 5037)    │
└───┬────────────┘              └────┬──────────┘
    │                                 │
    └─────────────┬───────────────────┘
                  │
    ┌─────────────▼─────────────┐
    │     Oracle Database       │
    │       (Port 1521)         │
    └───────────────────────────┘
    
    ┌─────────────────────────────┐
    │        Apache Kafka         │
    │    (Port 9092/29092)        │
    └─────────────────────────────┘
```

## 📝 Notas Importantes

- O banco Oracle pode levar até 3 minutos para inicializar completamente
- As APIs têm health checks e só ficam disponíveis após o banco estar pronto
- Kafka é configurado para criar tópicos automaticamente
- Todos os dados são persistidos em volumes Docker
- As senhas das contas de teste são "123456" (apenas para desenvolvimento)

## 🤝 Suporte

Para problemas ou dúvidas:
1. Verifique os logs dos containers
2. Execute o teste básico: `.\docker-manage.ps1 -Action test`
3. Reinicie o ambiente: `.\docker-manage.ps1 -Action restart`