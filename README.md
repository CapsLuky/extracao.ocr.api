# API Contratual Admin

API responsável pela extração massiva de textos de documentos via OCR, com capacidade de processar diretórios com gigabytes de arquivos, separando páginas e textos para armazenamento em banco de dados.

## Objetivo

Criar um repositório de consulta para empresas, facilitando a localização e consulta de documentos contratuais. A API extrai e indexa textos de documentos PDF, permitindo:

- Processamento massivo de documentos em lote
- Extração de texto via OCR de páginas individuais
- Pesquisa baseada em dicionário de palavras-chave
- Consulta rápida e eficiente de documentos
- Reutilização dos dados extraídos para análises e integrações

## Funcionalidades Principais

### 1. Extração de Texto
- Processamento recursivo de diretórios e subdiretórios
- Extração de texto de páginas individuais de PDFs
- Validação de tamanhos de caminhos e nomes de arquivos
- Sistema de cópia de segurança dos arquivos originais
- Controle de estado de processamento (Pronto, Processando, Reprocessar, Concluído)

### 2. Pesquisa por Dicionário
- Cadastro de palavras-chave por empresa/filial
- Pesquisa automática em documentos processados
- Armazenamento de resultados de pesquisa
- Prevenção de duplicação de resultados

### 3. Reprocessamento
- Sistema de retentativa para arquivos com falha
- Limpeza e reprocessamento de páginas

## Arquitetura

O projeto segue uma arquitetura em camadas (Clean Architecture), organizada da seguinte forma:

```
API.Contratual.Admin/
├── 0.Presentation/
│   └── API.Contratual.WebApi          # Camada de apresentação (Controllers, Endpoints)
├── 1.Application/
│   └── API.Contratual.Application     # Lógica de aplicação e orquestração
├── 2.Dto/
│   └── API.Contratual.Dto             # Data Transfer Objects
├── 3.Domain/
│   └── API.Contratual.Domain          # Entidades, interfaces e regras de negócio
├── 4.Infrastructure/
│   ├── API.Contratual.Data.Mysql      # Acesso a dados (MySQL)
│   └── API.Contratual.Integration.Http # Integrações HTTP externas
├── 5.CrossCutting/
│   ├── API.Contratual.CrossCutting    # Utilitários, notificações, helpers
│   └── API.Contratual.IoC             # Injeção de dependências
└── 6.Tests/
    └── API.Contratual.Test            # Testes unitários e de integração
```

### Camadas

- **Presentation**: Exposição de endpoints REST para consumo da API
- **Application**: Orquestração de casos de uso e fluxos de negócio
- **Dto**: Objetos de transferência de dados entre camadas
- **Domain**: Núcleo da aplicação com entidades, interfaces e regras de negócio
- **Infrastructure**: Implementações de acesso a dados e integrações externas
- **CrossCutting**: Funcionalidades transversais (logging, notificações, helpers)
- **Tests**: Testes automatizados

## Tecnologias

- .NET (C#)
- MySQL (banco de dados)
- OCR para extração de texto
- Arquitetura em camadas
- Injeção de dependências

## Fluxo de Processamento

1. **Configuração**: Define pasta de origem e configurações por empresa/filial
2. **Descoberta**: Busca recursiva de arquivos PDF na pasta configurada
3. **Validação**: Verifica tamanhos de caminhos e nomes de arquivos
4. **Seleção**: Identifica arquivos novos ou que precisam ser processados
5. **Cópia**: Cria backup dos arquivos originais (opcional)
6. **Registro**: Insere registros na tabela de arquivos
7. **Extração**: Processa cada página do PDF extraindo o texto via OCR
8. **Armazenamento**: Salva textos extraídos no banco de dados
9. **Pesquisa**: Executa pesquisas baseadas no dicionário de palavras
10. **Indexação**: Armazena resultados de pesquisa para consulta rápida

## Principais Endpoints

### Extração de Texto
- `POST /api/extracao/extrair-texto` - Inicia o processo de extração de texto dos arquivos
- `POST /api/extracao/reprocessar` - Retenta extração de arquivos com falha

### Pesquisa
- `POST /api/extracao/pesquisar-dicionario` - Executa pesquisa baseada em dicionário de palavras
- `POST /api/extracao/palavras` - Cadastra palavras-chave para pesquisa

## Configuração

A API requer configuração de:
- String de conexão com MySQL
- Pasta de origem dos documentos por empresa/filial
- Pasta de destino para cópias (opcional)
- Configurações de OCR

## Como Executar

1. Configure a string de conexão no `appsettings.json`
2. Configure as pastas de origem e destino no banco de dados
3. Execute a aplicação:
```bash
dotnet run --project API.Contratual.WebApi
```

## Requisitos

- .NET 6.0 ou superior
- MySQL 8.0 ou superior
- Espaço em disco adequado para processamento de documentos

---

## 📞 Contato

Para mais informações sobre este projeto, entre em contato:

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/lucianorodriguess/)

---