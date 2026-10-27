---
last_mapped_commit: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
last_mapped_at: 2026-10-27
---
# Соглашения о коде

**Дата анализа:** 2026-10-27

## Наименование

**Файлы (C#):**
- Один публичный тип на файл, имя файла совпадает с именем типа: `server/Siege.Engine/Hex.cs` содержит `Hex`.
- Тесты называются `<Тип>Tests.cs`: `server/Siege.Engine.Tests/HexTests.cs`.
- Пространства имён: `Siege.<Проект>[.<Папка>]`, например `Siege.Engine`, `Siege.Engine.Tests`. Объявления file-scoped (`namespace Siege.Engine;`).

**Файлы (клиент):**
- Vue-компоненты в `PascalCase.vue`: `client/src/App.vue`. Компоненты по плану: `BoardView`, `HexCell`, `PieceToken`, `ReservePanel`, `StatusBar` (`client/src/components/`, пока не созданы).
- TypeScript-модули в `camelCase.ts`: `client/src/game/hex.ts`.
- Unit-тесты лежат в `__tests__/` рядом с модулем, суффикс `.spec.ts`: `client/src/__tests__/App.spec.ts`.
- E2E-тесты: `client/e2e/*.spec.ts`, например `client/e2e/vue.spec.ts`.

**Функции и методы:**
- C#: `PascalCase` для публичных членов (`DistanceFromCenter`, `Neighbors()`, `Directions`).
- TypeScript: `camelCase` для функций (`hexToPixel` в `client/src/game/hex.ts`), `PascalCase` для типов и интерфейсов (`Hex`, `Point`).

**Переменные:**
- C#: приватные поля с префиксом подчёркивания и camelCase: `_camelCase` (правило `private_fields_underscore` в `.editorconfig`, уровень warning). Локальные переменные и параметры в camelCase: `var direction`, `int radius`.
- TypeScript: `camelCase`, константы верхнего уровня без явного правила, в существующем коде не встречаются.

**Типы:**
- C# состояние движка: `record` или `readonly record struct`. Пример: `public readonly record struct Hex(int Q, int R)` в `server/Siege.Engine/Hex.cs`.
- TypeScript: `interface` для DTO и геометрии (`export interface Hex` в `client/src/game/hex.ts`). DTO клиента описываются в `client/src/api/types.ts`, поля camelCase, дискриминатор хода `kind` (правило `.claude/rules/client.md`).

**Тестовые методы (C#):**
- Формат `Метод_Условие_Результат`: `Neighbors_AnyHex_ReturnsSixDistinctAdjacentHexes`, `DistanceFromCenter_Center_IsZero`. Подчёркивания в именах тестов допустимы, CA1707 отключён для проектов `*.Tests` в `.editorconfig`.

**Тестовые методы (TypeScript):**
- Описания `describe`/`it` пишутся на русском языке, в том же стиле, что и текст интерфейса: `it('показывает название игры', ...)` в `client/src/__tests__/App.spec.ts`.

## Стиль кода

**C#:**
- Отступ 4 пробела (`indent_size = 4` для `*.cs` в `.editorconfig`). Для `*.csproj`, `*.props`, `*.sln` отступ 2.
- Фигурные скобки всегда, открывающая на новой строке (`csharp_prefer_braces = true:warning`, `csharp_new_line_before_open_brace = all`).
- Неявно типизированные переменные (`var`) допустимы, когда тип очевиден (`csharp_style_var_when_type_is_apparent`, уровень suggestion).
- Коллекции через выражения коллекции: `[new(1, 0), new(1, -1)]`, `Hex[] expected = [...]`.
- Первые директивы `using` сортируются, `System` первым. Глобальные using включены через `ImplicitUsings`.
- Пример из `server/Siege.Engine/Hex.cs`:

```csharp
public int DistanceFromCenter => Math.Max(Math.Abs(Q), Math.Max(Math.Abs(R), Math.Abs(Q + R)));
```

**TypeScript и Vue:**
- Prettier (`client/.prettierrc.json`): без точек с запятой (`"semi": false`), одинарные кавычки (`"singleQuote": true`), ширина строки 100.
- Отступ 2 пробела, перевод строки LF (`.editorconfig`, `client/.gitattributes`).
- Пример из `client/src/game/hex.ts`: без точек с запятой, одинарные кавычки, `export interface` и именованные экспорты функций.
- В `.editorconfig` для TS и Vue указан `max_line_length = 100`, он совпадает с Prettier.

**Линтеры:**
- Oxlint (`client/.oxlintrc.json`): категория `correctness` с уровнем `error`, плагины `eslint`, `typescript`, `unicorn`, `oxc`, `vue`, `vitest`. Запуск: `npm --prefix client run lint:oxlint`.
- ESLint (`client/eslint.config.ts`): `@vue/eslint-config-typescript` (recommended), `eslint-plugin-vue` (`flat/essential`), плагин Playwright для `e2e/**`, плагин Vitest для `src/**/__tests__/*`. Форматирование отключено через `eslint-config-prettier`, поэтому форматирует только Prettier.
- Запуск без автоисправления: `npm --prefix client run check:lint`. С автоисправлением: `npm --prefix client run lint`.
- Проверка форматирования: `npm --prefix client run format:check`.

**C# анализаторы:**
- `server/Directory.Build.props`: `Nullable` включён, `ImplicitUsings` включён, `TreatWarningsAsErrors` включён, `AnalysisLevel` = `latest-recommended`, `EnforceCodeStyleInBuild` включён. Предупреждения исправляются, а не подавляются (правило из `CLAUDE.md`).
- Проверка: `dotnet format server/Siege.sln --verify-no-changes`.

## Автоматические проверки при правке

Хук `.claude/hooks/lint.sh` выполняется после каждой правки агентом:
- `server/**/*.cs`: `dotnet format server/Siege.sln --include <файл>`.
- `client/**/*.{ts,vue,js,mts}`: `npx prettier --write` и `npx eslint --fix` в каталоге `client`.
- `*.md`, `*.cs`, `*.ts`, `*.vue`: поиск длинного тире U+2014, при нахождении правка блокируется.

## Импорты

**C#:**
- Глобальные using: `ImplicitUsings` включён. Для тестов дополнительно `global using Xunit;` в `server/Siege.Engine.Tests/GlobalUsings.cs`.
- Явные `using` добавляются только для нестандартных пространств имён.

**TypeScript:**
- Порядок: внешние пакеты (`vue`, `vitest`, `@vue/test-utils`), затем относительные импорты. Пустая строка между группами (`client/src/__tests__/App.spec.ts`).
- Алиас путей: `@/*` указывает на `client/src/*` (`client/tsconfig.app.json`, `client/vite.config.ts`). Пример: `import { hexToPixel } from '@/game/hex'`. В существующих файлах алиас пока не используется, относительные пути работают.
- Правило `.claude/rules/client.md`: тексты интерфейса на русском, тематические названия фишек берутся из `client/src/themes/<тема>/theme.json`, а не хардкодятся в компонентах.

## Обработка ошибок

**Движок (`server/Siege.Engine`):**
- Ход проверяется через членство в `GetLegalMoves`. Второго набора правил на сервере не пишется (`CLAUDE.md`).
- Методы не изменяют аргументы, `Apply` возвращает новое состояние (`.claude/rules/engine.md`). Проверка на нарушение правила оформляется как возврат результата, а не как исключение. Конкретная форма результата в текущем коде пока не встречается, она определяется при реализации `Apply` по `docs/architecture.md`.
- Движок не использует ввод-вывод, `DateTime.Now` и `Random`. Жребий первого хода приходит снаружи.

**Клиент:**
- Ошибка от сервера при `MoveRejected` приводит к запросу свежего состояния и короткому сообщению пользователю (`docs/architecture.md`, раздел 6).
- Пока в `client/src` нет явного обработчика ошибок, ориентир: `docs/architecture.md`.

## Логирование

- Сервер: Serilog (`CLAUDE.md`). Конфигурация и вызовы пока отсутствуют, в `server/` есть только `Siege.Engine` и тесты.
- Клиент: `console` не используется в текущем коде.
- Движок логирование не выполняет: без зависимостей от пакетов.

## Комментарии и документация

**Язык:** комментарии, XML-doc и тексты документации пишутся на русском. Идентификаторы в коде на английском (`CLAUDE.md`).

**Запрещено:** длинное тире (U+2014) в комментариях, сообщениях коммитов и `.md`-файлах (проверяется хуком).

**XML-doc (C#):**
- Каждый публичный тип и член движка имеет `<summary>` на русском. Если смысл неочевиден, добавляются `<param>` и `<returns>` (`.claude/rules/engine.md`).
- Если член реализует правило игры, в `<summary>` указывается раздел `spec.md`. Пример из `server/Siege.Engine/Hex.cs`: «(architecture.md, раздел 4.1)».
- Тесты также имеют XML-doc только при необходимости, в `HexTests.cs` их нет.

**JSDoc/TSDoc:**
- Однострочные комментарии `//` на русском перед экспортом, например в `client/src/game/hex.ts`: `// Центр клетки в пикселях для ориентации острой вершиной вверх; size задаёт радиус клетки`.
- Заголовочный комментарий файла описывает назначение модуля и границы: `// Геометрия гексагонального поля для отрисовки в SVG ... Правил игры здесь нет`.

## Проектирование функций

**Размер:** функции короткие, геометрия и подсветка выносятся в чистые функции в `client/src/game/*.ts`. Компоненты только рисуют (`.claude/rules/client.md`).

**Параметры:**
- Тип состояния передаётся явно. Координаты в движке осевые `(q, r)`, направления берутся из `Hex.Directions`.
- Радиус поля приходит из `GameOptions`, константой не зашивается (`.claude/rules/engine.md`).

**Возвращаемые значения:**
- Коллекции возвращаются как `IEnumerable<T>` с ленивым `yield return` (`Hex.Neighbors()`), либо как `IReadOnlyList<T>` для статических наборов (`Hex.Directions`).

## Проектирование модулей

**Экспорты (C#):** публичные типы в своём файле, модуль движка без `PackageReference`/`ProjectReference`.

**Экспорты (TypeScript):** именованные экспорты (`export function hexToPixel`, `export interface Hex`). Default-экспорт не используется.

**Компоненты Vue:**
- Только `<script setup lang="ts">` и Composition API. Options API и `any` запрещены (`.claude/rules/client.md`).
- Props и события типизируются через `defineProps<...>()` и `defineEmits<...>()`.
- Пример текущего компонента `client/src/App.vue`: пустой `<script setup lang="ts"></script>` и шаблон с `<main>`.

**Barrel-файлы:** не используются, `index.ts`-реэкспорты в проекте пока не созданы.

## Фишки и предметная область

- В коде движка фишки называются по поведению: `Sovereign`, `Climber`, `Jumper`, `Scout`, `Runner`. Тематические названия (король, рыцарь) в коде и комментариях движка не используются.
- Тематические названия и изображения хранятся в `client/src/themes/medieval/theme.json` и SVG фишек, а не в компонентах.
- Копировать названия и графику оригинальной игры запрещено (`docs/spec.md`, §9).

## Markdown-документы

- Правило `.claude/rules/markdown.md`: пиши на русском, идентификаторы и пути оставляй в оригинале, даты в формате `ГГГГ-ММ-ДД`, кавычки «ёлочками» в русском тексте, в коде прямые.
- Один заголовок первого уровня, уровни не пропускаются.
- Пути файлов и идентификаторы в обратных кавычках, блоки кода с языком (`csharp`, `ts`, `vue`, `json`, `bash`).
- У таблиц обязательна строка заголовков и разделитель `|---|---|`.
- Без эмодзи.

---

*Анализ соглашений: 2026-10-27*
