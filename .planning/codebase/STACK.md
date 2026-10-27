---
last_mapped_commit: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
last_mapped_at: 2026-10-27
---
# Технологический стек

**Analysis Date:** 2026-10-27

Документ описывает только то, что реально есть в репозитории. Всё, что указано в `docs/architecture.md` или `docs/plan.md`, но не имеет файлов в коде, помечено как «запланировано».

## Статус реализации

- **Реализовано:** библиотека движка `server/Siege.Engine` (тип `Hex`), её тесты, клиентский каркас на Vue 3 + Vite, линтеры, unit-тесты Vitest, e2e-тест Playwright.
- **Запланировано, в коде отсутствует:** `server/Siege.Server` (ASP.NET Core, REST, SignalR), EF Core и Npgsql, PostgreSQL, Serilog, Pinia, `docker-compose.yml`, `server/Siege.Server.Tests`, CI.

## Языки

**Основные:**
- C# (.NET 8, `net8.0`) - движок правил и тесты: `server/Siege.Engine/Hex.cs`, `server/Siege.Engine.Tests/HexTests.cs`
- TypeScript `~6.0.3` (lock-файл) - клиент: `client/src/game/hex.ts`, `client/src/main.ts`, `client/src/__tests__/App.spec.ts`, `client/e2e/vue.spec.ts`
- Vue SFC (`<script setup lang="ts">`) - `client/src/App.vue`

**Вспомогательные:**
- HTML - `client/index.html`
- JSON - манифесты, `client/package-lock.json`, `global.json`, `.mcp.json`, `client/.oxlintrc.json`, `client/.prettierrc.json`
- Markdown - документация в `docs/`, `CLAUDE.md`, `README.md`

## Среда выполнения

**.NET:**
- Target framework: `net8.0`, общий для решения в `server/Directory.Build.props`
- SDK: закреплён в `global.json` как `8.0.100` с `rollForward: latestFeature`. На машине разработки установлен `8.0.132` (`dotnet --version`)
- `LangVersion` `latest`, `Nullable` `enable`, `ImplicitUsings` `enable`
- `TreatWarningsAsErrors` `true`, `EnforceCodeStyleInBuild` `true`, `AnalysisLevel` `latest-recommended` (всё в `server/Directory.Build.props`)

**Node.js:**
- Требование: `"engines": { "node": "^22.18.0 || >=24.12.0" }` в `client/package.json`
- Установленная на машине разработки версия: `node` из snap (`/snap/bin/node`)
- Менеджер пакетов: npm, lock-файл `client/package-lock.json` (`lockfileVersion: 3`), файл присутствует
- Каталог `client/node_modules` установлен локально (в git не попадает)

## Фреймворки

**Сервер:**
- Движок `Siege.Engine`: только BCL, без `PackageReference` и `ProjectReference` (правило из `.claude/rules/engine.md`)
- ASP.NET Core 8, SignalR: запланированы, в коде не используются

**Клиент:**
- Vue `3.5.44` (lock-файл; в `package.json` `^3.5.42`): `client/src/main.ts` создаёт приложение через `createApp`
- Vite `8.3.4` (lock-файл; в `package.json` `^8.2.2`): сборка и dev-сервер, конфигурация в `client/vite.config.ts`
- Pinia: запланирована (`docs/architecture.md`, раздел 7), в `client/package.json` отсутствует
- SVG для поля: запланирован, сейчас в `client/src/App.vue` только заголовок

**Тестирование:**
- xUnit `2.4.2`, `Microsoft.NET.Test.Sdk` `17.6.0`, `xunit.runner.visualstudio` `2.4.5`, `coverlet.collector` `6.0.0`: `server/Siege.Engine.Tests/Siege.Engine.Tests.csproj`
- Vitest `4.1.11` с окружением `jsdom` (`jsdom` `30.1.3`), `@vue/test-utils` `^2.5.0`: `client/vitest.config.ts`
- Playwright `@playwright/test` `1.64.1`, браузер Chromium, viewport 1920x1080: `client/playwright.config.ts`
- Testcontainers (PostgreSQL для серверных тестов): запланирован

**Сборка и разработка:**
- `dotnet build`, `dotnet test`, `dotnet format` для сервера
- Vite с плагинами `@vitejs/plugin-vue` `6.0.10` и `vite-plugin-vue-devtools` `^8.2.1` (подключён безусловно в `client/vite.config.ts`)
- `vue-tsc` `3.3.13` для проверки типов (`type-check`)
- `npm-run-all2` для составных скриптов lint и build
- Prettier `3.9.6`: `client/.prettierrc.json` (`semi: false`, `singleQuote: true`, `printWidth: 100`)
- Oxlint `1.82.0`: `client/.oxlintrc.json` (плагины `eslint`, `typescript`, `unicorn`, `oxc`, `vue`, `vitest`; категория `correctness` как `error`)
- ESLint `10.13.0` в flat-конфигурации `client/eslint.config.ts` с `@vue/eslint-config-typescript`, `eslint-plugin-vue`, `eslint-plugin-playwright`, `@vitest/eslint-plugin`, `eslint-plugin-oxlint`, `eslint-config-prettier`
- `@vue/tsconfig` `^0.9.1`, `@tsconfig/node24` `^24.0.5`: базовые `tsconfig`

## Ключевые зависимости

**Критичные (рантайм):**
- `vue` `3.5.44`: единственная рантайм-зависимость клиента (`client/package.json`, `dependencies`)

**Критичные (сборка и проверка):**
- `vite` `8.3.4`: сборка клиента, `npm --prefix client run build`
- `vitest` `4.1.11`: unit-тесты клиента
- `@playwright/test` `1.64.1`: e2e-тесты, `client/e2e/`
- `oxlint` `1.82.0`, `eslint` `10.13.0`, `prettier` `3.9.6`: линтеры и форматирование клиента
- `xunit` `2.4.2`: тесты движка
- `Microsoft.NET.Test.Sdk` `17.6.0`: запуск тестов через `dotnet test`

**Инфраструктурные (планируются):**
- `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`: доступ к PostgreSQL, в `docs/architecture.md` раздел 6
- `Serilog`: логирование, требование стандарта компании (`/home/alexander/Projects/alliance/.claude/CLAUDE.md`)
- `pinia`: состояние клиента, `docs/architecture.md` раздел 7

**Заметка по транзитивным зависимостям:** в `server/Siege.Engine.Tests/bin/Debug/net8.0/` есть `Newtonsoft.Json.dll`. В `csproj` тестового проекта такой зависимости нет, она пришла транзитивно через тестовую инфраструктуру. Явно её не объявляем.

## Конфигурация

**Окружение:**
- Переменные окружения сервера: `ConnectionStrings__Default` (строка подключения PostgreSQL, запланирована, `docs/architecture.md` раздел 6, `CLAUDE.md`)
- Переменная `CI`: читается в `client/playwright.config.ts` (при `CI` меняются `retries`, `workers`, `baseURL` на `localhost:4173`, команда webServer на `npm run preview`)
- Файл `.env` существует в корне репозитория и игнорируется git (`.gitignore`). Содержимое в документ не включено и не читалось. Правила проекта запрещают его читать и цитировать.

**Сборка и код:**
- `global.json`: версия SDK `8.0.100`, `rollForward: latestFeature`
- `server/Directory.Build.props`: общие настройки всех проектов решения
- `server/Siege.sln`: решение из `Siege.Engine` и `Siege.Engine.Tests`
- `.editorconfig` (корень): UTF-8, LF, отступы 2 пробела для большинства файлов, 4 пробела для `*.cs`, `max_line_length = 100` для клиента, file-scoped namespace (`warning`)
- `client/tsconfig.app.json`: расширяет `@vue/tsconfig/tsconfig.dom.json`, `noUncheckedIndexedAccess: true`, путь `@/*` указывает на `./src/*`
- `client/vite.config.ts`: алиас `@` на `./src`
- `client/vitest.config.ts`: объединяет конфиг Vite, окружение `jsdom`, исключает `e2e/**`
- `client/playwright.config.ts`: `testDir: ./e2e`, отчёт `list` и `html`, `webServer` поднимает dev-сервер на порту 5173 (или preview на 4173 в CI)
- `.mcp.json`: MCP-сервер `playwright` (`npx @playwright/mcp@latest`, Chromium, viewport 1920x1080, вывод в `docs/screenshots`)

**Права агента (dev-инструменты):**
- `.claude/settings.json`: разрешены `dotnet build|test|format`, `npm --prefix client run *`, `npx vitest`, `npx playwright test`, `npx eslint|oxlint|prettier`. С запросом подтверждения: `npm install|ci|uninstall`, `npx playwright install`, `dotnet add|remove`, `docker`

**Сборка:**
- Сборка сервера: `dotnet build server/Siege.sln` (запуск через `.claude/rules/engine.md`)
- Сборка клиента: `npm --prefix client run build` (type-check и `vite build`)
- Артефакты: `client/dist` (исключён через `client/.gitignore`), `bin/`, `obj/` (исключены `.gitignore` корня)

## Требования к платформе

**Разработка:**
- .NET SDK 8 (`global.json` допускает `8.0.100` и более поздние патчи 8.0)
- Node.js `^22.18.0 || >=24.12.0`, npm
- Chromium для Playwright: `npx playwright install chromium` (один раз, команда требует подтверждения)
- Docker Compose: запланирован для PostgreSQL (`docs/architecture.md` раздел 8), файла `docker-compose.yml` пока нет

**Продакшен:**
- Не определён. По `docs/architecture.md` раздел 8 планируется статическая сборка клиента и сервер ASP.NET Core с PostgreSQL в Docker Compose.
- CI/CD в проекте не используется (`/home/alexander/Projects/alliance/.claude/CLAUDE.md`), пайплайнов и конфигов деплоя в репозитории нет.

## Соответствие стандартам компании

- Бэкенд: ASP.NET Core / .NET 8, EF Core, Serilog, xUnit. Сейчас реализован только движок и его тесты. EF Core и Serilog ещё не подключены.
- Аутентификация: по стандарту JWT на базе ASP.NET Core Identity, но `docs/architecture.md` раздел 5.6 явно выбирает анонимный токен (32 байта, хеш SHA-256 в БД). Это исключение задано проектом, поэтому стандарт не применяется.
- СУБД: PostgreSQL (`docs/architecture.md`, раздел 6). Проект не относится к исключениям АльянсТМ.
- Фронтенд: Vue 3 и Vite для нового клиента. Angular, Vue 2, Razor и jQuery в проекте не используются.

---

*Stack analysis: 2026-10-27*
