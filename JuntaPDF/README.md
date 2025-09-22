# JuntaPDF

Uma aplicação Windows Forms simples e eficiente para mesclar múltiplos arquivos PDF em um único documento.

## 📋 Sobre o Projeto

O JuntaPDF é uma ferramenta desktop desenvolvida em C# com Windows Forms que permite aos usuários combinar vários arquivos PDF de forma rápida e intuitiva. A aplicação oferece uma interface amigável com funcionalidades de validação de arquivos, monitoramento de progresso e tratamento de erros.

## ✨ Funcionalidades

- **Mesclagem de PDFs**: Combine múltiplos arquivos PDF em um único documento
- **Validação de Arquivos**: Verificação automática da integridade dos arquivos PDF
- **Interface Intuitiva**: Design limpo e fácil de usar
- **Monitoramento de Progresso**: Barra de progresso durante a operação de mesclagem
- **Informações de Arquivo**: Visualização de detalhes como número de páginas e tamanho do arquivo
- **Tratamento de Erros**: Gerenciamento robusto de exceções e feedback ao usuário

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**: Framework principal
- **C# 12.0**: Linguagem de programação
- **Windows Forms**: Interface gráfica
- **iText 7**: Biblioteca para manipulação de PDFs
- **Async/Await**: Programação assíncrona para melhor responsividade

### Componentes Principais

- **PdfMergerService**: Serviço responsável por todas as operações relacionadas a PDFs
- **PdfDocument**: Modelo que representa um documento PDF com suas propriedades
- **IPdfMergerService**: Interface que define o contrato para operações com PDFs

**Solução**: Instale o pacote `itext.bouncy-castle-adapter`


### PDFs não são mesclados corretamente

**Verificações**:
- Certifique-se de que todos os arquivos PDF são válidos
- Verifique se há permissões de escrita no diretório de saída
- Confirme se os arquivos PDF não estão protegidos por senha

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 🔄 Versões

- **v1.0.0** - Versão inicial com funcionalidades básicas de mesclagem de PDFs

---

**Desenvolvido com ❤️ em C#**
