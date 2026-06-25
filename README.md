# DnD Character Manager

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows)
![License](https://img.shields.io/badge/license-MIT-green)

Дипломный проект (ВКР): desktop-приложение для создания и управления персонажами **Dungeons & Dragons 5e** с RPG-интерфейсом, интерактивным листом персонажа, бестиарием, справочником правил и генерацией контента через Google Gemini.

## Быстрый старт (локальная разработка)

```bash
git clone https://github.com/SharikBot/DnD_Hub.git
cd DnD_Hub

# Терминал 1 — API (SQLite, без PostgreSQL)
dotnet run --project src/DnDCharacterManager.Api --launch-profile http

# Терминал 2 — Desktop
dotnet run --project src/DnDCharacterManager.Desktop
```

- API: http://localhost:5049  
- Swagger: http://localhost:5049/swagger  

В режиме Development используется SQLite (`dnd_dev.db`). Миграции и seed-данные применяются автоматически при старте API.

## Описание

**DnD Character Manager** — менеджер персонажей D&D с мастером создания, CRUD, экспортом в PDF и REST API. Клиенты desktop и mobile могут использовать общий API.

### Основные возможности

- Пошаговое создание персонажа (раса, класс, предыстория, характеристики, мировоззрение)
- Интерактивный лист персонажа с авторасчётом модификаторов
- Бестиарий с поиском и фильтрацией
- Оцифрованный справочник правил с глоссарием
- AI-генерация имён, биографий и подсказок (Google Gemini)
- Экспорт листа персонажа в PDF (QuestPDF)

## Архитектура

Проект построен по принципам **Clean Architecture**:

| Слой | Проект | Назначение |
|------|--------|------------|
| Core | `DnDCharacterManager.Core` | Сущности, DTO, интерфейсы, паттерны |
| Application | `DnDCharacterManager.Application` | Сервисы, валидаторы, бизнес-логика |
| Infrastructure | `DnDCharacterManager.Infrastructure` | EF Core, репозитории, Gemini API |
| Presentation (API) | `DnDCharacterManager.Api` | ASP.NET Core Web API |
| Presentation (Desktop) | `DnDCharacterManager.Desktop` | WPF + MVVM |

Подробнее: [docs/architecture.md](docs/architecture.md), [docs/uml.md](docs/uml.md), [docs/er-diagram.md](docs/er-diagram.md).

## Технологический стек

| Категория | Технологии |
|-----------|------------|
| Backend | C#, ASP.NET Core Web API, EF Core |
| Desktop | WPF, MVVM, CommunityToolkit.Mvvm, MaterialDesignInXaml |
| База данных | PostgreSQL |
| PDF | QuestPDF |
| AI | Google Gemini API |
| Логирование | Serilog |
| Кэширование | MemoryCache |
| Тестирование | xUnit, Moq, EF Core InMemory |

## Паттерны проектирования

- **Repository** — абстракция доступа к данным (`ICharacterRepository`, `IUnitOfWork`)
- **Factory** — создание доменной модели персонажа (`CharacterFactory`)
- **Strategy** — методы генерации характеристик (`StandardArrayStrategy`, `Roll4D6Strategy`, `PointBuyStrategy`)
- **Singleton** — конфигурация DI и единые сервисы приложения

## Структура solution

```
DnDCharacterManager/
├── src/
│   ├── DnDCharacterManager.Api/
│   ├── DnDCharacterManager.Application/
│   ├── DnDCharacterManager.Core/
│   ├── DnDCharacterManager.Desktop/
│   └── DnDCharacterManager.Infrastructure/
├── tests/
│   └── DnDCharacterManager.Tests/
├── docs/
├── screenshots/
└── DnDCharacterManager.slnx
```

## Требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) 14+
- Google Gemini API key (для AI-функций)

## Настройка PostgreSQL (production)

1. Установите PostgreSQL и создайте базу данных:

```sql
CREATE DATABASE dnd_character_manager;
```

2. Скопируйте `src/DnDCharacterManager.Api/appsettings.example.json` в `appsettings.json` и укажите строку подключения:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=dnd_character_manager;Username=postgres;Password=your_password"
}
```

3. Примените миграции EF Core (из корня solution):

```bash
dotnet ef database update --project src/DnDCharacterManager.Infrastructure --startup-project src/DnDCharacterManager.Api
```

## Настройка Gemini API

Скопируйте `appsettings.example.json` или задайте ключ в `appsettings.json` / переменных окружения:

```json
"Gemini": {
  "ApiKey": "ваш_ключ",
  "Model": "gemini-2.0-flash",
  "BaseUrl": "https://generativelanguage.googleapis.com/v1beta/models/"
}
```

Ключ можно получить в [Google AI Studio](https://aistudio.google.com/apikey).

## Запуск API

```bash
dotnet run --project src/DnDCharacterManager.Api --launch-profile http
```

Swagger UI: http://localhost:5049/swagger

## Запуск Desktop

```bash
dotnet run --project src/DnDCharacterManager.Desktop
```

Перед запуском Desktop убедитесь, что API доступен по адресу из настроек Desktop (`http://localhost:5049` по умолчанию).

## Тестирование

```bash
dotnet test tests/DnDCharacterManager.Tests
```

## Сборка установщика (Windows)

Требуется [.NET 10 SDK](https://dotnet.microsoft.com/download) и [Inno Setup 6](https://jrsoftware.org/isinfo.php).

```powershell
cd installer
powershell -ExecutionPolicy Bypass -File .\build-installer.ps1
```

Результат:

- `installer/output/publish/` — portable-сборка (API + Desktop + Launcher)
- `installer/output/installer/DnDCharacterManager-Setup-1.0.0.exe` — установщик

Ярлык запускает `DnDCharacterManager.exe`: поднимает API на `localhost:5049` и открывает WPF-клиент.

## Публикация на GitHub (Git Bash)

```bash
cd installer
chmod +x push-github.sh
./push-github.sh
```

Перед первым push создайте пустой репозиторий на GitHub и при необходимости задайте имя пользователя:

```bash
export GITHUB_USER=SharikBot
export REPO_NAME=DnD_Hub
./push-github.sh
```

## Скриншоты

> Скриншоты интерфейса — в папке `screenshots/`:

| Файл | Описание |
|------|----------|
| `screenshots/main-menu.png` | Главное меню с sidebar |
| `screenshots/character-creator.png` | Мастер создания персонажа |
| `screenshots/character-sheet.png` | Интерактивный лист персонажа |
| `screenshots/bestiary.png` | Бестиарий |
| `screenshots/rulebook.png` | Справочник правил |
| `screenshots/ai-generator.png` | AI-генератор |

## Документация

- [Архитектура](docs/architecture.md)
- [UML-диаграммы](docs/uml.md)
- [ER-диаграмма](docs/er-diagram.md)
- [Roadmap разработки](docs/roadmap.md)

## Лицензия

[MIT](LICENSE) — учебный дипломный проект. Dungeons & Dragons — торговая марка Wizards of the Coast; проект не аффилирован с Wizards of the Coast.
