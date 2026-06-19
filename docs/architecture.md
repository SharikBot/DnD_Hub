# Архитектура DnD Character Manager

Проект следует **Clean Architecture** с разделением на независимые слои и инверсией зависимостей (зависимости направлены внутрь, к Core).

## Слои

```mermaid
flowchart TB
    subgraph Presentation["Presentation Layer"]
        Desktop["DnDCharacterManager.Desktop<br/>WPF + MVVM"]
        Api["DnDCharacterManager.Api<br/>ASP.NET Core Web API"]
    end

    subgraph Application["Application Layer"]
        Services["Services<br/>CharacterService, MonsterService, RuleService"]
        Validators["Validators"]
        Pdf["CharacterPdfService"]
    end

    subgraph Infrastructure["Infrastructure Layer"]
        Repos["Repositories + UnitOfWork"]
        DbContext["AppDbContext (EF Core)"]
        Gemini["GeminiAiService"]
    end

    subgraph Core["Core Layer"]
        Entities["Entities"]
        DTOs["DTOs"]
        Interfaces["Interfaces"]
        Patterns["Factory, Strategy"]
    end

    subgraph External["External Systems"]
        PG[("PostgreSQL")]
        GeminiAPI["Google Gemini API"]
    end

    Desktop --> Services
    Api --> Services
    Services --> Interfaces
    Services --> Patterns
    Validators --> DTOs
    Repos --> Interfaces
    Repos --> DbContext
    DbContext --> PG
    Gemini --> GeminiAPI
    Infrastructure --> Core
    Application --> Core
    Presentation --> Application
```

## Принципы

| Принцип | Реализация |
|---------|------------|
| Dependency Inversion | Core определяет `ICharacterRepository`, `ICharacterService`; Infrastructure и Application реализуют |
| Separation of Concerns | UI не содержит бизнес-логики; ViewModel делегирует сервисам |
| Mobile-ready | Общий API + DTO позволяют подключить MAUI / Flutter без изменения Core |
| Testability | Сервисы зависят от интерфейсов; тесты используют InMemory EF и моки |

## Поток данных (CRUD персонажа)

```mermaid
sequenceDiagram
    participant C as Client (Desktop/API)
    participant S as CharacterService
    participant V as CreateCharacterValidator
    participant F as CharacterFactory
    participant R as CharacterRepository
    participant DB as PostgreSQL

    C->>S: CreateCharacterDto
    S->>V: Validate(dto)
    V-->>S: errors / ok
    S->>F: Create(dto, abilityScores)
    F-->>S: Character entity
    S->>R: AddAsync(character)
    R->>DB: INSERT
    S-->>C: CharacterDto
```

## Зависимости проектов

```
Desktop  → Application, Core
Api      → Application, Infrastructure
Application → Core
Infrastructure → Core
Tests    → Application, Infrastructure, Core
```

Core не ссылается ни на один внешний слой.
