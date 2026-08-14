# CRUD Cafeteria — Testes

Projeto de testes unitários da [CRUD Cafeteria API](https://github.com/kaeve1/cafeCrud). Aqui ficam os testes automatizados que validam a camada de serviço da API, garantindo que as regras de negócio continuem se comportando como esperado a cada alteração no código.

Este documento explica o que os testes cobrem, como eles são construídos e como executá-los, para que qualquer pessoa entenda o cenário antes de rodar.

## Por que um projeto de testes separado

A API principal e os testes vivem em repositórios diferentes, mas o projeto de testes referencia diretamente o projeto da API. Isso permite testar as classes reais da aplicação (o serviço, os DTOs, a entidade e os enums) sem duplicar código, mantendo o repositório da API limpo, sem dependências de teste, e concentrando toda a verificação automatizada em um lugar próprio.

Para que os testes compilem, os dois repositórios precisam estar clonados lado a lado na mesma pasta, já que a referência ao projeto da API aponta para `..\CrudCafeteria\CrudCafeteria.csproj`.

## O que é testado

O alvo dos testes é a camada de serviço da API, onde moram as regras de negócio: criação, busca, atualização parcial e exclusão de solicitações de manutenção.

A estratégia é testar essa camada de forma **isolada**, sem tocar em banco de dados real. Como o serviço depende de uma interface de repositório, e não da implementação concreta, é possível substituir o repositório por um objeto simulado que devolve exatamente o cenário que cada teste precisa. Assim, os testes verificam apenas a lógica do serviço, de forma rápida e determinística, sem depender de infraestrutura externa.

Todos os testes seguem o padrão Arrange-Act-Assert: primeiro preparam o cenário e o objeto simulado, depois executam o método sob teste, e por fim verificam o resultado.

## Cenários cobertos

Os testes estão agrupados por operação.

**Atualização.** Um teste verifica que atualizar um registro inexistente devolve falso. Outro confirma que atualizar um registro existente devolve verdadeiro e aplica a mudança no campo enviado. Um terceiro garante o comportamento de atualização parcial: quando um campo não é enviado na requisição, o valor antigo é preservado, e apenas o campo informado muda.

**Exclusão.** Um teste verifica que excluir um registro inexistente devolve falso. Outro confirma que excluir um registro existente devolve verdadeiro. Um terceiro vai além do resultado e verifica o comportamento interno: confirma que o método de exclusão do repositório foi de fato chamado, exatamente uma vez, com o registro correto.

**Busca por identificador.** Um teste confirma que buscar um identificador inexistente devolve nulo. Outro verifica que buscar um registro existente devolve o objeto de resposta com os dados corretos.

**Criação.** Um teste garante uma regra importante do negócio: toda solicitação nasce com status Aberta, independentemente do que venha na requisição. Outro verifica que a data de abertura é preenchida automaticamente pelo servidor no momento da criação, checando que o valor gerado cai dentro da janela de tempo esperada.

## Tecnologias

* .NET 10
* xUnit como framework de testes
* Moq para simular o repositório e verificar interações
* coverlet para coleta de cobertura de código

## Estrutura

```
CrudCafeteriaTest.csproj          Configuração do projeto e referência à API
Service/
  SolicitacaoServiceTests.cs      Testes da camada de serviço
```

## Como executar

Pré-requisito: .NET SDK 10 instalado.

Os dois repositórios precisam estar clonados na mesma pasta, lado a lado, por causa da referência de projeto:

```
git clone https://github.com/kaeve1/cafeCrud.git
git clone https://github.com/kaeve1/cafeCrudTests.git
```

A estrutura de pastas deve ficar assim:

```
pasta-raiz/
├── cafeCrud/           API principal (projeto CrudCafeteria)
└── cafeCrudTests/      Este projeto de testes
```

Entre na pasta dos testes e execute a suíte:

```
cd cafeCrudTests
dotnet test
```

O resultado mostra quantos testes passaram e quantos falharam. Para incluir a cobertura de código na execução:

```
dotnet test --collect:"XPlat Code Coverage"
```
