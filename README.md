# 🎫 API de Gestão de Eventos
Uma API RESTful desenvolvida em C# com o objetivo de gerenciar eventos, atrações, ingressos, organizadores e clientes. O projeto foi construído com foco em boas práticas de desenvolvimento, arquitetura limpa e uso do Entity Framework para persistência de dados.

## 📌 Objetivo
Facilitar o controle e a organização de eventos, permitindo a criação, consulta, atualização e remoção de dados referentes a:

- Eventos
- Atrações
- Ingressos
- Clientes
- Organizadores
- Usuários e autenticação

## 🏗️ Arquitetura
O projeto foi desenvolvido utilizando os seguintes princípios:

- Arquitetura em camadas (Clean Architecture)
- Entity Framework Core (ORM para o banco de dados)
- Autenticação baseada em roles (Admin, Organizador, Cliente)
- DTOs e AutoMapper para transporte e transformação de dados

## 📁 Estrutura de Pastas

``` bash
/API
│
├── Controllers/
├── Models/
├── DTOs/
├── Enums/
├── Data/
├── Services/
├── Repositories/
└── Program.cs / Startup.cs
```
---

## 🔗 Endpoints Principais

| Verbo HTTP | Rota                   | Descrição                         |
|------------|------------------------|-----------------------------------|
| GET        | /api/eventos           | Lista todos os eventos            |
| POST       | /api/eventos           | Cria um novo evento               |
| GET        | /api/ingressos/{id}    | Consulta um ingresso específico   |
| POST       | /api/usuarios/login    | Autenticação do usuário           |
| ...        | ...                    | Outros endpoints conforme a regra |

Consulte a documentação da API (Swagger) para visualizar todos os endpoints e seus respectivos modelos.

## 🧠 Modelagem de Dados
As entidades principais incluem:

- Usuario (com enum Roles: Admin, Organizador, Cliente)
- Evento
- Atracao
- Ingresso (com enum TipoIngresso: VIP, PISTA, CAMAROTE...)
- Cliente
- Organizador

! Relacionamentos são gerenciados com chaves estrangeiras e entidades de navegação. !

## ⚙️ Tecnologias Utilizadas

``` bash
.NET 6
C#
Entity Framework Core
SQLite
AutoMapper
Swagger (documentação)
```

## ✅ Testes
A aplicação foi testada com dados simulados (mockados) no banco de dados. Foram criadas 10 instâncias de cada entidade para validar os relacionamentos e regras de negócio.

## 📝 Contribuição
Este projeto foi desenvolvido para fins acadêmicos. Contribuições, sugestões ou melhorias são sempre bem-vindas!

## 📄 Licença
Este projeto é de código aberto e livre para uso educacional.