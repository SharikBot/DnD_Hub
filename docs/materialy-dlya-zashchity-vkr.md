# Материалы для защиты ВКР: DnD Character Manager

> **Назначение документа:** подготовка к устной защите дипломного проекта. Здесь собрано, **что использовалось**, **как реализовано**, **какие конструкции языка C# применены** и **как отвечать на типичные вопросы комиссии**.

**Репозиторий:** https://github.com/SharikBot/DnD_Hub  
**Платформа:** .NET 10, Windows (WPF)  
**Тип проекта:** desktop-приложение + REST API для управления персонажами Dungeons & Dragons 5e

---

## Содержание

1. [Тема и цель работы](#1-тема-и-цель-работы)
2. [Как я строил проект — этапы разработки](#2-как-я-строил-проект--этапы-разработки)
3. [Структура solution и назначение проектов](#3-структура-solution-и-назначение-проектов)
4. [Архитектура Clean Architecture](#4-архитектура-clean-architecture)
5. [Технологический стек](#5-технологический-стек)
6. [Средства языка C# и операторы](#6-средства-языка-c-и-операторы)
7. [Паттерны проектирования](#7-паттерны-проектирования)
8. [База данных и Entity Framework Core](#8-база-данных-и-entity-framework-core)
9. [REST API — контроллеры и endpoints](#9-rest-api--контроллеры-и-endpoints)
10. [Desktop-клиент WPF и MVVM](#10-desktop-клиент-wpf-и-mvvm)
11. [Генерация PDF](#11-генерация-pdf)
12. [Интеграция Google Gemini AI](#12-интеграция-google-gemini-ai)
13. [Валидация данных](#13-валидация-данных)
14. [Тестирование](#14-тестирование)
15. [Сборка, установщик и развёртывание](#15-сборка-установщик-и-развёртывание)
16. [CI/CD на GitHub Actions](#16-cicd-на-github-actions)
17. [Демонстрационный сценарий на защите](#17-демонстрационный-сценарий-на-защите)
18. [Типичные вопросы комиссии и ответы](#18-типичные-вопросы-комиссии-и-ответы)

---

## 1. Тема и цель работы

**Тема (формулировка для защиты):**  
Разработка desktop-приложения для создания, хранения и управления персонажами настольной ролевой игры Dungeons & Dragons 5e с REST API, экспортом в PDF и генерацией игрового контента через облачный AI-сервис.

**Цели, которые я ставил:**

| Цель | Как достигнута |
|------|----------------|
| Удобный интерфейс для игрока | WPF-клиент с пошаговым мастером (9 шагов), лист персонажа, бестиарий, справочник правил |
| Разделение логики и представления | Clean Architecture: Core → Application → Infrastructure → Api/Desktop |
| Возможность расширения (mobile) | Общий REST API и DTO — можно подключить MAUI/Flutter без изменения доменного слоя |
| Соответствие правилам D&D 5e | Расчёт модификаторов, HP, AC, proficiency bonus, три метода генерации характеристик |
| Надёжность | 47 автоматических тестов (unit + integration), глобальная обработка ошибок API |
| Распространение | Inno Setup установщик + Launcher, который поднимает API и Desktop одним ярлыком |

**Предметная область:** D&D 5e — персонаж имеет расу, класс, предысторию, 6 характеристик (STR, DEX, CON, INT, WIS, CHA), навыки, заклинания, черты, инвентарь, мировоззрение, биографию.

---

## 2. Как я строил проект — этапы разработки

Я разбивал работу на 10 логических этапов (подробнее — `docs/roadmap.md`):

| № | Этап | Что сделано |
|---|------|-------------|
| 1 | Архитектура | Спроектировал слои Clean Architecture, нарисовал UML и ER-диаграммы |
| 2 | Структура solution | Создал 6 проектов + тесты, настроил DI |
| 3 | UI Desktop | MainWindow, sidebar, Material Design, 7 экранов, MVVM |
| 4 | Модели | 15 сущностей, DTO, enums |
| 5 | Сервисы | CharacterService, Factory, Strategy, валидатор |
| 6 | БД | EF Core, Fluent API, миграция InitialCreate, seed-данные |
| 7 | AI | GeminiAiService, AiController, UI генератора |
| 8 | PDF | CharacterPdfService на QuestPDF |
| 9 | Тесты | xUnit, InMemory EF, WebApplicationFactory |
| 10 | Polishing | Snackbar, установщик, CI, документация |

**Порядок реализации функционала (от фундамента к UI):**

1. Сначала **Core** — сущности и интерфейсы (контракты).
2. Затем **Infrastructure** — репозитории, DbContext, миграции.
3. **Application** — бизнес-логика (CharacterService и др.).
4. **Api** — REST-контроллеры, middleware, Swagger.
5. **Desktop** — ViewModels и Views, HTTP-клиент к API.
6. **Tests** — параллельно с сервисами, финально integration tests.
7. **Launcher + installer** — упаковка для пользователя.

---

## 3. Структура solution и назначение проектов

```
DnDCharacterManager/
├── src/
│   ├── DnDCharacterManager.Core           — домен (без зависимостей)
│   ├── DnDCharacterManager.Application  — бизнес-логика
│   ├── DnDCharacterManager.Infrastructure — БД, Gemini HTTP
│   ├── DnDCharacterManager.Api          — ASP.NET Core Web API
│   ├── DnDCharacterManager.Desktop      — WPF MVVM клиент
│   └── DnDCharacterManager.Launcher     — exe-лаунчер (API + Desktop)
├── tests/
│   └── DnDCharacterManager.Tests        — xUnit
├── docs/                                — архитектура, UML, ER, этот документ
└── installer/                           — build-installer.ps1, setup.iss
```

| Проект | Зачем нужен | Зависит от |
|--------|-------------|------------|
| **Core** | Сущности, DTO, интерфейсы, паттерны Factory/Strategy/Singleton | — |
| **Application** | Сервисы, валидация, PDF | Core |
| **Infrastructure** | EF Core, репозитории, Gemini | Core |
| **Api** | HTTP API, Swagger, Serilog | Application, Infrastructure, Core |
| **Desktop** | WPF UI, MVVM | Core, Application (не Infrastructure!) |
| **Launcher** | Запуск двух процессов | — |
| **Tests** | Автотесты | Api, Application, Infrastructure, Core |

**Почему Desktop не ссылается на Infrastructure:** клиент — «тонкий»; он общается с сервером по HTTP. Бизнес-логика и БД остаются на API. Это упрощает тестирование UI и позволяет позже сделать мобильный клиент на тот же API.

---

## 4. Архитектура Clean Architecture

### 4.1. Принцип инверсии зависимостей

- **Core** объявляет интерфейсы: `ICharacterRepository`, `ICharacterService`, `IGeminiAiService`.
- **Infrastructure** и **Application** реализуют эти интерфейсы.
- Внешние слои зависят от абстракций, а не от конкретных классов.

### 4.2. Направление зависимостей

```
Desktop  → Application, Core
Api      → Application, Infrastructure, Core
Application → Core
Infrastructure → Core
```

Core **ни от кого не зависит** — это «ядро» системы.

### 4.3. Поток данных при создании персонажа

```
Пользователь → CharacterCreatorViewModel → POST /api/characters
  → CharactersController → CharacterService
    → CreateCharacterValidator (проверка DTO)
    → IAbilityScoreStrategy (генерация STR–CHA)
    → CharacterFactory (создание Entity)
    → CharacterRepository → AppDbContext → SQLite/PostgreSQL
  ← CharacterDto ← 201 Created
```

### 4.4. SOLID в проекте

| Принцип | Пример в проекте |
|---------|------------------|
| **S** — Single Responsibility | `CharacterPdfService` только генерирует PDF; `CharacterService` — CRUD и оркестрация |
| **O** — Open/Closed | Новый метод характеристик = новый класс Strategy, без изменения CharacterService |
| **L** — Liskov Substitution | Любая `IAbilityScoreStrategy` взаимозаменяема |
| **I** — Interface Segregation | Отдельные интерфейсы: `ICharacterRepository`, `IMonsterRepository`, `IRuleRepository` |
| **D** — Dependency Inversion | CharacterService зависит от `IUnitOfWork`, а не от `AppDbContext` |

---

## 5. Технологический стек

| Категория | Технология | Версия / примечание |
|-----------|------------|---------------------|
| Язык | C# | 13 (.NET 10) |
| Backend | ASP.NET Core Web API | Minimal hosting + Controllers |
| ORM | Entity Framework Core | 10.0.9 |
| БД (dev/installer) | SQLite | файл `dnd_dev.db` / `dnd_app.db` |
| БД (prod) | PostgreSQL | через Npgsql |
| Desktop UI | WPF | net10.0-windows |
| MVVM | CommunityToolkit.Mvvm | 8.4.2 — `[ObservableProperty]`, `[RelayCommand]` |
| UI-тема | MaterialDesignInXaml | Dark theme, Amber + DeepOrange |
| PDF | QuestPDF | fluent API, Community License |
| AI | Google Gemini REST API | модель gemini-2.0-flash |
| Логирование | Serilog | Console (API), File (Desktop) |
| Кэш | IMemoryCache | TTL 5 минут для CharacterDto |
| HTTP-клиент | HttpClient + IHttpClientFactory | Gemini, Desktop ApiClient |
| Тесты | xUnit, EF InMemory, WebApplicationFactory | 47 тестов |
| CI | GitHub Actions | restore → build → test |
| Установщик | Inno Setup 6 | русский язык, ярлыки |

---

## 6. Средства языка C# и операторы

Ниже — конструкции C#, которые я **осознанно применял** в проекте, с пояснением **зачем**.

### 6.1. Асинхронность: `async` / `await`

**Где:** все сервисы, контроллеры, ViewModels, репозитории, Gemini, Launcher.

**Зачем:** операции с БД и HTTP не блокируют поток UI и сервера.

**Пример** (`CharacterService.GetByIdAsync`):
```csharp
var character = await _unitOfWork.Characters.GetByIdWithDetailsAsync(id, cancellationToken);
```

**`CancellationToken`** — стандартный параметр для отмены длительных операций (закрытие приложения, timeout).

---

### 6.2. LINQ — язык интегрированных запросов

**Операторы и методы, которые используются:**

| LINQ | Назначение | Пример |
|------|------------|--------|
| `.Select()` | Проекция Entity → DTO | `characters.Select(MapToListItem).ToList()` |
| `.Where()` | Фильтрация | поиск монстров, правил |
| `.OrderBy()` / `.OrderByDescending()` | Сортировка | бестиарий |
| `.FirstOrDefault()` | Первый элемент или null | поиск в коллекциях |
| `.ToList()` / `.ToArray()` | Материализация запроса | возврат списков из сервиса |
| `.Distinct()` | Уникальные значения | — |
| `.Any()` | Есть ли элементы | проверки коллекций |
| `.Sum()` | Сумма | Point Buy — подсчёт очков |
| `Enumerable.Range()` | Диапазон чисел | 4 броска d6 в Roll4D6Strategy |

**Пример** (`Roll4D6Strategy`):
```csharp
var rolls = Enumerable.Range(0, 4)
    .Select(_ => _random.Next(1, 7))
    .OrderByDescending(r => r)
    .ToArray();
return rolls[0] + rolls[1] + rolls[2]; // сумма трёх лучших
```

---

### 6.3. Generics (обобщения)

**`Repository<T> where T : BaseEntity`** — один базовый репозиторий для всех сущностей с GUID-ключом:

```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    public async Task<T?> GetByIdAsync(Guid id, ...) =>
        await _dbSet.FindAsync([id], cancellationToken);
}
```

**`NavigateTo<TViewModel>()`** — типобезопасная навигация в WPF без строковых имён.

**`Enum.GetValues<AbilityType>()`** — generic-метод для перечисления всех 6 характеристик.

---

### 6.4. Nullable reference types (`string?`, `Character?`)

Во всех `.csproj` включено `<Nullable>enable</Nullable>`.

- `string? Backstory` — биография может отсутствовать.
- `CharacterDto?` — метод может вернуть null, если персонаж не найден.
- Оператор **`??`** (null-coalescing): `race ?? throw new ArgumentException(...)`.
- Оператор **`?.`** (null-conditional): `dto.PortraitPath?.Trim()`.

---

### 6.5. Pattern matching (сопоставление с образцом)

**`is null` / `is not null`:**
```csharp
if (character is null)
    return null;
```

**Property pattern** (`Roll4D6Strategy`):
```csharp
if (requestedScores is { Count: > 0 })
    throw new ArgumentException(...);
```

**Switch expression** — выбор стратегии характеристик:
```csharp
IAbilityScoreStrategy strategy = dto.AbilityScoreMethod switch
{
    AbilityScoreMethod.Roll4D6 => new Roll4D6Strategy(),
    AbilityScoreMethod.PointBuy => new PointBuyStrategy(),
    _ => new StandardArrayStrategy()
};
```

**Switch expression** — построение AI-промпта по типу контента (`GeminiAiService.BuildPrompt`):
```csharp
var instruction = contentType switch
{
    "backstory" => "Напиши биографию...",
    "name" => "Предложи 5 имён...",
    _ => "Ответь по правилам D&D 5e..."
};
```

---

### 6.6. Операторы C# (справочник для комиссии)

| Оператор | Название | Где используется |
|----------|----------|------------------|
| `=` | Присваивание | везде |
| `==`, `!=` | Сравнение | условия, валидация |
| `<`, `>`, `<=`, `>=` | Сравнение чисел | характеристики, HP |
| `+`, `-`, `*`, `/` | Арифметика | модификатор `(score - 10) / 2`, HP |
| `%` | Остаток | proficiency: `2 + (level - 1) / 4` |
| `&&`, `\|\|` | Логические И/ИЛИ | составные условия |
| `!` | Логическое НЕ | `if (!response.IsSuccessStatusCode)` |
| `??` | Null-coalescing | значения по умолчанию |
| `??=` | Null-coalescing assignment | `_instance ??= new AppConfiguration()` |
| `?.` | Null-conditional | безопасный доступ к свойствам |
| `?.` + `[]` | Index from end | — |
| `=>` | Lambda / expression body | `private static int Modifier(int score) => (score - 10) / 2;` |
| `?.` | Conditional access | `dto.Backstory?.Trim()` |
| `throw` | Исключение | валидация, ошибки API |
| `nameof()` | Имя параметра | `nameof(requestedScores)` в ArgumentException |
| `is` | Pattern matching | проверка типов и null |
| `as` | Безопасное приведение | редко |
| `new()` | Target-typed new | `new List<CharacterTrait>()` |
| `[...]` | Collection expressions | `MenuItems = [ new NavigationMenuItem {...}, ... ]` |

---

### 6.7. Другие конструкции C#

| Конструкция | Применение |
|-------------|------------|
| **Partial class + source generators** | ViewModels: `[ObservableProperty]` генерирует свойства |
| **Raw string literals** `$"""..."""` | Многострочный промпт для Gemini |
| **Exception filters** `catch (ex) when (...)` | Retry в GeminiAiService — ловим только если есть ещё попытки |
| **`ArgumentNullException.ThrowIfNull(dto)`** | Guard clause в Factory |
| **`Math.Clamp(value, min, max)`** | Ограничение текущих HP при сохранении листа |
| **`lock` + тип `Lock`** | Потокобезопасный Singleton AppConfiguration |
| **`IAsyncDisposable`** | UnitOfWork.DisposeAsync — освобождение транзакции |
| **Implicit usings** | Глобальные using в .csproj |
| **Top-level statements** | Program.cs в Api и Launcher |
| **`public partial class Program`** | Для WebApplicationFactory в тестах |

**Records не использовал** — DTO реализованы обычными классами для совместимости с JSON-сериализацией и EF.

---

### 6.8. Формулы D&D 5e в коде

Эти формулы я реализовал в `CharacterService` и на Desktop (`AbilityHelper`):

| Показатель | Формула | Код |
|------------|---------|-----|
| Модификатор характеристики | (score − 10) / 2 (целочисленное деление) | `(score - 10) / 2` |
| Proficiency Bonus | 2 + (level − 1) / 4 | `2 + (level - 1) / 4` |
| Standard Array | фиксированный набор [15,14,13,12,10,8] | `StandardArrayStrategy` |
| 4d6 drop lowest | 4×d6, отбросить минимальный, сумма 3 оставшихся | `Roll4D6Strategy` |
| Point Buy | 27 очков, таблица стоимости 8–15 | `PointBuyStrategy` |

---

## 7. Паттерны проектирования

### 7.1. Repository (Репозиторий)

**Суть:** абстракция доступа к данным. Сервис не знает про SQL — только про `ICharacterRepository`.

**Классы:**
- `IRepository<T>` — базовый CRUD
- `CharacterRepository` — `GetByUserIdAsync`, `GetByIdWithDetailsAsync` (Eager Loading через `.Include()`)
- `Repository<T>` — общая реализация на EF Core

**Зачем на защите:** отделяю доменную логику от способа хранения; можно заменить SQLite на PostgreSQL без изменения CharacterService.

---

### 7.2. Unit of Work (Единица работы)

**Суть:** одна транзакция на несколько операций с репозиториями.

**Класс:** `UnitOfWork` — свойства `Characters`, `Monsters`, `Rules` + `SaveChangesAsync()`.

**Пример использования:**
```csharp
await _unitOfWork.Characters.AddAsync(character, cancellationToken);
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

**Методы транзакций:** `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`.

---

### 7.3. Factory (Фабрика)

**Суть:** централизованное создание сложного объекта `Character`.

**Класс:** `CharacterFactory.Create(dto, abilityScores)`:
- создаёт `Character` с GUID
- заполняет 6 характеристик из словаря
- создаёт связанный `Inventory`
- рассчитывает начальные MaxHitPoints и ArmorClass

**Зачем:** логика «как собрать персонажа из DTO» в одном месте, а не размазана по сервису.

---

### 7.4. Strategy (Стратегия)

**Суть:** три алгоритма генерации характеристик — три класса, один интерфейс.

| Класс | Метод D&D | Поведение |
|-------|-----------|-----------|
| `StandardArrayStrategy` | Standard Array | Фиксированные значения или пользовательское распределение |
| `Roll4D6Strategy` | 4d6 drop lowest | Случайные броски |
| `PointBuyStrategy` | Point Buy | 27 очков, валидация таблицы стоимости |

**Интерфейс:** `IAbilityScoreStrategy.GenerateScores(Dictionary<AbilityType, int>? requestedScores)`

**Выбор стратегии:** switch expression в `CharacterService.ResolveAbilityScores()` — это **композиция Strategy + switch**, без DI для стратегий (они stateless, создаются на лету).

---

### 7.5. Singleton (Одиночка)

**Класс:** `AppConfiguration` — единые настройки Desktop-клиента (URL API, ключ Gemini).

**Реализация:**
- приватный конструктор
- статическое свойство `Instance` с double-checked locking через `lock (SyncRoot)`
- метод `Reset()` для тестов и сброса настроек

**Используют:** `ApiClient`, `SettingsViewModel`.

**На защите:** это **GoF Singleton** для глобальной конфигурации клиента; на сервере вместо этого — `IConfiguration` и DI.

---

### 7.6. MVVM + Command (Presentation pattern)

**Не GoF, но обязателен для WPF:**

- **Model** — DTO и модели UI (`NavigationMenuItem`, `GlossaryTerm`)
- **View** — XAML (`CharacterCreatorView.xaml`)
- **ViewModel** — `CharacterCreatorViewModel` с `[RelayCommand]` и `[ObservableProperty]`

**Data Binding:** `{Binding Races}`, `Command="{Binding NextStepCommand}"`.

**View Locator:** в `MainWindow.xaml` DataTemplate сопоставляет тип ViewModel → View автоматически.

---

### 7.7. Dependency Injection (Внедрение зависимостей)

**Extension methods:**
- `AddApplication()` — сервисы Application layer
- `AddInfrastructure(configuration)` — DbContext, реpositories, HttpClient для Gemini

**Lifetime (время жизни):**

| Lifetime | Регистрации |
|----------|-------------|
| **Scoped** | CharacterService, repositories, UnitOfWork, validators |
| **Singleton** | IMemoryCache, NavigationService, ApiClient, MainWindow |
| **Transient** | ViewModels (новый экземпляр при каждой навигации) |

---

### 7.8. Middleware (Pipeline)

**`GlobalExceptionMiddleware`** — перехватывает необработанные исключения API и возвращает JSON с кодом ошибки вместо stack trace клиенту.

---

## 8. База данных и Entity Framework Core

### 8.1. Сущности (15 классов)

**Основные:**
- `User`, `Character`, `Race`, `CharacterClass`, `Background`
- `Monster`, `Rule`
- `Inventory`, `InventoryItem`
- `Trait`, `Skill`, `Spell`

**Связующие (M:N):**
- `CharacterTrait`, `CharacterSkill`, `CharacterSpell`

**Базовый класс `BaseEntity`:**
```csharp
public Guid Id { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime UpdatedAt { get; set; }
```

`AppDbContext.SaveChangesAsync` автоматически проставляет `CreatedAt`/`UpdatedAt`.

### 8.2. Связи Character

```
Character ──N:1── User, Race, CharacterClass, Background
Character ──1:1── Inventory
Character ──M:N── Trait, Skill, Spell (через join-таблицы)
```

### 8.3. Fluent API конфигурации

Файлы в `Infrastructure/Data/Configurations/`:
- `CharacterConfiguration` — FK, cascade delete, индексы по `UserId` и `Name`
- `JoinEntityConfigurations` — составные ключи для M:N
- Enum `AlignmentType` хранится как **string** в БД

### 8.4. Миграции

- Одна миграция: `20260619150327_InitialCreate`
- Применение: `await dbContext.Database.MigrateAsync()` при старте API
- Seed: `DevDataSeeder` — расы, классы, монстры, правила, демо-пользователь

### 8.5. Переключение провайдера БД

В `DependencyInjection.cs` по ключу `Database:Provider`:
- `"Sqlite"` → `UseSqlite(...)` — разработка и установщик
- `"PostgreSQL"` → `UseNpgsql(...)` — production

### 8.6. Eager Loading

`GetByIdWithDetailsAsync` использует `.Include()` для загрузки Race, Class, Background, Traits, Spells, Skills, Inventory — один запрос с JOIN вместо N+1.

---

## 9. REST API — контроллеры и endpoints

**Базовый URL:** `http://localhost:5049`  
**Swagger (Development):** `/swagger`

### CharactersController — `/api/characters`

| HTTP | Маршрут | Действие |
|------|---------|----------|
| GET | `{id}` | Получить персонажа с деталями |
| GET | `user/{userId}` | Список персонажей пользователя |
| POST | `/` | Создать → 201 Created |
| PUT | `{id}` | Обновить |
| DELETE | `{id}` | Удалить → 204 |
| GET | `{id}/pdf` | Скачать PDF |
| PATCH | `{id}/sheet` | Частичное обновление листа (HP, имя, backstory) |

### MonstersController — `/api/monsters`

GET `/`, GET `{id}`, GET `search?name=`, GET `type/{creatureType}`, POST, PUT, DELETE

### RulesController — `/api/rules`

GET `/`, GET `{id}`, GET `category/{category}`, GET `search?title=`, POST, PUT, DELETE

### ReferenceController — `/api/reference`

GET `races`, `classes`, `backgrounds`, `traits`, `spells` — справочники для мастера создания

### AiController — `/api/ai`

POST `generate`, `generate/backstory`, `generate/npc`, `generate/encounter`

**Cross-cutting:**
- CORS `DefaultCors` — AllowAnyOrigin (для dev и desktop-клиента)
- Serilog — логирование каждого HTTP-запроса
- `GlobalExceptionMiddleware` — единый формат ошибок

---

## 10. Desktop-клиент WPF и MVVM

### 10.1. Экраны приложения

| Экран | ViewModel | Функции |
|-------|-----------|---------|
| Главное меню | MainViewModel | Sidebar, навигация, Snackbar |
| Создание персонажа | CharacterCreatorViewModel | 9 шагов мастера |
| Список персонажей | CharactersListViewModel | Поиск, открытие, удаление |
| Лист персонажа | CharacterSheetViewModel | Редактирование, PDF, HP ± |
| Бестиарий | BestiaryViewModel | Поиск, фильтр по типу, сортировка |
| Справочник правил | RulebookViewModel | Поиск, категории, глоссарий с кликабельными терминами |
| AI-генератор | AiGeneratorViewModel | 9 типов контента |
| Настройки | SettingsViewModel | URL API, ключ Gemini |

### 10.2. Мастер создания — 9 шагов (`CreatorStep`)

1. **Race** — выбор расы (карточки)
2. **Class** — выбор класса
3. **Background** — предыстория
4. **Traits** — черты (feats)
5. **Spells** — заклинания
6. **Equipment** — снаряжение
7. **Abilities** — характеристики (Standard Array / 4d6 / Point Buy)
8. **Alignment** — мировоззрение
9. **Portrait** — имя, биография, портрет

**Команды:** `NextStepCommand`, `PreviousStepCommand`, `SaveCharacterCommand`, `SelectCardCommand`.

### 10.3. Привязка данных (Data Binding)

- **OneWay:** отображение списков `{Binding Races}`
- **TwoWay:** поля ввода `{Binding CharacterName, Mode=TwoWay}`
- **Commands:** кнопки через `{Binding SaveCommand}`
- **DataTriggers:** видимость блоков по `CurrentStep`

### 10.4. ApiClient

Единый HTTP-клиент Desktop → API:
- `GetRacesAsync()`, `CreateCharacterAsync(dto)`, `DownloadCharacterPdfAsync(id)` и т.д.
- Базовый URL из `AppConfiguration.Instance.ApiBaseUrl`

### 10.5. Демо-пользователь

`Guid.Parse("00000000-0000-0000-0000-000000000001")` — используется в Desktop для привязки персонажей без полноценной авторизации (упрощение для ВКР).

---

## 11. Генерация PDF

**Библиотека:** QuestPDF — декларативное описание документа (fluent API).

**Сервис:** `CharacterPdfService.GenerateCharacterSheetAsync(characterId)`

**Содержимое PDF:**
- Шапка: портрет, имя, уровень, класс, раса, alignment
- 6 характеристик с модификаторами
- Блок боя: AC, HP, Speed, Proficiency, Initiative
- Навыки, saving throws, черты, заклинания, инвентарь, биография

**API:** `GET /api/characters/{id}/pdf` → `File(bytes, "application/pdf", filename)`

**Desktop:** Preview (открытие временного файла) и Download (SaveFileDialog).

---

## 12. Интеграция Google Gemini AI

### 12.1. Архитектура

Desktop → API (`POST /api/ai/generate`) → `GeminiAiService` → HTTPS → Google Generative Language API

**Ключ API хранится на сервере** в `appsettings.json`, не в клиенте (безопаснее).

### 12.2. HTTP-запрос

```
POST https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=...
Body: { "contents": [{ "parts": [{ "text": "промпт" }] }], "generationConfig": { "maxOutputTokens": 1024 } }
```

### 12.3. Типы генерации (9 типов)

| ContentType | Что генерирует |
|-------------|----------------|
| backstory | Биография персонажа |
| name | 5 имён с значениями |
| personality | Черты, идеалы, привязанности |
| build | Билд характеристик |
| class-recommendation | Рекомендация классов |
| spell-recommendation | Рекомендация заклинаний |
| npc | Описание NPC |
| encounter | Сцена боя/социальная |
| quest | Квест |

### 12.4. Надёжность

- До **3 попыток** при ошибках 429, 502, 503, 504
- Задержка между попытками: `RetryDelay * attempt` (2s, 4s, 6s)
- Exception filter: `catch (Exception ex) when (attempt < MaxRetryAttempts)`

---

## 13. Валидация данных

**FluentValidation не использовал** — написал свой класс `CreateCharacterValidator`.

**Правила:**
- DTO не null
- `Name` — обязателен, max 100 символов
- `UserId`, `RaceId`, `CharacterClassId`, `BackgroundId` — не `Guid.Empty`

**Вызов:** в начале `CharacterService.CreateAsync` / `UpdateAsync` — при ошибках `ArgumentException` → API возвращает 400.

**Дополнительно:** `PointBuyStrategy.ValidateScores` — проверка бюджета 27 очков; `StandardArrayStrategy` — проверка набора значений.

---

## 14. Тестирование

**Всего: 47 тестов, все проходят.**

### 14.1. Unit-тесты (изолированные)

| Файл | Что проверяет |
|------|---------------|
| `CharacterServiceTests` | CRUD, кэш, ошибки валидации — EF **InMemory** |
| `CharacterFactoryTests` | Создание персонажа, HP, AC, Inventory, null guards |
| `AbilityScoreStrategyTests` | 3 стратегии; `[Theory]` + `[InlineData]` для Point Buy |
| `CreateCharacterValidatorTests` | Все правила валидации (9 тестов) |
| `CharacterRepositoryTests` | Add, GetByUserId, GetByIdWithDetails |

**Вспомогательные классы:**
- `TestDataSeeder` — тестовые данные
- `SequenceRandom` — детерминированный Random для 4d6 (предсказуемые тесты)

### 14.2. Integration-тесты (через HTTP)

**`DndWebApplicationFactory`** — поднимает API in-process с env `Testing` и SQLite file DB.

**`ApiControllersIntegrationTests`** — 6 тестов:
- справочники races/classes
- monsters, rules search
- полный CRUD цикл персонажа
- экспорт PDF (проверка magic byte `%PDF` и Content-Type)

### 14.3. Покрытие

Подключён `coverlet.collector` для сбора code coverage в CI.

**Moq** в csproj есть, но в тестах не использую — тестирую реальные реализации + InMemory EF (ближе к интеграционному поведению).

---

## 15. Сборка, установщик и развёртывание

### 15.1. Launcher (`DnDCharacterManager.exe`)

Алгоритм:
1. Запуск `DnDCharacterManager.Api.exe` (скрытое окно, порт 5049)
2. Ожидание готовности API — polling `GET /api/reference/races` (timeout 45 сек)
3. Запуск `DnDCharacterManager.Desktop.exe`
4. При закрытии Desktop — завершение процесса API (`Kill(entireProcessTree: true)`)

### 15.2. Сборка установщика

```powershell
cd installer
powershell -ExecutionPolicy Bypass -File .\build-installer.ps1
```

**Шаги скрипта:**
1. `dotnet publish` API, Desktop, Launcher — `win-x64`, self-contained, ReadyToRun
2. Launcher — single-file exe
3. Копирование `appsettings.Production.json` (SQLite `dnd_app.db`)
4. Inno Setup → `DnDCharacterManager-Setup-1.0.0.exe`

### 15.3. Inno Setup

- Русский язык установщика
- Ярлыки: меню Пуск, опционально рабочий стол
- Post-install: запуск приложения
- Uninstall: удаление logs и БД

---

## 16. CI/CD на GitHub Actions

**Файл:** `.github/workflows/ci.yml`

**Триггер:** push и pull request на `main`

**Шаги на ubuntu-latest:**
1. checkout
2. setup .NET 10.0.x
3. `dotnet restore`
4. `dotnet build -c Release`
5. `dotnet test -c Release`

**Примечание:** установщик Windows в CI не собирается (нужны Windows + Inno Setup); CI проверяет компиляцию и тесты.

---

## 17. Демонстрационный сценарий на защите

Рекомендуемый порядок live-demo (5–7 минут):

1. **Запуск** — через ярлык Launcher или два терминала (API + Desktop).
2. **Swagger** — показать REST API, endpoint `/api/reference/races`.
3. **Создание персонажа** — пройти 2–3 шага мастера, выбрать Standard Array, сохранить.
4. **Лист персонажа** — показать авторасчёт модификаторов, изменить HP, сохранить (PATCH).
5. **PDF** — скачать лист персонажа.
6. **Бестиарий** — поиск и фильтр по типу существа.
7. **Правила** — клик по термину в глоссарии.
8. **AI** — сгенерировать биографию (если настроен ключ Gemini).
9. **Тесты** — `dotnet test` — 47 passed.

---

## 18. Типичные вопросы комиссии и ответы

### «Почему Clean Architecture, а не просто MVC?»

MVC подходит для монолита с UI на сервере. У меня **два клиента** (Desktop сейчас, mobile потенциально) и **общая бизнес-логика** на API. Clean Architecture отделяет домен от инфраструктуры: Core не знает про EF и WPF, что упрощает тестирование и замену компонентов.

### «Зачем API, если приложение desktop?»

1. Разделение ответственности — UI не содержит бизнес-логики.  
2. Mobile-ready — тот же API для MAUI/Flutter.  
3. Централизованный доступ к БД и Gemini.  
4. Integration tests через HTTP — проверка реального pipeline.

### «Какие паттерны вы использовали и где?»

Repository + Unit of Work — доступ к БД. Factory — создание Character. Strategy — три метода характеристик. Singleton — конфигурация Desktop. MVVM + Command — WPF. DI — во всех слоях. Middleware — обработка ошибок API.

### «Почему не FluentValidation?»

Для ВКР достаточно собственного валидатора с явным списком правил — проще объяснить на защите. FluentValidation можно добавить позже без изменения контрактов сервиса.

### «SQLite или PostgreSQL?»

SQLite — для разработки и установщика (не нужна отдельная установка СУБД). PostgreSQL — для production-сценария (масштабируемость, concurrent access). Переключение — одна строка в конфигурации `Database:Provider`.

### «Как считается модификатор характеристики?»

Формула D&D 5e: `(значение - 10) / 2` с **целочисленным делением** в C#. Например, STR 16 → модификатор +3.

### «Что такое Strategy в вашем проекте?»

Интерфейс `IAbilityScoreStrategy` с методом `GenerateScores`. Три реализации — Standard Array, 4d6, Point Buy. CharacterService выбирает стратегию через switch expression по полю `AbilityScoreMethod` в DTO. Добавить четвёртый метод — создать новый класс, не меняя остальной код (Open/Closed).

### «Как работает кэширование?»

`IMemoryCache` в CharacterService. Ключ `character:{guid}`, TTL 5 минут. При Update/Delete — инвалидация кэша. Ускоряет повторное чтение листа персонажа.

### «Как тестируете без реальной PostgreSQL?»

Unit-тесты — EF Core **InMemory** provider. Integration-тесты — **WebApplicationFactory** + SQLite file database в окружении `Testing`. Это стандартный подход Microsoft для ASP.NET Core.

### «Откуда берутся справочные данные (расы, монстры)?»

`DevDataSeeder.SeedAsync()` — при первом запуске API после миграции. Данные захардкожены в коде seeder'а (учебный набор для демо).

### «Безопасность API-ключа Gemini?»

Ключ хранится в `appsettings.json` на сервере, Desktop вызывает только `/api/ai/generate`. Ключ не передаётся в исходниках клиента (кроме локальных настроек Settings для dev).

### «Сколько тестов и что они покрывают?»

47 тестов: валидация, фабрика, стратегии, сервис с InMemory, репозиторий, 6 integration-тестов API включая PDF.

### «Чем ваш проект отличается от готовых листов персонажа D&D?»

Это **полноценное приложение**: мастер создания, хранение в БД, REST API, бестиарий, правила с глоссарием, AI-генерация, PDF-экспорт, установщик — не просто статичная форма, а система с архитектурой enterprise-уровня.

### «Какие операторы C# вы использовали?»

См. [раздел 6.6](#66-операторы-c-справочник-для-комиссии): арифметические, логические, null-coalescing (`??`), null-conditional (`?.`), lambda (`=>`), pattern matching (`is`, switch expressions).

### «Что бы вы улучшили в следующей версии?»

- JWT-авторизация пользователей  
- Полноценные скриншоты в README  
- MAUI mobile-клиент на том же API  
- FluentValidation  
- PostgreSQL в установщике для multi-user  

---

## Краткая шпаргалка (1 минута перед защитой)

- **Стек:** C# / .NET 10, WPF MVVM, ASP.NET Core API, EF Core, SQLite/PostgreSQL, QuestPDF, Gemini  
- **Архитектура:** Clean Architecture, 6 проектов, DI  
- **Паттерны:** Repository, Unit of Work, Factory, Strategy, Singleton, MVVM  
- **C#:** async/await, LINQ, generics, nullable, switch expressions, pattern matching  
- **API:** 5 контроллеров, CRUD + PDF + AI  
- **UI:** 9 шагов мастера, 7 экранов, Material Design  
- **Тесты:** 47/47 xUnit  
- **Деплой:** Launcher + Inno Setup 1.0.0  

---

*Документ подготовлен для устной защиты ВКР. Актуальная версия кода — репозиторий DnD_Hub, ветка `main`.*
