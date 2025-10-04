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

## 1.2 **Branch RavenDB**

- Branch: [ravendb](https://github.com/leadsoftlucas/dgbk-api/tree/ravendb)

```bash
git checkout ravendb
```

Você pode abrir a solução no Visual Studio ou qualquer outra IDE de sua escolha. Antes de executar a aplicação, certifique-se de ter as dependências necessárias instaladas.
Você deve configurar as variáveis de ambiente para que a aplicação seja executada corretamente.

`LucasRT.DGBK.RestApi/Properties/launchSettings.json`

```json
"environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "HMAC": "bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ",
    "WEBHOOK_ENDPOINT": "https://localhost:7077/api/",
    "RAVENDB_PASSWORD": "LucasDGBK"
}
```

### 2. **Banco de dados RavenDB**

Como sou entusiasta do RavenDB, esta é a versão do mesmo projeto, mas usando **RavenDB** para voar alto ao invés de pesar como um elefante!

#### 2.1 **Faça o Download e execute o RavenDB caso for rodar o projeto na sua máquina local**

Baixe o RavenDB do [site oficial](https://ravendb.net/download/), descompacte seu cluster, e no arquivo `run.ps1`, clique com o botão direito e `Executar com o PowerShell`, caso vá executar isso localmente.
Inicie o serviço, o Raven Studio inicializará automaticamente a configuração do cluster e iniciar quandi finalizar, poderá utiliza-lo no seu Browser.

> Alternativamente, utilize o [RavenDB Cloud](https://ravendb.net/cloud) para criar um cluster gratuito de desenvolvimento.

Recomendo usar a instância protegida por SSL, que é a padrão.

Por meio do Raven Studio, você pode criar um banco de dados chamado `DGBK` ou deixar que a aplicação crie automaticamente ao iniciar.

> Caso tenha criado um servidor protegido por SSL, você precisará criar um certificado.
> 
> - No menu de navegação, clique em **Manage Server > Certificates > Manage Certificates > Generate Client Certificate**.
> - Cole o arquivo .pfx na pasta `LucasRT.DGBK.RestApi/Infrastructure/Certificates` e renomeie para `LucasDGBK.pfx`. Configure-o como um `Embedded Resource`.
> - Configure a senha do certificado como `RAVENDB_PASSWORD` na variável de ambiente.
> - Selecione o certificado gerado e selecione na opção `git > Untrack this item` para evitar expor seu certificado no repositório.

#### 2.2 **Conecte à sua base de dados**

Abra o AppSettings de desenvolvimento `LucasRT.DGBK.RestApi/appsettings.development.json` e configure a propriedade `RavenSettings` com a configuração do cluster RavenDB abaixo:

> Configure conforme suas configurações do Cluster RavenDB criado.
>
> - Se estiver usando RavenDB Cloud, insira as URLs dos três nós do cluster, A, B e C.

```json
{
  "RavenSettings": {
    "Urls": [
      "https://a.ravenchild.development.run/"
    ],
    "DatabaseName": "DGBK",
    "CertificateResourceName": "LucasDGBK"
  }
}
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