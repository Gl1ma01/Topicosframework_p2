# Academia — Trabalho P2 de Tópicos de Framework

Sistema full-stack para gestão de uma academia: planos, alunos e mensalidades, com relatório financeiro do mês.

**Integrantes:**
- Gabriel Lima Ceretta
- Rafael Ferreira Vetrenka

## Cenário

Uma academia vende **planos** (Mensal, Trimestral, Anual…). Cada **aluno** é matriculado em um plano e gera **mensalidades**. A recepção precisa cadastrar alunos, registrar pagamentos e saber quanto entrou no mês e quem está atrasado.

### Regras de validação (biblioteca `Academia.Validacao`)

| # | Entidade | Regra | Desde |
|---|----------|-------|-------|
| 1 | Aluno | Nome com 3 a 100 caracteres | 1.0.0 |
| 2 | Aluno | CPF válido (dígitos verificadores) | 1.0.0 |
| 3 | Aluno | E-mail em formato válido | 1.0.0 |
| 4 | Aluno | Idade mínima de 14 anos | 1.0.0 |
| 5 | Plano | Nome com pelo menos 3 caracteres | 1.0.0 |
| 6 | Plano | Valor mensal maior que zero | 1.0.0 |
| 7 | Plano | Duração entre 1 e 24 meses | 1.0.0 |
| 8 | Mensalidade | Valor maior que zero | 1.0.0 |
| 9 | Aluno | Telefone opcional; se informado, DDD + número (10 ou 11 dígitos) | **1.1.0** |

### Regras de negócio (Services da API)

- Ao matricular um aluno, a primeira mensalidade é gerada automaticamente com o valor do plano e vencimento em 10 dias (`AlunoService.Criar`).
- O CPF não pode se repetir entre alunos (`AlunoService.ValidarAsync`).
- Um plano com alunos matriculados não pode ser excluído (`PlanoService.Excluir`).
- A mesma mensalidade não pode ser paga duas vezes, e uma mensalidade paga não pode ser excluída (`MensalidadeService.Pagar` / `Excluir`).
- O status da mensalidade é calculado: **Paga**, **Atrasada** (vencida sem pagamento) ou **Em aberto**.

## Mapeamento das entidades

| Entidade | Propriedades | Relacionamentos |
|----------|--------------|-----------------|
| **Plano** | Id, Nome, ValorMensal, DuracaoMeses | 1 Plano → N Alunos |
| **Aluno** | Id, Nome, Cpf (único), Email, DataNascimento, Telefone (migration 2), PlanoId | N Alunos → 1 Plano; 1 Aluno → N Mensalidades |
| **Mensalidade** | Id, AlunoId, Valor, Vencimento, DataPagamento | N Mensalidades → 1 Aluno (exclusão em cascata) |
| **Usuario** | Id, Login (único), SenhaHash | — (login da API) |

## Onde cada requisito foi aplicado

| Req. | Onde | Detalhe |
|------|------|---------|
| **R1** Class Library | `backend/Academia.Validacao/` → `ValidadorAluno`, `ValidadorPlano`, `ValidadorMensalidade` | 9 regras, 3 entidades. Sem dependência de ASP.NET Core nem EF Core (ver `.csproj`) |
| **R2** xUnit | `backend/Academia.Validacao.Testes/` → `ValidadorAlunoTestes`, `ValidadorTelefoneTestes`, `ValidadorPlanoTestes`, `ValidadorMensalidadeTestes` | 25 casos, padrão Arrange-Act-Assert, vários `[Theory]` + `[InlineData]` (ex.: `CpfValido_VariosCpfs_RetornaEsperado`) |
| **R3** NuGet + SemVer | `backend/FeedNuGetLocal/` (1.0.0 e 1.1.0), `backend/nuget.config`, `AcademiaApi.csproj` → `<PackageReference Include="Academia.Validacao" Version="1.1.0" />` | 1.0.0 → 1.1.0 é **MINOR**: adiciona `ValidadorAluno.ValidarTelefone` sem mudar nem remover nada da 1.0.0, então quem usa a 1.0.0 atualiza sem quebrar |
| **R4** Web API + DI | `AcademiaApi/Program.cs` (`AddScoped<PlanoService>()` etc.), `Controllers/*` recebem os Services pelo construtor | Controllers só chamam o Service e devolvem a resposta; regras ficam em `Services/` |
| **R5** EF Core Code First | `Dados/AcademiaContext.cs` (SQLite, relacionamentos em `OnModelCreating`), `Migrations/` | Migrations `Inicial` e `AdicionaTelefoneAluno` (evolução do modelo) |
| **R6** CRUD + status HTTP | `PlanosController` e `AlunosController` (CRUD completo), `MensalidadesController` | 200 (GET), 201 + `Location` (`CreatedAtAction`), 204 (PUT/DELETE), 400 `{ "erros": [...] }` com as mensagens da biblioteca, 404 (id inexistente) |
| **R7** JWT | `AuthController.Login` → `AuthService.Login`; `[Authorize]` em todo POST/PUT/DELETE; `appsettings.json` → `Jwt:ExpiracaoMinutos` = 120 | Sem token → 401. `ClockSkew = TimeSpan.Zero` em `Program.cs` para expirar no tempo exato |
| **R8** Angular standalone | `frontend/src/app/paginas/` → `AlunosLista`, `AlunoForm` (cadastrar/editar), `AlunoDetalhe` (relacionamento aluno → mensalidades), `Planos`, `Relatorio`; `servicos/api.service.ts` (`HttpClient`) | Todos os dados vêm da API, sem mock |
| **R9** Integração | `paginas/login.ts`, `interceptors/token.interceptor.ts`, `guardas/auth.guard.ts`, CORS em `Program.cs` (`AddCors("Frontend")`) | Erros 400 aparecem na tela via `mensagensDeErro()` em `api.service.ts` |

**Relatório:** `GET /api/relatorios/financeiro?ano=2026&mes=10` (`RelatorioService.Financeiro`) devolve o previsto, o recebido e o em aberto do mês, além dos inadimplentes com os dias de atraso. A tela fica em **Relatório**.

**Nunca 500 para entrada do usuário:** dado inválido → 400 (biblioteca + `RegraNegocioException`), JSON malformado ou de tipo errado → 400 (`InvalidModelStateResponseFactory`), id inexistente → 404, id não numérico → 404 (`{id:int}`).

## Como executar do zero

### Requisitos (versões obrigatórias)

| Ferramenta | Versão |
|------------|--------|
| [.NET SDK](https://dotnet.microsoft.com/download) | **10.0** (o projeto usa `net10.0` e `.slnx`; .NET 8/9 não compilam) |
| [Node.js](https://nodejs.org) | **22.22+ ou 24.15+** (exigência do Angular 22) |
| Angular | 22 (vem pelo `npm install`; não precisa de CLI global) |
| dotnet-ef | 10.0.14 (vem pelo `dotnet tool restore`) |

Confira com `dotnet --version` e `node --version`.

```bash
# 1. Testes da biblioteca
cd backend
dotnet test

# 2. API (http://localhost:5080)
dotnet tool restore                 # instala o dotnet-ef do manifesto do projeto
cd AcademiaApi
dotnet ef database update
dotnet run

# 3. Front-end (outro terminal) → http://localhost:4200
cd frontend
npm install
npx ng serve
```

A API também aplica as migrations e o seed sozinha ao iniciar. O banco `academia.db` fica em `backend/AcademiaApi/`: os dados sobrevivem a um reinício, e apagar o arquivo recria o banco com os dados de exemplo.

### Login padrão

- **Usuário:** `admin`
- **Senha:** `admin123`

### Gerar uma nova versão da biblioteca

```bash
cd backend
# altere <Version> em Academia.Validacao/Academia.Validacao.csproj (ex.: 1.2.0)
dotnet pack Academia.Validacao -c Release -o FeedNuGetLocal
# atualize a versão no PackageReference de AcademiaApi.csproj
```

> A chave JWT em `appsettings.json` é só para desenvolvimento. Em produção ela viria de variável de ambiente.
