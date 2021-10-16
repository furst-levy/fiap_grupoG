# fiap.Big Data Science.3BDTO.Grupo G

=====================

Esse repositório é referente a Fase 3 do curso BigData da FIAP para o grupo G.

Se trata de um serviço backend que busca em 18 perfis do twitter, do segmento financeiro, onde coleta todos os tweets postados por esses no dia anterior.

Esses tweets são refinados na ferramenta AWS Comprehend com objetivo de coletar informaçoes úteis e sentimento dentro do texto postado.

No final, os dados são persistidos em uma collection no MongoDB.

A ideia é gerar indicadores com esses dados, que é apenas uma parte do projeto do grupo, que trata problemas de inadiplência e conhecimento do perfil do cliente na institução financeira. Dessa forma dentro da solução, esses indcadores serão cruzados com os dados internos da instituição.

=====================

## Technology

- .NET Core

## Dev requirements

- VS ou VS Code
- .NET Core SDK 3.1
- Docker (with docker compose)
- AWS Comprehend
- MongoDB

## How to use

- fazer o clone  do projeto

- alterar o arquivo src\Fiap.GrupoG.Jobs\appsettings.json nas propriedades "Access_Key" e "Secret_Access_Key" com os valores informados pelo grupo no PDF da entrega.
Obs.: Não podemos deixar esse valor no github, pois o repositório é publico e a conta da AWS pode levar penalidade.

- executar o camando na pasta raiz do projeto:

```bash
docker-compose up --build
```

2 Jobs serão iniciados, o do MongoDB e do servio backend
