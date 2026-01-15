# Sistema de Logística com Microsserviços (.NET + RabbitMQ)

Projeto de estudo focado em **backend corporativo**, utilizando **.NET 8**, **RabbitMQ**, **Docker** e **Swagger**, simulando comunicação assíncrona entre microsserviços de logística.

## Arquitetura

- **Logistica.Pedidos.Api**
  - API REST responsável por criar pedidos
  - Publica eventos no RabbitMQ
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

---

## Tecnologias utilizadas

- .NET 8
- C#
- RabbitMQ
- Docker & Docker Compose
- Swagger (OpenAPI)
- Git & GitHub

---

## Pré-requisitos

- .NET SDK 8+
- Docker Desktop
- Git

---

## Subindo o RabbitMQ com Docker

Na raiz do projeto, execute:

```bash
docker compose up -d

