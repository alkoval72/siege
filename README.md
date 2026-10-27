# Siege

Браузерная сетевая стратегия для двух игроков на гексагональном поле в средневековой теме. Сервер на ASP.NET Core 8 (движок правил `Siege.Engine`, SignalR, PostgreSQL) является единственным источником истины, клиент на Vue 3 + TypeScript рисует поле в SVG и правил не знает.

Документация:

- [spec.md](docs/spec.md): правила игры;
- [ARCHITECTURE-TARGET.md](.planning/ARCHITECTURE-TARGET.md): целевая архитектура (компоненты, контракты, схема БД, алгоритмы движка);
- [plan.md](docs/plan.md) и [progress.md](docs/progress.md): план работ и журнал прогресса.

## Как агент работает в проекте

Проект ведётся вместе с агентом Claude Code. Вся настройка агента лежит в репозитории: [CLAUDE.md](CLAUDE.md), [.claude/](.claude/) и [.mcp.json](.mcp.json).

### Какие инструкции читает агент

| Файл | Когда загружается | Что задаёт |
|---|---|---|
| `CLAUDE.md` | в начале каждой сессии | стек, команды, проверка результата, конвенции, git, память |
| `.claude/rules/engine.md` | при работе с `server/Siege.Engine/**` и тестами движка | чистота и неизменяемость движка, тест на каждое правило, XML-doc |
| `.claude/rules/client.md` | при работе с `client/src/**` | `<script setup lang="ts">`, клиент не вычисляет правила, подсветка «цвет + значок» |
| `.claude/rules/markdown.md` | при работе с `**/*.md` | оформление документации |
| `docs/spec.md`, `.planning/ARCHITECTURE-TARGET.md` | перед работой с правилами и контрактами | источник истины для движка и API |

Если код, `spec.md` и `ARCHITECTURE-TARGET.md` расходятся, агент останавливается и спрашивает владельца.

### Память между сессиями

- `docs/plan.md`: план с чекбоксами, выполненное отмечается `[x]`.
- `docs/progress.md`: журнал, после каждой задачи агент добавляет запись «Сделано / Решения / Дальше / Ловушки / Коммит».
- В начале сессии агент читает `plan.md`, последнюю запись `progress.md` и `git log --oneline -10` и продолжает с первого незакрытого пункта (`docs/screenshots/progress.png`).

### Права и режим по умолчанию

Права описаны в [.claude/settings.json](.claude/settings.json). Режим по умолчанию `acceptEdits`: правки файлов агент вносит без подтверждения, команды проходят через списки ниже.

| Список | Примеры | Поведение |
|---|---|---|
| `allow` | `dotnet build/test/format`, `npm --prefix client run *`, `npx vitest`, `npx playwright test`, `git status/diff/log/add`, `mcp__playwright__*` | выполняется без вопроса |
| `ask` | `git commit`, `git push`, `git merge`, `git switch`, `npm install`, `dotnet add`, `docker` | агент спрашивает разрешение |
| `deny` | чтение и правка `.env*`, правка `.git/` и `package-lock.json`, `rm -rf`, `git reset --hard`, `git clean`, `git push --force`, `git push origin main`, `sudo` | запрещено всегда |

- Агент не коммитит и не пушит сам: после логического блока правок он предлагает сообщение коммита в формате Conventional Commits, коммитит владелец.
- Вся работа идёт в ветке `develop`.
- Личные настройки кладутся в `.claude/settings.local.json`, он в `.gitignore`.

### Hooks

| Событие | Скрипт | Что делает |
|---|---|---|
| `PreToolUse` (`Read`, `Edit`, `Write`, `MultiEdit`, `NotebookEdit`, `Bash`) | [protect-files.sh](.claude/hooks/protect-files.sh) | блокирует доступ к `.env*`, запись в `.git/` и lock-файлы; выход с кодом 2 и причиной в stderr |
| `PostToolUse` (`Edit`, `Write`, `MultiEdit`) | [lint.sh](.claude/hooks/lint.sh) | `dotnet format` для `*.cs`, `prettier` и `eslint --fix` для `*.ts` и `*.vue`, поиск длинного тире; оставшиеся ошибки возвращает агенту |

Примеры срабатывания: отказ менять `.env` (`docs/screenshots/protected.png`), автоисправление форматирования `hex.ts` (`docs/screenshots/lint.png`).

### Skills

| Skill | Запуск | Назначение |
|---|---|---|
| `engine-test` | агент вызывает сам | пишет xUnit-тесты к правилу движка и прогоняет их |
| `/verify` | вручную | сборка, форматирование, линтеры, все тесты, отчёт и сообщение коммита |
| `/handoff` | вручную | отмечает выполненное в `plan.md`, добавляет запись в `progress.md` |

### Запуск тестов и проверка результата

После правок агент сам запускает проверки затронутой части. Задача не считается выполненной, пока они не зелёные.

| Часть | Проверки |
|---|---|
| `server/**` | `dotnet build server/Siege.sln`, `dotnet test server/Siege.sln`, `dotnet format server/Siege.sln --verify-no-changes` |
| `client/**` | `npm --prefix client run lint`, `npm --prefix client run type-check`, `npm --prefix client run test:unit` |
| видимые изменения интерфейса | дополнительно `npm --prefix client run test:e2e` и/или проверка через Playwright MCP |

Один тест запускается так:

```bash
dotnet test server/Siege.sln --filter "FullyQualifiedName=Siege.Engine.Tests.HexTests.DistanceFromCenter_Center_IsZero"
cd client && npx vitest run src/__tests__/App.spec.ts -t "название"
cd client && npx playwright test e2e/vue.spec.ts
```

Любое изменение правил в `Siege.Engine` агент сопровождает тестом сам (skill `engine-test`). Ослаблять проверки ради зелёного результата запрещено: отключать правила линтера, `TreatWarningsAsErrors`, пропускать или удалять тесты.

### Проверка в браузере

[.mcp.json](.mcp.json) подключает Playwright MCP (Chromium, 1920×1080, скриншоты в `docs/screenshots/`). Агент запускает dev-сервер в фоне, открывает `http://localhost:5173`, проходит сценарий, снимает скриншот и проверяет консоль на ошибки (`docs/screenshots/home.png`).
