# 🛍️ DeveloperStore API

**DeveloperStore** é uma API RESTful desenvolvida em **.NET 8**, utilizando **PostgreSQL**, **Entity Framework Core** e arquitetura limpa (**Clean Architecture**).  
O objetivo é gerenciar **usuários, produtos, carrinhos, vendas e autenticação JWT**.

---

## 📌 Funcionalidades

- 🔐 **Autenticação JWT** (`Login` e proteção de rotas).
- 👤 **Gerenciamento de Usuários** (CRUD + filtros).
- 📦 **Gerenciamento de Produtos** (com paginação e filtros).
- 🛒 **Carrinhos de Compras** (adicionar, atualizar, remover itens).
- 💰 **Vendas** (criar, listar, cancelar venda e itens).
- 📄 **Filtros, paginação e ordenação** em todos os endpoints.
- 🧪 **Testes unitários completos** com **xUnit + Moq**.

---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture**, separando responsabilidades:
```bash
DeveloperStore
│
├── DeveloperStore.Api → Controllers, DTOs, Configurações
├── DeveloperStore.Application → Services, Interfaces, Regras de Negócio
├── DeveloperStore.Domain → Entidades e Regras de Domínio
├── DeveloperStore.Infrastructure→ Persistência, Repositórios, Migrations
└── DeveloperStore.Tests → Testes Unitários
```

- **DTOs** → Transportam dados entre camadas sem expor entidades diretamente.
- **Services** → Contêm a lógica de negócio.
- **Interfaces** → Definem contratos para implementação.
- **Controllers** → Recebem requisições HTTP e chamam os serviços.

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8**
- **Entity Framework Core**
- **PostgreSQL**
- **Swagger / OpenAPI**
- **xUnit** + **Moq** (testes)
- **JWT (JSON Web Token)** para autenticação
- **Docker** (opcional)

---

## 📦 Como Executar

### 1️⃣ Clonar o repositório
```bash
git clone https://github.com/Jolusofo/developerstore-sales-api.git

cd DeveloperStore
```
## Configurar o banco de dados
No arquivo appsettings.json, configure a string de conexão:

```bash
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=developerstore;Username=postgres;Password=senha"
}
```

## Rodar as migrations

```bash
dotnet ef database update
```

## Executar o projeto

```bash
dotnet run --project DeveloperStore.Api
```

## A API estará disponível em:
```bash
http://localhost:5135
```

## E a documentação Swagger em:
http://localhost:5135/swagger

## 🔑 Autenticação JWT
Para acessar rotas protegidas:

Login com usuário e senha em /api/auth/login.

Receba o token JWT.

Inclua o token no Authorization Header:

```bash
Authorization: Bearer seu_token_aqui
```

## 📖 Exemplos de Endpoints

### 🔹 Login

```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "123456"
}
```

### 🔹 Criar Produto

```bash
POST /api/products
Authorization: Bearer <seu_token>
Content-Type: application/json

{
  "title": "Notebook Dell XPS",
  "price": 7999.90,
  "category": "Eletrônicos"
}
```

## 🧪 Testes
Rodar todos os testes:

```bash
dotnet test
```

Estrutura de testes:

Controllers Tests → valida comportamento das rotas.

Services Tests → testa a lógica de negócio isoladamente.

Domain Tests → valida regras de entidades.

## 📌 Boas Práticas Adotadas
✔ Clean Architecture

✔ SOLID Principles

✔ DTO Pattern

✔ Dependency Injection

✔ Testes Unitários

✔ Swagger para documentação


