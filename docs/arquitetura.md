# Arquitetura

## Camadas

```mermaid
flowchart LR
    subgraph Front["Front-end · Angular standalone (localhost:4200)"]
        P[Páginas<br/>login, alunos, planos,<br/>mensalidades, relatório]
        G[Guarda de rota<br/>authGuard]
        I[Interceptor<br/>anexa Bearer token]
        S[ApiService / AuthService<br/>HttpClient]
        P --> S --> I
        G -.protege.-> P
    end

    subgraph Back["Back-end · ASP.NET Core Web API (localhost:5080)"]
        C[Controllers<br/>recebem e respondem]
        SV[Services<br/>regras de negócio]
        DB[(AcademiaContext<br/>EF Core)]
        C --> SV --> DB
    end

    L[[Academia.Validacao<br/>Class Library<br/>pacote NuGet 1.1.0]]
    T[Academia.Validacao.Testes<br/>xUnit]
    F[(SQLite<br/>academia.db)]

    I -- HTTP + JWT<br/>CORS --> C
    SV -- PackageReference --> L
    T -- ProjectReference --> L
    DB --> F
```

**Fluxo de uma requisição de escrita** (ex.: cadastrar aluno):

1. A página chama o `ApiService`, e o interceptor anexa `Authorization: Bearer <token>`.
2. O middleware JWT valida o token. Sem token, ou com token expirado, a API responde **401**.
3. O `AlunosController` recebe o DTO e chama `AlunoService.Criar`.
4. O Service valida com a biblioteca (`ValidadorAluno`), aplica as regras de negócio (CPF único, plano existe) e grava via `AcademiaContext`.
5. Se der erro, o Service lança `RegraNegocioException`, que vira **400** `{ "erros": [...] }`. Se der certo, a API responde **201** com `Location`.
6. A página mostra os erros ou volta para a lista.

## Entidades

```mermaid
erDiagram
    PLANO ||--o{ ALUNO : "tem"
    ALUNO ||--o{ MENSALIDADE : "gera"

    PLANO {
        int Id PK
        string Nome
        decimal ValorMensal
        int DuracaoMeses
    }
    ALUNO {
        int Id PK
        string Nome
        string Cpf UK
        string Email
        date DataNascimento
        string Telefone "migration 2"
        int PlanoId FK
    }
    MENSALIDADE {
        int Id PK
        int AlunoId FK
        decimal Valor
        date Vencimento
        date DataPagamento "nulo = em aberto"
    }
    USUARIO {
        int Id PK
        string Login UK
        string SenhaHash
    }
```

- **Plano → Aluno:** exclusão restrita. O Service bloqueia com uma mensagem antes do banco.
- **Aluno → Mensalidade:** exclusão em cascata.

## Decisões técnicas

- **Validação numa biblioteca separada:** as regras do domínio não dependem do framework web. Dá para testá-las isoladas (xUnit) e reutilizá-las em outro projeto pelo NuGet.
- **Biblioteca como pacote NuGet (não ProjectReference):** a API depende de uma versão fixa. A evolução 1.0.0 → 1.1.0 (MINOR, pelo SemVer) adicionou a regra de telefone sem quebrar quem usa a 1.0.0.
- **Controller fino + Service:** é o padrão de camadas. O Controller só traduz HTTP, e o Service concentra a regra de negócio. Os Services entram pelo construtor (injeção de dependência), o que facilita trocar e testar.
- **DTOs em vez de entidades na resposta:** evitam ciclos de serialização (Aluno ↔ Plano) e expõem só o necessário.
- **SQLite:** é um arquivo único, sem servidor para instalar, e os dados sobrevivem a reinícios.
- **Angular standalone e zoneless, com signals:** é o padrão atual do Angular. O estado das telas fica em `signal()`, que avisa o Angular quando precisa redesenhar.

## Padrões de projeto usados

| Padrão | Onde |
|--------|------|
| **Injeção de Dependência** | `Program.cs` registra os Services (`AddScoped`) e o `AcademiaContext`, e os Controllers os recebem pelo construtor |
| **Camada de Serviço (Service Layer)** | `Services/*Service.cs`: as regras de negócio ficam fora do Controller |
| **DTO (Data Transfer Object)** | `Modelos/Dtos.cs`: o que entra e sai da API é separado das entidades do banco |
| **Unit of Work / Repository** | o `AcademiaContext` (EF Core) agrupa as alterações e grava tudo em `SaveChangesAsync()` |
| **Filtro / Pipeline** | `RegraNegocioFiltro` (exceção → 400) no back; `tokenInterceptor` (anexa o JWT) e `authGuard` (protege as rotas) no front |

## Escolha dos frameworks

- **ASP.NET Core:** traz injeção de dependência, autenticação JWT e roteamento prontos, integra direto com o EF Core e é o framework da disciplina.
- **EF Core + SQLite:** com Code First o modelo vive no C#, e as migrations versionam a evolução do banco. O SQLite dispensa servidor.
- **Angular (standalone):** é um framework completo, com roteador, HttpClient, interceptors e guards nativos, e em TypeScript, então os tipos do front espelham os DTOs da API.

## Metodologia

O histórico do Git mostra uma entrega incremental, em que cada etapa funciona e é testada antes da próxima: biblioteca 1.0.0 → biblioteca 1.1.0 → API com a migration inicial → evolução (telefone + migration 2) → front-end → documentação.

Como éramos só em dois, a gente decidiu não usar um Scrum "completo", com sprint, daily e todas as cerimônias. Para um grupo desse tamanho e com o prazo da P2, isso ia tomar mais tempo organizando do que fazendo. Ficamos com algo mais próximo de um Kanban simples: uma lista do que faltava (biblioteca, testes, pacote, API, banco, login, front, documentação), e cada item só saía da lista quando estava funcionando e testado.

A ordem seguiu o mesmo caminho que fizemos em sala com o Cadastro de Alunos: primeiro as regras de validação e os testes, porque o resto do sistema depende delas, depois a API e o banco e por último o front. Isso ajudou bastante: quando chegamos no Angular, a API já estava pronta e testada, então qualquer erro na tela era do front e não "de algum lugar".

Um exemplo de onde essa forma de trabalhar fez diferença foi a regra de telefone. Ela surgiu no meio do desenvolvimento, e em vez de mexer na versão que a API já usava, publicamos a biblioteca como 1.1.0 e criamos uma migration nova. Foi aí que o SemVer e as migrations fizeram sentido na prática para a gente, porque deu para evoluir sem quebrar o que já funcionava.

O que faríamos diferente: começar o README e a documentação junto com o código, e não só no final. Mapear os requisitos R1 a R9 no fim deu mais trabalho do que se tivéssemos anotado conforme íamos fazendo.
