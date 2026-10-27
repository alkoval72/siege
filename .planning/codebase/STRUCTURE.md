---
last_mapped_commit: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
last_mapped_at: 2026-10-27
---
# Codebase Structure

**Analysis Date:** 2026-10-27

## Directory Layout

```
siege/
├── CLAUDE.md                     # инструкции для Claude Code (проект и стек, команды, конвенции)
├── README.md                     # описание проекта
├── .mcp.json                     # MCP-сервер Playwright (Chromium, скриншоты в docs/screenshots/)
├── .editorconfig                 # общие правила форматирования
├── .gitignore                    # node_modules, артефакты Playwright, .env.*, локальные настройки
├── global.json                   # версия .NET SDK (8.0.100, rollForward latestFeature)
├── docs/                         # спецификация, архитектура, handoff-файлы, скриншоты
├── .claude/                      # настройки агента: права, hooks, правила, навыки
├── server/                       # .NET 8: решение, движок правил, тесты
│   ├── Siege.sln
│   ├── Directory.Build.props     # общие настройки сборки для всех проектов
│   ├── Siege.Engine/             # библиотека правил (только BCL)
│   └── Siege.Engine.Tests/       # xUnit-тесты движка
└── client/                       # Vue 3 + TypeScript + Vite
    ├── index.html                # HTML-оболочка с <div id="app">
    ├── package.json              # скрипты npm, зависимости
    ├── src/                      # исходники приложения
    │   ├── main.ts               # точка входа Vue
    │   ├── App.vue               # корневой компонент (заглушка)
    │   ├── game/                 # чистая геометрия поля
    │   │   └── hex.ts
    │   └── __tests__/            # unit-тесты Vitest рядом с модулями
    │       └── App.spec.ts
    └── e2e/                      # e2e-тесты Playwright
        └── vue.spec.ts
```

Примечание: каталоги `server/Siege.Server/`, `server/Siege.Server.Tests/`, `client/src/api/`, `client/src/stores/`, `client/src/components/`, `client/src/views/`, `client/src/themes/`, файл `docker-compose.yml` описаны в `docs/architecture.md` §3 как целевые, но в репозитории отсутствуют. Перед созданием файлов в этих местах каталог создается по мере необходимости.

## Directory Purposes

**`docs/`:**
- Purpose: спецификация и handoff-документы.
- Contains: `spec.md` (правила игры, §4 фишки, §5 правила, §12 журнал решений), `architecture.md` (целевая архитектура, §4 движок, §5 сервер, §6 БД, §11 журнал решений, §12 точки сверки с официальными правилами), `plan.md` (план с чекбоксами), `progress.md` (журнал, только дописывать в конец), `screenshots/` (PNG для README и Playwright MCP).
- Key files: `docs/spec.md`, `docs/architecture.md`, `docs/plan.md`, `docs/progress.md`.

**`.claude/`:**
- Purpose: инструкции и автоматизация агента.
- Contains: `settings.json` (права, режим по умолчанию, hooks, в том числе GSD), `rules/` (правила по путям), `skills/` (навыки), `hooks/` (скрипты `protect-files.sh`, `lint.sh`), `agents/`, `commands/`, `gsd-core/`, `scripts/` (инструменты GSD).
- Key files: `.claude/rules/engine.md` (`paths: server/Siege.Engine/**`, `server/Siege.Engine.Tests/**`), `.claude/rules/client.md` (`paths: client/src/**`), `.claude/rules/markdown.md` (`paths: **/*.md`), `.claude/skills/engine-test/SKILL.md`, `.claude/skills/verify/SKILL.md`, `.claude/skills/handoff/SKILL.md`, `.claude/hooks/protect-files.sh`, `.claude/hooks/lint.sh`.
- Note: каталоги GSD (`agents/`, `commands/`, `gsd-core/`, `hooks/gsd-*`, `scripts/`, `gsd-*.json`) служебные и в анализ продукта не входят.

**`server/`:**
- Purpose: серверная часть на .NET 8. Сейчас содержит только движок правил и его тесты.
- Contains: `Siege.sln` (два проекта: `Siege.Engine`, `Siege.Engine.Tests`), `Directory.Build.props`.
- Key files: `server/Directory.Build.props` (`TargetFramework net8.0`, `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`, `AnalysisLevel latest-recommended`).

**`server/Siege.Engine/`:**
- Purpose: библиотека правил игры. Без сети, БД, интерфейса и темы.
- Contains: `Hex.cs`, `Siege.Engine.csproj` (только `RootNamespace`, без зависимостей).
- Key files: `server/Siege.Engine/Hex.cs` (осевые координаты, направления, расстояние, соседи).
- Planned (по `docs/architecture.md` §4 и §3): типы состояния и ходов, `Game`, алгоритмы (`Tarjan`, скольжение, обход для `Scout` и `Runner`), `GameOptions`. Размещение по папкам согласовать с правилом `Siege.<Проект>[.<Папка>]` из `CLAUDE.md` (например, `Siege.Engine/Board/`, `Siege.Engine/Moves/`).

**`server/Siege.Engine.Tests/`:**
- Purpose: xUnit-тесты движка.
- Contains: `HexTests.cs`, `GlobalUsings.cs` (`global using Xunit;`), `Siege.Engine.Tests.csproj` (xunit, Microsoft.NET.Test.Sdk, coverlet.collector).
- Key files: `server/Siege.Engine.Tests/HexTests.cs`.

**`client/`:**
- Purpose: браузерный интерфейс на Vue 3, Vite, TypeScript, SVG.
- Contains: исходники `src/`, `e2e/`, конфигурации Vite, Vitest, Playwright, TypeScript, ESLint, Oxlint, Prettier (`.oxlintrc.json`, `eslint.config.ts`, `.prettierrc.json`), `public/favicon.ico`, `env.d.ts`, `tsconfig.*.json`.
- Key files: `client/src/main.ts`, `client/src/App.vue`, `client/src/game/hex.ts`, `client/vite.config.ts`, `client/vitest.config.ts`, `client/playwright.config.ts`.
- Note: `client/node_modules/` присутствует локально, в git не отслеживается.

**`client/src/`:**
- Purpose: исходный код приложения.
- Contains (реализовано): `main.ts`, `App.vue`, `game/hex.ts`, `__tests__/App.spec.ts`.
- Contains (планируется по `docs/architecture.md` §3 и `.claude/rules/client.md`): `api/` (`http.ts`, `hub.ts`, `types.ts`), `stores/` (`gameStore.ts`, Pinia), `components/` (`BoardView`, `HexCell`, `PieceToken`, `ReservePanel`, `StatusBar`), `views/` (`HomeView`, `GameView`), `themes/medieval/` (`theme.json`, `svg/`), `game/highlight.ts`.

**`client/src/game/`:**
- Purpose: чистые функции геометрии и подсветки, без Vue-зависимостей.
- Contains: `hex.ts` (интерфейсы `Hex`, `Point`, функция `hexToPixel(hex, size)`).

**`client/src/__tests__/`:**
- Purpose: unit-тесты Vitest для модулей `src/`.
- Contains: `App.spec.ts` (проверяет, что `App` показывает «Siege»).
- Convention: тесты лежат в `__tests__/` рядом с модулем (`client/src/game/__tests__/hex.spec.ts` для модуля `game/hex.ts`, по `CLAUDE.md`). Пока папки `client/src/game/__tests__/` нет.

**`client/e2e/`:**
- Purpose: сквозные тесты Playwright.
- Contains: `vue.spec.ts` (открывает `/`, проверяет заголовок `h1` «Siege»), `tsconfig.json`.

## Key File Locations

**Entry Points:**
- `client/index.html`: HTML-оболочка, подключает `/src/main.ts`.
- `client/src/main.ts`: создание приложения Vue и монтирование `App`.
- `server/Siege.Engine.Tests/HexTests.cs`: точка входа проверок движка (через `dotnet test`).
- Серверный `Program.cs` отсутствует (планируется `server/Siege.Server/Program.cs`).

**Configuration:**
- `server/Directory.Build.props`: общие свойства сборки .NET.
- `global.json`: версия SDK .NET.
- `.editorconfig`: форматирование и правила анализа (`CA1707` отключен для тестов, по `CLAUDE.md`).
- `client/vite.config.ts`: плагины Vue и devtools, алиас `@` на `src`.
- `client/vitest.config.ts`: окружение `jsdom`, исключение `e2e/**`.
- `client/playwright.config.ts`: конфигурация e2e.
- `client/tsconfig.app.json`, `client/tsconfig.vitest.json`, `client/tsconfig.node.json`, `client/tsconfig.json`: TypeScript.
- `.mcp.json`: MCP-сервер Playwright.
- `.claude/settings.json`: права и hooks агента.

**Core Logic:**
- `server/Siege.Engine/Hex.cs`: геометрия поля (`Directions`, `Center`, `DistanceFromCenter`, `Neighbors()`).
- `client/src/game/hex.ts`: пиксельные координаты (`hexToPixel`), формула совпадает с `Hex.cs` по соглашению осей.

**Testing:**
- `server/Siege.Engine.Tests/HexTests.cs`: тесты геометрии, включая подсчет клеток радиуса 10 (331).
- `client/src/__tests__/App.spec.ts`: unit-тест корневого компонента.
- `client/e2e/vue.spec.ts`: e2e-смоук главной страницы.

**Документы правил:**
- `docs/spec.md`: правила игры (источник истины для движка).
- `docs/architecture.md`: архитектура, контракты, схема БД.
- `.claude/rules/engine.md`, `.claude/rules/client.md`, `.claude/rules/markdown.md`: правила, подгружаемые по путям.

## Naming Conventions

**Файлы C#:**
- Один публичный тип на файл, имя файла совпадает с именем типа: `Hex.cs`.
- Тесты: `<Тип>Tests.cs`, например `HexTests.cs`.
- Проект: `Siege.<Проект>`, тесты `Siege.<Проект>.Tests`.

**Файлы TypeScript и Vue:**
- Компоненты: `PascalCase.vue` (`App.vue`; планируется `BoardView.vue`, `HexCell.vue`).
- Модули: `camelCase.ts` (`main.ts`, `hex.ts`; планируется `gameStore.ts`, `highlight.ts`, `http.ts`, `hub.ts`). Исключение, уже принятое в репозитории: `hex.ts` и `main.ts` в нижнем регистре, их имена совпадают с содержимым.
- Unit-тесты: `<модуль>.spec.ts` в `__tests__/`.
- e2e-тесты: `client/e2e/*.spec.ts`.

**Каталоги:**
- Серверные: `Siege.<Проект>[.<Папка>]`, например `Siege.Engine`.
- Клиентские: строчные, `camelCase` или одно слово (`game`, `stores`, `components`, `views`, `themes`, `api`).

**Идентификаторы в коде:** английские (`Hex`, `Directions`, `DistanceFromCenter`). Комментарии, XML-doc и тексты интерфейса: русские.

**Типы фишек:** поведенческие имена (`Sovereign`, `Climber`, `Jumper`, `Scout`, `Runner`) в движке и API. Тематические названия только в `client/src/themes/`.

**Документы:** UPPERCASE для корневых файлов проекта (`CLAUDE.md`, `README.md`). В `docs/` имена строчные (`spec.md`, `architecture.md`). В `.planning/codebase/` (GSD) имена UPPERCASE.

## Where to Add New Code

**Новое правило или тип движка:**
- Реализация: `server/Siege.Engine/` (при желании подпапка по смыслу, например `server/Siege.Engine/Moves/`). Без зависимостей от ASP.NET, EF, Serilog, `DateTime.Now`, `Random`.
- Тесты: `server/Siege.Engine.Tests/<Тип>Tests.cs`, имена методов `Метод_Условие_Результат`, табличные случаи через `[Theory]`. Для новых правил использовать skill `engine-test`.
- Сначала сверить правило с `docs/spec.md` §4-5 и `docs/architecture.md` §4. Если правило меняется, сначала обновить `spec.md` с записью в журнал решений.

**Новый алгоритм движка (связность, скольжение, обход фишек):**
- Реализация: `server/Siege.Engine/` как чистая функция над неизменяемым состоянием.
- Тесты: отдельный класс, например `server/Siege.Engine.Tests/ConnectivityTests.cs`, плюс случайные партии по `docs/architecture.md` §9 (когда появятся ходы).

**Новый серверный сервис, контроллер или хаб (планируется):**
- Реализация: `server/Siege.Server/` (создать проект, добавить в `server/Siege.sln`).
- Контроллеры и хаб: `server/Siege.Server/Controllers/`, `server/Siege.Server/Hubs/` (`GameHub.cs`).
- Сервисы и сессии: `server/Siege.Server/Services/` (`GameService.cs`), `server/Siege.Server/Sessions/` (`GameRegistry.cs`, `GameSession.cs`).
- Доступ к БД: `DbContext` в `server/Siege.Server/Data/`, без слоя Repository.
- Тесты: `server/Siege.Server.Tests/` (`WebApplicationFactory`, Testcontainers).

**Новый компонент интерфейса:**
- Реализация: `client/src/components/<PascalCase>.vue` (`<script setup lang="ts">`, Composition API).
- Геометрия или подсветка без Vue-зависимостей: `client/src/game/<модуль>.ts`.
- Тесты: `client/src/__tests__/<Компонент>.spec.ts` (по запросу), e2e в `client/e2e/`.

**Новая страница (view):**
- Реализация: `client/src/views/<Name>View.vue` (`HomeView`, `GameView` по плану). Маршрутизация (vue-router) не установлена, добавлять через `npm` только после согласования.

**Новый API-вызов или тип DTO:**
- REST и SignalR-клиент: `client/src/api/http.ts`, `client/src/api/hub.ts` (планируется).
- Типы DTO вручную: `client/src/api/types.ts`. camelCase, дискриминатор хода `kind`.

**Pinia-хранилище (планируется):**
- Реализация: `client/src/stores/gameStore.ts`. Зависимость `pinia` не установлена, добавляется только с согласования.

**Тема оформления (планируется):**
- Файлы: `client/src/themes/medieval/theme.json` (названия, описания, соответствие идентификатора фишки и SVG, цвета команд), `client/src/themes/medieval/svg/`.
- Тексты интерфейса на русском, не хардкодить «Король» в компонентах.

**Утилиты:**
- Общая геометрия движка: `server/Siege.Engine/Hex.cs` и связанные типы.
- Клиентские чистые функции: `client/src/game/`.

**Документация правил:**
- Правила игры: `docs/spec.md`. Архитектура: `docs/architecture.md`. Журналы решений: разделы §12 и §11 соответственно.

## Special Directories

**`client/node_modules/`:**
- Purpose: зависимости npm.
- Generated: да (`npm --prefix client ci`).
- Committed: нет (`client/.gitignore`).

**`server/**/bin/`, `server/**/obj/`:**
- Purpose: артефакты сборки .NET.
- Generated: да (`dotnet build server/Siege.sln`).
- Committed: нет (`.gitignore`).

**`client/dist/` (при сборке):**
- Purpose: статическая сборка Vite.
- Generated: да (`npm --prefix client run build`).
- Committed: нет (проверить `client/.gitignore`).

**`docs/screenshots/`:**
- Purpose: скриншоты для README и отчетов Playwright MCP (`.mcp.json`, `--output-dir docs/screenshots`).
- Generated: да, Playwright MCP и e2e.
- Committed: да (PNG отслеживаются git).

**`.claude/gsd-core/`, `.claude/agents/`, `.claude/commands/`, `.claude/hooks/gsd-*`, `.planning/`:**
- Purpose: инструменты GSD и артефакты планирования.
- Generated: да, установщиком GSD.
- Committed: по решению владельца (`commit_docs: false` в `.planning/config.json`).

**`.planning/codebase/`:**
- Purpose: документы картирования кодовой базы (этот файл и соседние).
- Generated: да, командой `/gsd-map-codebase`.
- Committed: зависит от `commit_docs`.

## Заметки по структуре

- Серверная и клиентская части разделены верхнеуровневыми папками `server/` и `client/`, что соответствует монорепо из `docs/architecture.md` §3.
- Общие контракты (типы хода, состояния) пока описаны только в документации. Реализация DTO клиента и сервера ожидается вручную в `client/src/api/types.ts` и в `Siege.Server`.
- Проверки перед коммитом (`/verify`, skill `.claude/skills/verify/SKILL.md`) ссылаются на команды, которые уже работают для `server/` и `client/`.

---

*Structure analysis: 2026-10-27*
