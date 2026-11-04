# Prototype for sagsbehandling på stedet

For sagsbehandleren, som har brug for at generere og håndtere små opgaver på stedet.
Man kan oprette, se, markere som færdig (og genåbne) samt slette todo-punkter. Data gemmes i hukommelsen.

## Teknologier
Applikationen består af en server og en klient.

Data gemmes in-memory, da sagsbehandleren i disse situationer ikke behøver at persistere data.

Backenden er bygget i .NET Core og konstrueret efter principperne inden for Clean Code.

Frontenden er lavet i React og TypeScript.

Kommunikation mellem klient og server sker via REST endpoints.

## Kom i gang

**Forudsætninger:** Hav .NET 10 og Node > 19 installeret.

### Mulighed 1: Én kommando (anbefalet til at prøve appen)

API'et serverer selv den byggede React-app, så alt kører fra én URL:

```bash
cd frontend/todo-web && npm install && npm run build && cd ../..
dotnet run --project backend/src/TodoApp.Api
```

| Hvad | URL |
|---|---|
| Applikationen | http://localhost:5080 |
| API-dokumentation (Scalar) | http://localhost:5080/scalar/v1 |
| OpenAPI-spec | http://localhost:5080/openapi/v1.json |

### Mulighed 2: Udviklingstilstand (hot reload)

```bash
# Terminal 1 – API
dotnet run --project backend/src/TodoApp.Api

# Terminal 2 – frontend
cd frontend/todo-web
npm install
npm run dev
```

Frontenden åbnes på http://localhost:5173.

### Tests

```bash
dotnet test backend
```

29 tests fordelt på tre niveauer: domæneregler (unit), use cases med en fake repository (unit) og hele HTTP-pipelinen via `WebApplicationFactory` (integration).

## Arkitektur

Repo'et er delt i to selvstændige moduler, ét per teknologi. Hvert modul har sit eget værktøj (`dotnet` / `npm`) og kan bygges, testes og åbnes i en IDE uafhængigt af det andet. Den eneste forbindelse er, at frontendens produktionsbuild lægges i API'ets `wwwroot` (én linje i `vite.config.ts`).

```
backend/
  TodoApp.slnx
  src/
    TodoApp.Domain/          Entiteten TodoItem med forretningsregler, Result/Error-typer. Ingen afhængigheder.
    TodoApp.Application/     Use cases (TodoService), ITodoRepository-interface, DTO'er.      → Domain
    TodoApp.Infrastructure/  EF Core DbContext, mapping og repository-implementation.       → Application
    TodoApp.Api/             Minimal API-endpoints, ProblemDetails, OpenAPI, DI-opsætning.  → Application, Infrastructure
  tests/
    TodoApp.Domain.Tests/  TodoApp.Application.Tests/  TodoApp.Api.Tests/
frontend/
  todo-web/                  React + TypeScript (Vite)
```

**Backenden** følger **Clean Architecture** med ét projekt per lag. Projekt-referencerne håndhæver afhængighedsreglen ved compile-tid: afhængigheder peger kun indad, og f.eks. kan Domain slet ikke se EF Core.

**Frontend:** Komponenterne er rent præsentationelle. Al state og alle API-kald ligger i hook'en `useTodos`, som får API-klienten injiceret. API-klienten er typet efter backendens DTO'er, og ingen komponent kalder `fetch` direkte.

## REST API

| Metode | Route | Beskrivelse | Svar |
|---|---|---|---|
| `GET` | `/api/todos` | Hent alle | `200` |
| `GET` | `/api/todos/{id}` | Hent ét punkt | `200` / `404` |
| `POST` | `/api/todos` | Opret, body: `{ "title": "..." }` | `201` + `Location` / `400` |
| `PATCH` | `/api/todos/{id}/complete` | Markér som færdig | `200` / `404` |
| `PATCH` | `/api/todos/{id}/reopen` | Markér som ikke færdig | `200` / `404` |
| `DELETE` | `/api/todos/{id}` | Slet | `204` / `404` |

Alle endpoints kan afprøves interaktivt i Scalar på `/scalar/v1`.

## Valg og afvigelser fra opgavebeskrivelsen

- **Data i hukommelsen via EF Core InMemory:** Opgaven kræver, at data gemmes i hukommelsen. I stedet for en simpel liste bruges EF Core InMemory-provideren bag et repository for at gøre databaseinteraktionen mere realistisk.
- **"Genåbn" er tilføjet:** Ud over at markere et punkt som færdigt kan det markeres som ikke færdigt igen (`/reopen`), da dette er blevet vurderet som et must-have krav.
- **PATCH returnerer `200` med det opdaterede punkt** i stedet for `204`, så frontenden ikke behøver et ekstra kald for at hente den nye tilstand.
- **Fravalgt:** autentificering, paginering og redigering af titel. Det ligger uden for opgavens scope.
- **Arkitekturen er bevidst mere kompleks end berettiget.** Dette er for at demonstrere en skalerbar arkitektur.

## Viderearbejde
Som udgangspunkt tiltænkes denne applikation som værende en basisapplikation for sagsbehandling, hvor den kan bruges som et samtaleværktøj til at finde ud af, hvad brugerens behov ellers kunne være, og naturligvis demonstrere mine tekniske kompetencer.

Leger vi videre med, at denne applikation skal være produktionsklar i en kommunal kontekst, udestår der umiddelbart:
- Login og brugerstyring: Integration med auth-system, fx MitID Erhverv, og autentificering for interfacelaget i backenden.
- Sporbarhed: Da der kan være behov for aktindsigt med mere, skal sagsaktivitet kunne dokumenteres.
- Tilgængelighed: Brugergrænsefladen skal opfylde WCAG 2.1 niveau AA.
- GDPR: Fritekstfelterne kan være problematiske ift. personfølsomme data. Her kan et design med fx sagsnummer til andet system løse det for at reducere risikoen for personfølsomme oplysninger.
- Drift: Der skal opsættes CI/CD-processer for at drifte og vedligeholde applikationen.

Herudover skal det undersøges, hvordan en in-memory applikation kan følge krav om journalisering: skal data arkiveres ved et eksternt system? Her er løsningen nok blot at gøre databasen persistent.

