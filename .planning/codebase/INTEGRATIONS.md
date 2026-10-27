---
last_mapped_commit: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
last_mapped_at: 2026-10-27
---
# Внешние интеграции

**Analysis Date:** 2026-10-27

Документ описывает интеграции, которые есть в коде сейчас, и отдельно интеграции, которые запланированы в `docs/architecture.md`. Запланированное не считается реализованным, пока нет файлов в репозитории.

## Статус реализации

- **Реализовано:** внешних API, облачных сервисов и баз данных в продуктовом коде нет. Единственная внешняя зависимость клиента на рантайме: пакет `vue` из npm.
- **Реализовано для разработки:** MCP-сервер Playwright для агента, пакеты npm и NuGet из реестров, Claude Code hooks.
- **Запланировано:** PostgreSQL, SignalR, REST API, `localStorage` в браузере, `docker-compose.yml`, Serilog, Testcontainers.

## API и внешние сервисы

**Сторонние API:**
- Не обнаружено. В `client/src` и `server/` нет HTTP-клиентов к внешним сервисам, SDK облачных провайдеров и ключей API.

**Инструменты разработки:**
- Playwright MCP: `.mcp.json`, сервер `playwright` запускает `npx @playwright/mcp@latest` с Chromium, viewport 1920x1080, скриншоты пишутся в `docs/screenshots`. Разрешён в `.claude/settings.json` через `enabledMcpjsonServers` и `mcp__playwright__*`.
- Реестры пакетов: npm (`client/package-lock.json`, `npm --prefix client ci`), NuGet (`server/Siege.Engine.Tests/Siege.Engine.Tests.csproj`, пакеты `xunit`, `Microsoft.NET.Test.Sdk`, `coverlet.collector`).
- Браузер для e2e: Chromium, устанавливается через `npx playwright install chromium`.

## Хранилище данных

**База данных (запланирована):**
- PostgreSQL, описание в `docs/architecture.md` раздел 6. Таблицы `players`, `games`, `moves`. Истина хранится в виде списка ходов, снимков состояния нет.
- Доступ: EF Core с провайдером Npgsql через `DbContext` напрямую, без слоя Repository (`docs/architecture.md`, журнал решений).
- Строка подключения: переменная окружения `ConnectionStrings__Default`. В коде она пока не читается, так как `Siege.Server` отсутствует.
- Миграции EF Core: запланированы, каталога миграций нет.
- Тесты сервера: PostgreSQL через Testcontainers (запланировано, `docs/architecture.md` раздел 9).

**Клиентское хранилище (запланировано):**
- `localStorage` браузера хранит `gameId` и `playerToken` для восстановления партии (`docs/architecture.md` разделы 5.6 и 7). В `client/src` обращений к `localStorage` сейчас нет.

**Файловое хранилище:**
- Только локальная файловая система. Статика клиента: `client/public/favicon.ico`. Скриншоты и отчёты Playwright: `docs/screenshots/`, `client/playwright-report/`, `client/test-results/`.
- Облачного хранилища (S3, Blob Storage и т. п.) нет.

**Кэширование:**
- Не используется во внешних системах. Активные партии предполагается держать в памяти процесса в `GameRegistry` (`docs/architecture.md` раздел 5.5). Это не внешний сервис, а словарь в памяти сервера.

## Аутентификация и идентичность

**Провайдер:**
- Внешнего провайдера (OAuth, Identity-сервис, Auth0 и т. п.) нет.
- Запланирована анонимная идентичность: сервер генерирует случайный токен (32 байта) при создании или входе, клиент хранит его в `localStorage`, в БД хранится только SHA-256 хеш (`docs/architecture.md` раздел 5.6, таблица `players.token_hash`).
- Токен передаётся аргументом SignalR-метода `JoinGame` или заголовком `X-Player-Token`, но не в адресе (`docs/architecture.md` раздел 5.2).
- JWT и ASP.NET Core Identity не используются. Стандарт компании допускает отступление, которое здесь задано явно в `docs/architecture.md`.

## Мониторинг и наблюдаемость

**Отслеживание ошибок:**
- Не настроено (Sentry, Application Insights и т. п. отсутствуют).

**Логи:**
- Serilog запланирован как стандарт компании (`/home/alexander/Projects/alliance/.claude/CLAUDE.md`). В `server/` пакет не подключён, вызовов логгера нет.
- Клиент: логирования в `client/src` нет.

**Аналитика:**
- Не обнаружена.

## CI/CD и деплой

**Хостинг:**
- Не определён. `docs/architecture.md` раздел 8 описывает запуск локально через Docker Compose: сервис `db` (PostgreSQL), сервис `server`, сервис `client` (статическая сборка). Файла `docker-compose.yml` в репозитории нет.

**CI-пайплайн:**
- Нет. В `.github/`, `.gitlab-ci.yml` и подобных файлах конфигурации CI в репозитории нет. По стандарту компании CI пока не используется.
- Переменная `CI` влияет только на `client/playwright.config.ts` (повторы, порт preview, `forbidOnly`). Она не означает наличие пайплайна.

**Права агента:**
- `docker *` в `.claude/settings.json` стоит в списке с запросом подтверждения. Docker Compose для продукта пока не нужен.

## Переменные окружения и секреты

**Требуемые переменные (запланированные):**
- `ConnectionStrings__Default`: строка подключения к PostgreSQL сервера (`docs/architecture.md` раздел 8)
- `CI`: только для Playwright, см. `client/playwright.config.ts`

**Где лежат секреты:**
- Файл `.env` в корне репозитория. Он игнорируется git (`.gitignore`, правило `.env.*` и `.env`). Содержимое не читалось и в документ не включено. Правила проекта запрещают доступ к `.env` и его шаблонам, защита реализована в `.claude/hooks/protect-files.sh`.
- Личные настройки агента: `.claude/settings.local.json`, в `.gitignore`.
- Секретов в коде, `docs/` и `client/src` не обнаружено.

## Webhooks и callbacks

**Входящие:**
- Нет. HTTP-эндпоинтов сервера пока нет (`server/Siege.Server` отсутствует).
- Запланированы: REST `POST /api/games`, `POST /api/games/{id}/join`, `GET /api/games/{id}`; SignalR-хаб `/hubs/game` (`docs/architecture.md` разделы 5.1 и 5.2).

**Исходящие:**
- Нет.

**Реальное время (запланировано):**
- SignalR между браузером и сервером: клиент отправляет `SubmitMove`, сервер рассылает `GameState`, `PresenceChanged`, `MoveRejected`.
- Прокси разработки: Vite должен проксировать `/api` и `/hubs` (включая WebSocket) на сервер. В `client/vite.config.ts` прокси пока не настроен, блок `server.proxy` отсутствует.

---

*Integration audit: 2026-10-27*
