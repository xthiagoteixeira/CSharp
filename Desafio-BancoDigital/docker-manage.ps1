# Script PowerShell para gerenciar o ambiente Docker do Banco Digital

param(
    [Parameter(Mandatory=$true)]
    [string]$Action,
    [string]$Service
)

switch ($Action.ToLower()) {
    "up" {
        Write-Host "🚀 Iniciando todos os serviços do Banco Digital..." -ForegroundColor Green
        docker-compose up -d
        Write-Host "✅ Serviços iniciados!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📋 URLs dos serviços:" -ForegroundColor Cyan
        Write-Host "• API Conta Corrente: http://localhost:5222" -ForegroundColor White
        Write-Host "• API Transferência: http://localhost:5037" -ForegroundColor White
        Write-Host "• Kafka UI: http://localhost:8090" -ForegroundColor White
        Write-Host "• Oracle Database: localhost:1521 (XE)" -ForegroundColor White
        Write-Host ""
        Write-Host "🔍 Para ver os logs: docker-compose logs -f [service-name]" -ForegroundColor Yellow
    }
    "down" {
        Write-Host "🛑 Parando todos os serviços..." -ForegroundColor Red
        docker-compose down
        Write-Host "✅ Serviços parados!" -ForegroundColor Green
    }
    "restart" {
        Write-Host "🔄 Reiniciando todos os serviços..." -ForegroundColor Yellow
        docker-compose down
        docker-compose up -d
        Write-Host "✅ Serviços reiniciados!" -ForegroundColor Green
    }
    "logs" {
        if ([string]::IsNullOrEmpty($Service)) {
            Write-Host "📋 Mostrando logs de todos os serviços..." -ForegroundColor Cyan
            docker-compose logs -f
        } else {
            Write-Host "📋 Mostrando logs do serviço: $Service" -ForegroundColor Cyan
            docker-compose logs -f $Service
        }
    }
    "status" {
        Write-Host "📊 Status dos serviços:" -ForegroundColor Cyan
        docker-compose ps
    }
    "build" {
        Write-Host "🔨 Fazendo build das aplicações..." -ForegroundColor Yellow
        docker-compose build --no-cache
        Write-Host "✅ Build concluído!" -ForegroundColor Green
    }
    "clean" {
        Write-Host "🧹 Limpando containers, imagens e volumes..." -ForegroundColor Red
        docker-compose down -v --rmi all
        docker system prune -f
        Write-Host "✅ Limpeza concluída!" -ForegroundColor Green
    }
    "setup" {
        Write-Host "⚙️ Configurando ambiente inicial..." -ForegroundColor Yellow
        Write-Host "1. Parando serviços existentes..." -ForegroundColor White
        docker-compose down -v
        Write-Host "2. Fazendo build das aplicações..." -ForegroundColor White
        docker-compose build --no-cache
        Write-Host "3. Iniciando serviços..." -ForegroundColor White
        docker-compose up -d
        Write-Host "4. Aguardando inicialização..." -ForegroundColor White
        Start-Sleep -Seconds 30
        Write-Host "✅ Ambiente configurado e pronto para uso!" -ForegroundColor Green
    }
    "test" {
        Write-Host "🧪 Executando testes básicos..." -ForegroundColor Cyan
        Write-Host "Testando API Conta Corrente..." -ForegroundColor White
        try {
            Invoke-RestMethod -Uri "http://localhost:5222/health" -Method GET -ErrorAction Stop
            Write-Host "✅ API Conta Corrente está respondendo" -ForegroundColor Green
        } catch {
            Write-Host "❌ API Conta Corrente não está respondendo" -ForegroundColor Red
        }
        
        Write-Host "Testando API Transferência..." -ForegroundColor White
        try {
            Invoke-RestMethod -Uri "http://localhost:5037/health" -Method GET -ErrorAction Stop
            Write-Host "✅ API Transferência está respondendo" -ForegroundColor Green
        } catch {
            Write-Host "❌ API Transferência não está respondendo" -ForegroundColor Red
        }
        Write-Host "✅ Testes básicos concluídos!" -ForegroundColor Green
    }
    default {
        Write-Host "🏦 Banco Digital - Gerenciador Docker" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Uso: .\docker-manage.ps1 -Action [comando] [-Service nome-do-servico]" -ForegroundColor White
        Write-Host ""
        Write-Host "Comandos disponíveis:" -ForegroundColor Yellow
        Write-Host "  up      - Inicia todos os serviços" -ForegroundColor White
        Write-Host "  down    - Para todos os serviços" -ForegroundColor White
        Write-Host "  restart - Reinicia todos os serviços" -ForegroundColor White
        Write-Host "  logs    - Mostra logs (use -Service para serviço específico)" -ForegroundColor White
        Write-Host "  status  - Mostra status dos serviços" -ForegroundColor White
        Write-Host "  build   - Faz build das aplicações" -ForegroundColor White
        Write-Host "  clean   - Remove containers, imagens e volumes" -ForegroundColor White
        Write-Host "  setup   - Configuração inicial completa" -ForegroundColor White
        Write-Host "  test    - Executa testes básicos" -ForegroundColor White
        Write-Host ""
        Write-Host "Exemplos:" -ForegroundColor Yellow
        Write-Host "  .\docker-manage.ps1 -Action up" -ForegroundColor Gray
        Write-Host "  .\docker-manage.ps1 -Action logs -Service api-conta-corrente" -ForegroundColor Gray
        Write-Host "  .\docker-manage.ps1 -Action status" -ForegroundColor Gray
    }
}