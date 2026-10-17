# План работ: Siege

Обновлено: 2026-10-17.

Это handoff-файл. Выполненное отмечается `[x]`, подробности решений и ловушек записываются в `docs/progress.md`. Вся работа ведётся в ветке `develop`.

## 1. ДЗ OTUS: AI-ориентированная среда разработки

### Шаг 0. Подготовка

- [x] Ветка `develop`, `docs/spec.md` и `docs/architecture.md` в git
- [x] Стек зафиксирован: .NET 8 + xUnit + `dotnet format`; Vue 3 + TS + Vite + Pinia, npm, ESLint + Oxlint + Prettier, Vitest, Playwright
- [x] Агент: Claude Code, инструкции в `CLAUDE.md` (без `AGENTS.md`)
- [ ] Обновить `spec.md` §11: вся разработка в `develop`, `main` для стабильных состояний

### Шаг 1. Каркас проекта

Каркас минимальный, только под критерии ДЗ. `Siege.Server`, Router, Pinia и прокси добавляются на этапах M4-M5.

- [x] `server/Siege.sln`: `Siege.Engine` (тип `Hex`), `Siege.Engine.Tests` (xUnit, 7 тестов)
- [x] `server/Directory.Build.props` (`TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`), `global.json`, корневой `.editorconfig` (CA1707 отключён для тестов)
- [x] `client/`: Vue 3 + TS + Vite, ESLint + Oxlint + Prettier, Vitest, Playwright (Chromium 1920×1080); `src/game/hex.ts`
- [x] `.gitignore`: `node_modules`, артефакты Playwright, `.env.*`, `.claude/settings.local.json`, `CLAUDE.local.md`
- [x] Проверка: `dotnet build`, `dotnet test`, `dotnet format --verify-no-changes`, `check:lint`, `type-check`, `test:unit` зелёные

### Шаг 2. Инструкции для агента

- [x] `CLAUDE.md`: проект и стек, команды (в том числе один тест), проверка результата, конвенции, правила работы, память; не больше 200 строк
- [x] В `CLAUDE.md` переопределено глобальное правило о тестах: агент сам запускает проверки и пишет тесты к движку
- [x] `architecture.md`: доступ к данным без Repository, структура файлов агента в §3
- [x] Сверить команды в `CLAUDE.md` с каркасом после шага 1; правила движка и клиента перенесены в `.claude/rules/`

### Шаг 3. Память между сессиями

- [x] `docs/plan.md` с чекбоксами
- [x] `docs/progress.md`: журнал «Сделано / Решения / Дальше / Ловушки / Коммит», первая запись; формат записи описан в `CLAUDE.md`
- [ ] Hook `SessionStart`: подставляет в контекст незакрытые пункты `plan.md` и последнюю запись `progress.md` (опционально)

### Шаг 4. Правила по путям

- [x] `.claude/rules/markdown.md` (`paths: "**/*.md"`): оформление Markdown
- [x] `.claude/rules/engine.md` (`paths: server/Siege.Engine/**`, `server/Siege.Engine.Tests/**`): чистота движка, неизменяемость, тест на каждое правило, XML-doc
- [x] `.claude/rules/client.md` (`paths: client/src/**`): `<script setup lang="ts">`, клиент не вычисляет правила, подсветка «цвет + значок»
- [x] Код для проверки: `Hex` в движке, `client/src/game/hex.ts`
- [ ] Эксперимент: одна и та же задача в движке и в клиенте, сравнение поведения и вывода `/memory`, скриншот

### Шаг 5. Skills

- [x] `engine-test`: агент вызывает сам, пишет xUnit-тесты к правилу движка и прогоняет их
- [x] `/verify`: запуск вручную (`disable-model-invocation: true`), сборка, линтеры, все тесты, отчёт и сообщение коммита
- [x] `/handoff`: запуск вручную, обновляет `progress.md` и отмечает пункты в `plan.md`

### Шаг 6. Права доступа

- [x] `.claude/settings.json`, allow: `dotnet build|test|format`, `npm run lint|type-check|test:unit|test:e2e|build`, `npx vitest`, `npx playwright test`, `git status|diff|log|add`
- [x] ask: `git push`, `git commit`, `npm install`, `docker compose`, `git checkout -b`, `git switch -c`, `git merge`
- [x] deny: `Read(./.env)`, `Read(./.env.*)`, `Bash(cat .env*)`, `Bash(rm -rf *)`, `Bash(git reset --hard *)`, `Bash(git push --force *)`, `Bash(git push origin main *)`, `Edit(**/package-lock.json)`
- [x] `defaultMode: acceptEdits`
- [x] Личные настройки только в `.claude/settings.local.json` (в `.gitignore`)

### Шаг 7. Hooks

- [x] `PreToolUse` на `Read|Edit|Write|MultiEdit|NotebookEdit|Bash` → `.claude/hooks/protect-files.sh`: блокирует `.env*`, `.git/`, lock-файлы, выход с кодом 2
- [x] `PostToolUse` на `Edit|Write|MultiEdit` → `.claude/hooks/lint.sh`: `dotnet format --include` для `*.cs`, `prettier` и `eslint --fix` для `*.ts` и `*.vue`, поиск длинного тире; при оставшихся ошибках выход с кодом 2
- [x] Проверка агентом: 13 случаев защиты через pipe-тест, в живой сессии заблокирован `grep … .env`, Prettier исправил испорченный `hex.ts`
- [x] Скриншот: владелец создаёт `.env` и просит агента изменить его, агент отказывается (`docs/screenshots/protected.png`)
- [x] Скриншот: владелец портит форматирование `hex.ts` и просит агента исправить, срабатывает `lint.sh` (`docs/screenshots/lint.png`)

### Шаг 8. Проверка в браузере

- [x] Браузер Chromium для Playwright установлен, `npm --prefix client run test:e2e` зелёный: 1 тест (`e2e/vue.spec.ts`), скриншот `docs/screenshots/playwright.png`
- [x] `.mcp.json`: Playwright MCP (`npx @playwright/mcp@latest`, Chromium, 1920×1080, скриншоты в `docs/screenshots/`); в `settings.json` `enabledMcpjsonServers` и `mcp__playwright__*` в `allow`
- [x] Проверка агентом через MCP: открыт `localhost:5173`, заголовок «Siege», 0 ошибок в консоли; скриншот страницы `docs/screenshots/home.png`, скриншот сессии `docs/screenshots/playwright.png.png`

### Шаг 9. Демонстрация памяти

- [x] Сессия 1: часть задачи, запись в `progress.md` по правилу `CLAUDE.md`
- [x] Сессия 2: «продолжи работу», агент прочитал `plan.md`, `progress.md`, `git log` и продолжил с открытого пункта (`docs/screenshots/progress.png`)

### Шаг 10. Отчёт и сдача

- [x] `README.md`, раздел «Как агент работает в проекте»: какие инструкции читает, права и режим по умолчанию, hooks, запуск тестов

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
