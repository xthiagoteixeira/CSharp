#!/bin/bash

# Script para gerenciar o ambiente Docker do Banco Digital

case "$1" in
  "up")
    echo "🚀 Iniciando todos os serviços do Banco Digital..."
    docker-compose up -d
    echo "✅ Serviços iniciados!"
    echo ""
    echo "📋 URLs dos serviços:"
    echo "• API Conta Corrente: http://localhost:5222"
    echo "• API Transferência: http://localhost:5037"
    echo "• Kafka UI: http://localhost:8090"
    echo "• Oracle Database: localhost:1521 (XE)"
    echo ""
    echo "🔍 Para ver os logs: docker-compose logs -f [service-name]"
    ;;
  "down")
    echo "🛑 Parando todos os serviços..."
    docker-compose down
    echo "✅ Serviços parados!"
    ;;
  "restart")
    echo "🔄 Reiniciando todos os serviços..."
    docker-compose down
    docker-compose up -d
    echo "✅ Serviços reiniciados!"
    ;;
  "logs")
    if [ -z "$2" ]; then
      echo "📋 Mostrando logs de todos os serviços..."
      docker-compose logs -f
    else
      echo "📋 Mostrando logs do serviço: $2"
      docker-compose logs -f "$2"
    fi
    ;;
  "status")
    echo "📊 Status dos serviços:"
    docker-compose ps
    ;;
  "build")
    echo "🔨 Fazendo build das aplicações..."
    docker-compose build --no-cache
    echo "✅ Build concluído!"
    ;;
  "clean")
    echo "🧹 Limpando containers, imagens e volumes..."
    docker-compose down -v --rmi all
    docker system prune -f
    echo "✅ Limpeza concluída!"
    ;;
  "setup")
    echo "⚙️ Configurando ambiente inicial..."
    echo "1. Parando serviços existentes..."
    docker-compose down -v
    echo "2. Fazendo build das aplicações..."
    docker-compose build --no-cache
    echo "3. Iniciando serviços..."
    docker-compose up -d
    echo "4. Aguardando inicialização..."
    sleep 30
    echo "✅ Ambiente configurado e pronto para uso!"
    ;;
  "test")
    echo "🧪 Executando testes básicos..."
    echo "Testando API Conta Corrente..."
    curl -f http://localhost:5222/health || echo "❌ API Conta Corrente não está respondendo"
    echo "Testando API Transferência..."
    curl -f http://localhost:5037/health || echo "❌ API Transferência não está respondendo"
    echo "✅ Testes básicos concluídos!"
    ;;
  *)
    echo "🏦 Banco Digital - Gerenciador Docker"
    echo ""
    echo "Uso: $0 [comando]"
    echo ""
    echo "Comandos disponíveis:"
    echo "  up      - Inicia todos os serviços"
    echo "  down    - Para todos os serviços"
    echo "  restart - Reinicia todos os serviços"
    echo "  logs    - Mostra logs (logs [service-name] para serviço específico)"
    echo "  status  - Mostra status dos serviços"
    echo "  build   - Faz build das aplicações"
    echo "  clean   - Remove containers, imagens e volumes"
    echo "  setup   - Configuração inicial completa"
    echo "  test    - Executa testes básicos"
    echo ""
    echo "Exemplos:"
    echo "  $0 up"
    echo "  $0 logs api-conta-corrente"
    echo "  $0 status"
    ;;
esac