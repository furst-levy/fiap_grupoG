# fiap.Big Data Science.3BDTO.Grupo G

=====================

Esse repositório é referente a Fase 4 do curso BigData da FIAP para o grupo G.

Se trata de um serviço backend que busca em 18 perfis do twitter, do segmento financeiro, onde coleta todos os tweets postados por esses via streaming, em tempo real.

Esses tweets são enviados para um tópico do kafka para serem consumidos por ferramentas e serviços de ingestão de dados.

A ideia é gerar indicadores com esses dados, que é apenas uma parte do projeto do grupo, que trata problemas de inadiplência e conhecimento do perfil do cliente na institução financeira. Dessa forma dentro da solução, esses indcadores serão cruzados com os dados internos da instituição.

=====================

## Technology

- .NET Core

## Dev requirements

- VS ou VS Code
- .NET Core SDK 3.1
- Docker (with docker compose)
- Kafka

## How to use

- fazer o clone  do projeto

- executar o camando na pasta raiz do projeto:

```bash
docker-compose up --build
```

3 Jobs serão iniciados, o Kafka, Zookeeper e o microserviço que foi desenvolvido.
