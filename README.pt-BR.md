# DengueWatch — desafio DIO .NET e MongoDB

[English](README.md) | [Português (BR)](README.pt-BR.md)

Projeto de portfólio **educacional**, com dados sintéticos. Inspirado no laboratório
"Construindo um projeto de uma API.NET integrada ao MongoDB" da DIO. O objetivo é praticar
Controllers, injeção de dependência, MongoDB, Docker Compose, autenticação JWT e testes — não
servir usuários reais.

## Contexto do desafio

O enunciado do portfólio pede que a pessoa crie seu próprio repositório, replique ou melhore o
projeto demonstrado e inclua os links e arquivos necessários para entendê-lo. O fork do repositório
de referência é opcional.

- Links do laboratório e das aulas: [projetoNetMongo.md](projetoNetMongo.md)
- Implementação de referência: [gabrielfaraday/dotnet-mongo](https://github.com/gabrielfaraday/dotnet-mongo)

O repositório de referência demonstra uma API pequena com MongoDB. O DengueWatch é uma adaptação
própria e o contrato da API **não** é assumido igual ao `/infectado` da referência.

## Decisões do projeto

- .NET 10 (`net10.0`) com ASP.NET Core Controllers.
- `WebApplication.CreateBuilder` (builder padrão).
- MongoDB via `MongoDB.Driver`, executado localmente pelo Docker Compose.
- OpenAPI e Scalar habilitados apenas em `Development`.
- Autenticação educacional: login e senha, emitindo um JWT. É uma simplificação de aprendizado, não
  uma solução de identidade pronta para produção.
- Uso exclusivo de dados sintéticos.

Essas são decisões **do DengueWatch**, não requisitos oficiais declarados pelo enunciado do desafio.

## Estado atual

O repositório contém hoje o **scaffold** da aplicação: configuração e injeção de dependência do
MongoDB, Docker/Compose, Controllers, DTOs de exemplo, OpenAPI/Scalar e um endpoint `/health`
básico.

Ainda **não** estão implementados: CRUD de casos, operações de leitura/escrita no MongoDB, login/JWT
e testes automatizados. A última revisão compilou a API com sucesso, mas integração e testes
permanecem não verificados.

O plano detalhado, com marcos e critérios de aceite, está em [plan/README.pt-BR.md](plan/README.pt-BR.md).

## Executar localmente

```bash
docker compose up --build
```

A API fica em `http://localhost:5000`. Em `Development`, o Scalar está disponível em
`http://localhost:5000/scalar/v1`.

## Dados e licença

Projeto educacional. Não utilize dados reais de pacientes ou de saúde.
