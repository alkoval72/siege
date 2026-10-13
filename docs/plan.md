# План работ: Siege

Обновлено: 2026-10-13.

Это handoff-файл. Выполненное отмечается `[x]`, подробности решений и ловушек записываются в `docs/memory.md`. Вся работа ведётся в ветке `develop`.

## 1. ДЗ OTUS: AI-ориентированная среда разработки

### Шаг 0. Подготовка

- [x] Ветка `develop`, `docs/spec.md` и `docs/architecture.md` в git
- [x] Стек зафиксирован: .NET 8 + xUnit + `dotnet format`; Vue 3 + TS + Vite + Pinia, npm, ESLint + Oxlint + Prettier, Vitest, Playwright
- [x] Агент: Claude Code, инструкции в `CLAUDE.md` (без `AGENTS.md`)
- [ ] Обновить `spec.md` §11: вся разработка в `develop`, `main` для стабильных состояний

### Шаг 1. Каркас проекта

Готовый каркас лежит в `origin/develop` (старая ветка), его можно перенести через `git checkout origin/develop -- <пути>`.

- [ ] `server/Siege.sln`: `Siege.Engine`, `Siege.Engine.Tests` (xUnit), `Siege.Server` (`GET /health` на порту 5000)
- [ ] `server/Directory.Build.props` (`TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`), `global.json`, корневой `.editorconfig`
- [ ] `client/`: Vue 3 + TS + Vite + Pinia + Router, ESLint + Oxlint + Prettier, Vitest, Playwright (Chromium 1920×1080), прокси `/api` и `/hubs`
- [ ] `.gitignore`: `node_modules`, артефакты Playwright, `.env.*` кроме `.env.example`, `.claude/settings.local.json`, `CLAUDE.local.md`
- [ ] Проверка: `dotnet build`, `dotnet test`, `dotnet format --verify-no-changes`, `npm run lint`, `npm run type-check`, `npm run test:unit`

### Шаг 2. Инструкции для агента

- [x] `CLAUDE.md`: проект и стек, команды (в том числе один тест), проверка результата, конвенции, правила работы, память; не больше 200 строк
- [x] В `CLAUDE.md` переопределено глобальное правило о тестах: агент сам запускает проверки и пишет тесты к движку
- [x] `architecture.md`: доступ к данным без Repository, структура файлов агента в §3
- [ ] Сверить команды в `CLAUDE.md` с каркасом после шага 1

### Шаг 3. Память между сессиями

- [x] `docs/plan.md` с чекбоксами
- [ ] `docs/memory.md`: журнал «Сделано / Решения / Дальше / Ловушки», первая запись
- [ ] Hook `SessionStart`: подставляет в контекст незакрытые пункты `plan.md` и последнюю запись `memory.md` (опционально)

### Шаг 4. Правила по путям

- [x] `.claude/rules/markdown.md` (`paths: "**/*.md"`): оформление Markdown
- [ ] `.claude/rules/engine.md` (`paths: server/Siege.Engine/**`, `server/Siege.Engine.Tests/**`): чистота движка, неизменяемость, тест на каждое правило, XML-doc
- [ ] `.claude/rules/client.md` (`paths: client/src/**`): `<script setup lang="ts">`, клиент не вычисляет правила, подсветка «цвет + значок»
- [ ] Код для проверки: `Hex` и `HexGrid` в движке, `client/src/game/hex.ts`
- [ ] Эксперимент: одна и та же задача в движке и в клиенте, сравнение поведения и вывода `/memory`, скриншот

### Шаг 5. Skills

- [ ] `engine-test`: агент вызывает сам, пишет xUnit-тесты к правилу движка и прогоняет их
- [ ] `/verify`: запуск вручную (`disable-model-invocation: true`), сборка, линтеры, все тесты, отчёт и сообщение коммита
- [ ] `/handoff`: запуск вручную, обновляет `memory.md` и отмечает пункты в `plan.md`

### Шаг 6. Права доступа

- [ ] `.claude/settings.json`, allow: `dotnet build|test|format`, `npm run lint|type-check|test:unit|test:e2e|build`, `npx vitest`, `npx playwright test`, `git status|diff|log|add`
- [ ] ask: `git push`, `git commit`, `npm install`, `docker compose`, `git checkout -b`, `git switch -c`, `git merge`
- [ ] deny: `Read(./.env)`, `Read(./.env.*)`, `Bash(cat .env*)`, `Bash(rm -rf:*)`, `Bash(git reset --hard:*)`, `Bash(git push --force:*)`, `Bash(git push origin main:*)`
- [ ] `defaultMode: acceptEdits`, обоснование одной строкой в README
- [ ] Личные настройки только в `.claude/settings.local.json` (в `.gitignore`)

### Шаг 7. Hooks

- [ ] `PreToolUse` на `Edit|Write|MultiEdit|Bash` → `.claude/hooks/protect-files.sh`: блокирует `.env*`, `.git/`, lock-файлы, выход с кодом 2
- [ ] `PostToolUse` на `Edit|Write|MultiEdit` → `.claude/hooks/lint.sh`: `dotnet format --include` для `*.cs`, `eslint --fix` и `prettier` для `*.ts` и `*.vue`; при оставшихся ошибках выход с кодом 2
- [ ] Проверка: просьба изменить `.env` (блокировка, скриншот), исправление файла с ошибкой форматирования (линтер, скриншот)

### Шаг 8. Проверка в браузере

- [ ] `.mcp.json`: Playwright MCP (`npx @playwright/mcp@latest`)
- [ ] `BoardView`: SVG-поле из 331 клетки острой вершиной вверх, клик по центру подсвечивает клетку (зелёный фон и точка)
- [ ] Проверка агентом через MCP: сценарий, скриншот, консоль без ошибок
- [ ] `client/e2e/board.spec.ts`: 331 клетка, подсветка по клику, нет ошибок в консоли; прогон зелёный, скриншот

### Шаг 9. Демонстрация памяти

- [ ] Сессия 1: часть задачи и `/handoff`
- [ ] Сессия 2: «продолжи работу», агент опирается на `plan.md` и `memory.md`, скриншот

### Шаг 10. Отчёт и сдача

- [ ] `docs/about.md`: стек и суть проекта, правило по путям и способ проверки, skills по одной строке, почему `CLAUDE.md` вместо `AGENTS.md`
- [ ] Скриншоты в `docs/screenshots/`: блокировка `.env`, срабатывание линтера, продолжение по памяти, зелёный e2e-тест
- [ ] `README.md`, раздел «Как агент работает в проекте»: какие инструкции читает, права и режим по умолчанию, hooks, запуск тестов
- [ ] Финальный `/verify`, коммит и push `develop` (делает владелец), ссылка на репозиторий

## 2. Этапы разработки игры

Подробности в `architecture.md` §10.

- [ ] M0. Каркас репозитория, команды, `CLAUDE.md`, права, хук форматирования
- [ ] M1. Движок: поле, выставление, правило короля
- [ ] M2. Движение всех фишек, единство группы, скольжение, случайные партии
- [ ] M3. Победа, ничья, пропуск хода, сериализация, `Replay`
- [ ] M4. Сервер: REST, хаб, память; клиент: поле, подсветка, локальный режим
- [ ] M5. Онлайн-режим, PostgreSQL, миграции, комнаты по ссылке
- [ ] M6. Переподключение, присутствие, смоук-тест, доработка интерфейса
- [ ] M7. Оформление: SVG-фишки и фон, тема
