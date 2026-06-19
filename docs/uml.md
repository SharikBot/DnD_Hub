# UML-диаграммы DnD Character Manager

## Диаграмма классов (основные компоненты)

```mermaid
classDiagram
    direction TB

    class ICharacterService {
        <<interface>>
        +GetByIdAsync(id) CharacterDto
        +GetByUserIdAsync(userId) List
        +CreateAsync(dto) CharacterDto
        +UpdateAsync(id, dto) CharacterDto
        +DeleteAsync(id) bool
    }

    class CharacterService {
        -IUnitOfWork unitOfWork
        -ICharacterFactory factory
        -IMemoryCache cache
        -CreateCharacterValidator validator
        +CreateAsync(dto)
    }

    class ICharacterFactory {
        <<interface>>
        +Create(dto, scores) Character
    }

    class CharacterFactory {
        +Create(dto, scores) Character
    }

    class IAbilityScoreStrategy {
        <<interface>>
        +MethodName string
        +GenerateScores(requested) Dictionary
    }

    class StandardArrayStrategy
    class Roll4D6Strategy
    class PointBuyStrategy

    class ICharacterRepository {
        <<interface>>
        +GetByUserIdAsync(userId)
        +GetByIdWithDetailsAsync(id)
    }

    class CharacterRepository {
        +GetByIdWithDetailsAsync(id)
    }

    class IUnitOfWork {
        <<interface>>
        +Characters ICharacterRepository
        +SaveChangesAsync()
    }

    class Character {
        +Guid Id
        +string Name
        +int Level
        +int Strength
        +Race Race
        +CharacterClass CharacterClass
    }

    class CreateCharacterDto {
        +string Name
        +Guid UserId
        +AbilityScoreMethod AbilityScoreMethod
    }

    ICharacterService <|.. CharacterService
    ICharacterFactory <|.. CharacterFactory
    IAbilityScoreStrategy <|.. StandardArrayStrategy
    IAbilityScoreStrategy <|.. Roll4D6Strategy
    IAbilityScoreStrategy <|.. PointBuyStrategy
    ICharacterRepository <|.. CharacterRepository
    CharacterService --> IUnitOfWork
    CharacterService --> ICharacterFactory
    CharacterService --> IAbilityScoreStrategy : uses
    CharacterFactory ..> Character : creates
    CharacterRepository ..> Character : manages
    CharacterService ..> CreateCharacterDto
```

## Диаграмма последовательности: создание персонажа

```mermaid
sequenceDiagram
    autonumber
    actor User as Пользователь
    participant VM as CharacterCreatorViewModel
    participant API as CharactersController
    participant Svc as CharacterService
    participant Val as CreateCharacterValidator
    participant Strat as AbilityScoreStrategy
    participant Fac as CharacterFactory
    participant UoW as UnitOfWork
    participant Repo as CharacterRepository
    participant DB as PostgreSQL

    User->>VM: Заполнить мастер создания
    VM->>API: POST /api/characters
    API->>Svc: CreateAsync(dto)
    Svc->>Val: Validate(dto)
    alt Ошибки валидации
        Val-->>Svc: List errors
        Svc-->>API: ArgumentException
        API-->>VM: 400 Bad Request
    else Валидация успешна
        Val-->>Svc: пустой список
        Svc->>Strat: GenerateScores(abilityScores)
        Strat-->>Svc: Dictionary scores
        Svc->>Fac: Create(dto, scores)
        Fac-->>Svc: Character + Inventory
        Svc->>UoW: Characters.AddAsync
        UoW->>Repo: AddAsync
        Repo->>DB: INSERT characters, inventories
        Svc->>UoW: SaveChangesAsync
        Svc->>Repo: GetByIdWithDetailsAsync
        Repo->>DB: SELECT with JOIN
        DB-->>Repo: Character with Race, Class
        Repo-->>Svc: Character
        Svc-->>API: CharacterDto
        API-->>VM: 201 Created
        VM-->>User: Отображение листа персонажа
    end
```

## Диаграмма компонентов

```mermaid
flowchart LR
    subgraph DesktopApp["Desktop (WPF)"]
        Views[Views]
        ViewModels[ViewModels]
        Nav[NavigationService]
    end

    subgraph WebApi["API"]
        Controllers[Controllers]
        Middleware[Exception Middleware]
    end

    subgraph Business["Application"]
        CharSvc[CharacterService]
        MonSvc[MonsterService]
        RuleSvc[RuleService]
        PdfSvc[PdfService]
        Valid[Validators]
    end

    subgraph Data["Infrastructure"]
        Repos[Repositories]
        UoW[UnitOfWork]
        EF[AppDbContext]
        AI[GeminiAiService]
    end

    subgraph Domain["Core"]
        Entities[Entities]
        DTO[DTOs]
        IFaces[Interfaces]
        PAT[Factory / Strategy]
    end

    DB[(PostgreSQL)]
    Gemini[Google Gemini]

    Views --> ViewModels
    ViewModels --> Nav
    ViewModels --> CharSvc
    Controllers --> CharSvc
    Controllers --> MonSvc
    Controllers --> AI
    CharSvc --> Valid
    CharSvc --> PAT
    CharSvc --> Repos
    Repos --> UoW
    UoW --> EF
    EF --> DB
    AI --> Gemini
    Business --> Domain
    Data --> Domain
```

## Описание ключевых связей

- **ViewModel → Service**: MVVM не обращается к БД напрямую; все операции через `ICharacterService`.
- **Service → Strategy**: метод генерации характеристик выбирается по `AbilityScoreMethod` в DTO.
- **Service → Factory**: фабрика собирает `Character` и связанный `Inventory` из DTO и рассчитанных scores.
- **Repository → DbContext**: EF Core Fluent API конфигурирует связи 1:M и M:N.
