## DGBK Rest Api

Detalhamento técnico de exemplo de API Rest de integração de pagamento usando PosgreSQL e Entity Framework.

> GitHub: [leadsoftlucas/dgbk-api](https://github.com/leadsoftlucas/dgbk-api)

### 1. **Introdução**

Este projeto é uma API Restful desenvolvida em C# utilizando o .NET 9.0, que serve como um exemplo de integração de pagamento. A API foi projetada para ser simples, eficiente e fácil de entender, seguindo as melhores práticas de desenvolvimento de software, incluindo DDD (Domain-driven design) e Clean Architecture (Hexagonal). Por motivos óbvios e de tempo hábil, eu separei as camadas apenas em diretórios, não em projetos, mas em uma estrutura que fica simples separar e desacoplar futuramente.

#### 1.1 **Clone o repositório**

Para iniciar, clone o repositório para sua máquina local usando o comando abaixo:

```bash
git clone git@github.com:leadsoftlucas/dgbk-api.git
```

#### 1.2 **Branch Postgre SQL**

Faça chaeckout na branch `postgresql` para seguir os passos abaixo:

- Branch: [postgresql](https://github.com/leadsoftlucas/dgbk-api/tree/postgresql)

```bash
git checkout postgresql
```

Você pode abrir a solução no Visual Studio ou qualquer outra IDE de sua escolha. Antes de executar a aplicação, certifique-se de ter as dependências necessárias instaladas.
Você deve configurar as variáveis de ambiente para que a aplicação seja executada corretamente.

`LucasRT.DGBK.RestApi/Properties/launchSettings.json`

```json
"environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "HMAC": "bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ",
    "WEBHOOK_ENDPOINT": "https://localhost:7077/api/"
}
```

### 2. **Banco de dados PostgreSQL**

Para este contexto e propósito, usarei o PostgreSQL como banco de dados relacional.

> Embora eu seja entusiasta e arquiteto de soluções NoSQL em dados na **RavenDB**, este exemplo servirá como uma boa comparação.

#### 2.1 **Faça o Download e execute o PostgreSQL caso for rodar o projeto na sua máquina local**

Baixe o PostgreSQL do [site oficial](https://www.postgresql.org/download/) e faça a instalação necessária para usar localmente.
Inicie o serviço no gerenciador de Serviços do Windows.

> Baixe também o pgAdmin Tool para facilitar a administração do banco de dados.

#### 2.2 **Iniciando o serviço do PostgreSQL no Windows**

Depois de todos os passos da instalação e reiniciar o computador, você precisa iniciar o servio do PostgreSQL service.
Você pode fazer isso executando o comando abaixo do seu terminal (garanta que esteja executando como Administrador) ou abra o gerenciador de serviços do Windows e inicie com botão direito, o serviço do PostgreSQL:

```bash
net start postgresql-x64-13
```

Após instalar o pgAdmin, abra-o e crie uma nova base de dados chamada `DGBK` ou qualquer outro nome que preferir.

#### 2.2.1 **Configure o Entity Framework Core no seu projeto para instalar as Migrations**

Instale o Entity Framework Core tools no seu projeto para gerenciar as migrations e atualizações do banco de dados. Você pode fazer isso executando o comando abaixo no seu terminal:

```bash
dotnet tool install --global dotnet-ef
```

#### 2.2.2 **Conecte à sua base de dados**

Abra o AppSettings de desenvolvimento `LucasRT.DGBK.RestApi/appsettings.development.json` e configure a propriedade `ConnectionStrings` com a string de conexão do PostgreSQL:

```json
"ConnectionStrings": {
  "PostgreSQL": "Host=localhost;Port=5432;Database=DGBK;Username=postgres;Password=admin;"
}
```

#### 2.2.3 **Configurando ambiente de banco de dados para a aplicação**

Execute o comando abaixo para criar a migration inicial que criará as tabelas das Entidades na sua camada de Domínio:

```bash	
 dotnet ef migrations add InitialMigration -p LucasRT.DGBK.RestApi
 dotnet ef database update -p LucasRT.DGBK.RestApi
```

### 3. **Executar a aplicação **
Use o comando abaixo diretamente no Console ou use o comando de executar na sua IDE:

```bash
dotnet run
```

Swagger deve ser exibir sob a URL `https://localhost:7077/swagger/index.html` ou `http://localhost:5205/swagger/index.html` dependendo da sua configuração `http` ou `https`.

> O ambiente será informado no título do Swagger.

## **Sobre Autor**

[**Lucas Tavares** - Arquiteto de Soluções](https://www.linkedin.com/in/lucasrtavares/) 

**Origem:** 1991, Belo Horizonte, Minas Gerais, Brasil

**Formação:** Bacharelado em Sistemas de Informação desde 2015 pela Faculdade Cotemig (Minas Gerais, Brazil)

**Cidade atual:** Curitiba, Paraná, Brasil (2015)

**Experiência:** Desenvolvedor .net desde 2008, especialista em SQL Server (2013-2018), especialista em arquitetura Web Apis de alta performance e modelagem de dados NoSQL com RavenDB desde 2018 pela LeadSoft. Atualmente Arquiteto de Soluções na RavenDB.

**Contatos:** [LinkedIn](https://www.linkedin.com/in/lucasrtavares/) | [GitHub](https://github.com/leadsoftlucas) | [LeadSoft](mailto:lucas@leadsoft.inf.br), [RavenDB](mailto:lucas.tavares@ravendb.net)


> Quaisquer dúvidas, sugestões ou se quiser apenas conversar sobre este repositório, sinta-se à vontade para entrar em contato comigo.
>
> Obrigado pelo seu tempo!