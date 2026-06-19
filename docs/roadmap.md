# Roadmap разработки DnD Character Manager

Пошаговый план реализации дипломного проекта — 10 этапов.

## Этап 1 — Архитектура

- [x] Проектирование Clean Architecture (Core, Application, Infrastructure, Presentation)
- [x] Определение границ слоёв и зависимостей
- [x] Выбор технологий: WPF, ASP.NET Core, EF Core, PostgreSQL
- [x] Документация: architecture.md, uml.md, er-diagram.md

## Этап 2 — Структура solution

- [x] Создание solution и проектов (`Api`, `Application`, `Core`, `Desktop`, `Infrastructure`, `Tests`)
- [x] Настройка Dependency Injection
- [x] Базовые интерфейсы репозиториев и сервисов
- [x] Структура репозитория: `.gitignore`, `README.md`, `docs/`, `screenshots/`

## Этап 3 — UI (Desktop)

- [x] MainWindow с sidebar-навигацией
- [x] Material Design тема и стили
- [x] MVVM: ViewModels, NavigationService, RelayCommand
- [x] Страницы: создатель, список, бестиарий, правила, AI, настройки
- [x] Полный пошаговый wizard создания персонажа (все 9 шагов)
- [x] Tooltips, help popup, glossary

## Этап 4 — Модели

- [x] Доменные сущности: User, Character, Race, Class, Background, Inventory и др.
- [x] DTO для API и Desktop
- [x] Enums: AbilityType, AlignmentType, CreatureType, RuleCategory
- [x] BaseEntity с аудит-полями

## Этап 5 — Сервисы

- [x] CharacterService (CRUD + кэширование)
- [x] MonsterService, RuleService
- [x] CreateCharacterValidator
- [x] CharacterFactory, AbilityScoreStrategy (Standard Array, 4d6, Point Buy)
- [x] Полный расчёт характеристик с бонусами расы и класса

## Этап 6 — База данных

- [x] AppDbContext и Fluent API конфигурации
- [x] Repository Pattern + Unit of Work
- [x] PostgreSQL connection string
- [x] EF Core миграции и seed-данные (расы, классы, монстры)
- [x] Хранение путей к изображениям персонажей

## Этап 7 — AI (Google Gemini)

- [x] GeminiAiService с HttpClient
- [x] AiController в API
- [x] Конфигурация ApiKey / Model в appsettings
- [x] Retry policy и расширенная обработка ошибок
- [x] UI: loading states для всех AI-операций

## Этап 8 — PDF

- [x] CharacterPdfService на QuestPDF
- [x] Официальный вид D&D character sheet
- [x] Preview перед экспортом
- [x] Поддержка портрета персонажа в PDF

## Этап 9 — Тестирование

- [x] xUnit test project
- [x] CharacterServiceTests (CRUD, InMemory DB)
- [x] CharacterFactoryTests
- [x] AbilityScoreStrategyTests
- [x] CreateCharacterValidatorTests (positive / negative)
- [x] CharacterRepositoryTests (InMemory EF)
- [x] Integration tests для API controllers

## Этап 10 — Финальный polishing

- [ ] Скриншоты для README
- [x] Snackbar-уведомления об ошибках в Desktop
- [x] Оптимизация производительности и кэширования
- [x] Финальный code review и рефакторинг
- [ ] Подготовка к защите ВКР (презентация, демо-сценарий)

---

**Текущий статус:** этапы 1–9 выполнены; этап 10 — скриншоты и материалы к защите.
