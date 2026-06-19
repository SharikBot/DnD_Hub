# ER-диаграмма базы данных

Схема нормализована до **3НФ**: справочники (расы, классы, навыки и т.д.) вынесены в отдельные таблицы; связи M:N реализованы через промежуточные таблицы.

```mermaid
erDiagram
    users ||--o{ characters : owns
    races ||--o{ characters : has
    character_classes ||--o{ characters : has
    backgrounds ||--o{ characters : has
    characters ||--|| inventories : has
    inventories ||--o{ inventory_items : contains
    characters ||--o{ character_traits : has
    traits ||--o{ character_traits : referenced_by
    characters ||--o{ character_skills : has
    skills ||--o{ character_skills : referenced_by
    characters ||--o{ character_spells : knows
    spells ||--o{ character_spells : referenced_by

    users {
        string id PK
        string display_name
        string email UK
        string password_hash
        datetime created_at
        datetime updated_at
    }

    characters {
        string id PK
        string name
        int level
        string alignment
        string backstory
        int strength
        int dexterity
        int constitution
        int intelligence
        int wisdom
        int charisma
        int current_hit_points
        int max_hit_points
        int armor_class
        int speed
        string portrait_path
        string user_id FK
        string race_id FK
        string class_id FK
        string background_id FK
        datetime created_at
        datetime updated_at
    }

    races {
        string id PK
        string name
        string description
        int strength_bonus
        int dexterity_bonus
        int constitution_bonus
        int intelligence_bonus
        int wisdom_bonus
        int charisma_bonus
        int base_speed
        string size
    }

    character_classes {
        string id PK
        string name
        string description
        string hit_die
        string primary_ability
        int base_armor_class
    }

    backgrounds {
        string id PK
        string name
        string description
        string skill_proficiencies
        string feature
    }

    inventories {
        string id PK
        string character_id FK
        float max_weight
        int gold
    }

    inventory_items {
        string id PK
        string inventory_id FK
        string name
        string description
        int quantity
        float weight
        float value_in_gold
        boolean is_equipped
    }

    traits {
        string id PK
        string name
        string description
        string source
    }

    character_traits {
        string character_id PK
        string trait_id PK
        string notes
    }

    skills {
        string id PK
        string name
        string ability
        string description
    }

    character_skills {
        string character_id PK
        string skill_id PK
        boolean is_proficient
        int bonus
    }

    spells {
        string id PK
        string name
        int level
        string school
        string casting_time
        string range
        string components
        string duration
        string description
    }

    character_spells {
        string character_id PK
        string spell_id PK
        boolean is_prepared
    }

    monsters {
        string id PK
        string name
        string challenge_rating
        string creature_type
        int armor_class
        int hit_points
        int speed
        int strength
        int dexterity
        int constitution
        int intelligence
        int wisdom
        int charisma
        string description
    }

    rules {
        string id PK
        string title
        string content
        string category
        string source
    }
```

## Нормализация (3НФ)

| Таблица | Обоснование |
|---------|-------------|
| `users` | Данные пользователя не зависят от персонажей |
| `races`, `character_classes`, `backgrounds` | Справочники без транзитивных зависимостей |
| `characters` | Только FK на справочники; характеристики принадлежат персонажу |
| `character_traits`, `character_skills`, `character_spells` | M:N без дублирования атрибутов справочников |
| `inventories` / `inventory_items` | Инвентарь отделён от персонажа; предметы — отдельные записи |
| `monsters`, `rules` | Независимые справочники бестиария и правил |

## Типы связей

- **1:M** — `users` → `characters`, `races` → `characters`, `inventories` → `inventory_items`
- **1:1** — `characters` → `inventories` (`character_id` уникален)
- **M:N** — `characters` ↔ `traits`, `skills`, `spells` через join-таблицы

## Примечание к диаграмме

В Mermaid для ER допускается только один маркер ключа на поле (`PK`, `FK` или `UK`). Составные ключи join-таблиц показаны двумя полями с `PK`; внешние ключи — через связи `||--o{` и поля `FK`.
