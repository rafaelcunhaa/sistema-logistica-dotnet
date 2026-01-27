# Sistema de Logística com Microsserviços (.NET + RabbitMQ)

Projeto de estudo focado em **backend corporativo**, utilizando **.NET 8**, **RabbitMQ**, **Docker** e **Swagger**, simulando comunicação assíncrona entre microsserviços de logística.

## Arquitetura

- **Logistica.Pedidos.Api**
  - API REST responsável por criar e consultar pedidos
  - Publica eventos no RabbitMQ ao criar um pedido
  - Documentada com Swagger

- **Logistica.Estoque.Consumer**
  - Serviço consumidor
  - Escuta eventos de pedidos criados
  - Simula processamento de estoque

- **Logistica.Shared**
  - Biblioteca compartilhada
  - Centraliza contratos entre serviços (ex: nomes de filas)

- **RabbitMQ**
  - Broker de mensageria
  - Executado via Docker

## Tecnologias utilizadas

- .NET 8
- C#
- RabbitMQ
- Docker & Docker Compose
- SQL Server + Entity Framework Core
- Swagger (OpenAPI)
- Git & GitHub

## Pré-requisitos

- .NET SDK 8+
- Docker Desktop
- SQL Server local (ex.: SQL Server Express)
- Git

## Configuração

1. Suba o RabbitMQ:

```bash
docker compose up -d
```

2. Ajuste a string de conexão do SQL Server:

O arquivo `Logistica.Pedidos.Api/appsettings.Development.json` define a conexão padrão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LogisticaPedidosDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Altere para o seu ambiente caso necessário.

3. Crie/atualize o banco de dados com EF Core:

```bash
dotnet ef database update --project Logistica.Pedidos.Api
```

## Como executar

### API de Pedidos

```bash
dotnet run --project Logistica.Pedidos.Api
```

- Swagger: `http://localhost:5000/swagger`
- API base: `http://localhost:5000`

### Consumer de Estoque

Em outro terminal:

```bash
dotnet run --project Logistica.Estoque.Consumer
```

O consumer ficará aguardando mensagens da fila de pedidos criados.

## Endpoints principais

- `POST /pedidos` → Cria um novo pedido e publica no RabbitMQ.
- `GET /pedidos` → Lista pedidos.
- `GET /pedidos/{id}` → Consulta um pedido pelo ID.

Exemplo de criação de pedido:

```bash
curl -X POST http://localhost:5000/pedidos \
  -H "Content-Type: application/json" \
  -d '{
    "cliente": "Maria Silva",
    "produto": "Notebook",
    "quantidade": 2,
    "valor": 8500.00
  }'
```

## 🩺 Health Check

A API possui um endpoint de health check para verificação de status da aplicação.

### Endpoint

`GET /health`

### Exemplo de resposta

```json
{
  "status": "ok",
  "timestamp": "2026-01-23T23:45:12Z"
}
```

## RabbitMQ Management

O painel do RabbitMQ fica disponível em:

- URL: `http://localhost:15672`
- Usuário: `guest`
- Senha: `guest`

## Estrutura do repositório

```
Logistica.Pedidos.Api/        # API de pedidos
Logistica.Estoque.Consumer/   # Worker consumidor
Logistica.Shared/             # Contratos compartilhados
SistemaLogistica.sln          # Solution .NET
```

## Licença

Projeto de estudo sem licença definida.
