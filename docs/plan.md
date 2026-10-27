# План работ: Siege

Обновлено: 2026-10-27.

Это handoff-файл. Выполненное отмечается `[x]`, подробности решений и ловушек записываются в `docs/progress.md`. Вся работа ведётся в ветке `develop`.

## 1. ДЗ OTUS, модуль 1: AI-ориентированная среда разработки

### Шаг 0. Подготовка

- [x] Ветка `develop`, `docs/spec.md` и `docs/architecture.md` в git
- [x] Стек зафиксирован: .NET 8 + xUnit + `dotnet format`; Vue 3 + TS + Vite + Pinia, npm, ESLint + Oxlint + Prettier, Vitest, Playwright
- [x] Агент: Claude Code, инструкции в `CLAUDE.md` (без `AGENTS.md`)
- [x] Обновить `spec.md` §11: вся разработка в `develop`, `main` для стабильных состояний (на 2026-10-27 уже записано в §11)

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
- [ ] Hook `SessionStart`: подставляет в контекст незакрытые пункты `plan.md` и последнюю запись `progress.md` (опционально; перенесено в §2, шаг 3)

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

## 2. ДЗ OTUS, модуль 2: агентно-ориентированный пайплайн

Цель: GSD поверх существующих документов, память, быстрые команды и hooks, управление контекстом, Discovery. Уже сделанное в §1 не повторяется, а описывается в README как результат модуля 2.

### Шаг 0. Сверка с модулем 1

- [x] Сверено 2026-10-27: `plan.md`, `progress.md`, `architecture.md` и этапы M0-M7 (`architecture.md` §10) уже есть; память (`plan.md` + `progress.md` + `/handoff`, авто-память Claude Code) показана в шаге 9; `/verify`, `/handoff`, `engine-test`, `protect-files.sh`, `lint.sh` уже работают
- [x] Новое для модуля 2: GSD, roadmap, `security-checklist`, hooks `SessionStart` и `PreCompact`, управление контекстом со скриншотами, Discovery, раздел README

### Шаг 1. GSD и инициализация проекта

Решения владельца от 2026-10-27:

- пакет `@opengsd/gsd-core` (https://github.com/open-gsd/gsd-core), установка локально в `.claude/` проекта, файлы GSD в git;
- после инициализации GSD становится источником истины для плана и состояния: `.planning/` (`PROJECT.md`, `REQUIREMENTS.md`, `ROADMAP.md`, `STATE.md`, `phases/`); `docs/plan.md` и `docs/progress.md` замораживаются после сдачи ДЗ модуля 2;
- GSD сам не коммитит, коммиты делает владелец, `git commit` остаётся в `ask`;
- `.claude/CLAUDE.md`, который генерирует `/gsd-new-project`, остаётся как есть: любые правки в репозитории идут только через команды GSD (`/gsd-fast`, `/gsd-quick`, `/gsd-debug`, `/gsd-execute-phase`), иначе `STATE.md` расходится с проектом.

Задачи:

- [x] Установка: `npx @opengsd/gsd-core@latest --claude --local` (v1.19.0): 72 команды в `.claude/commands/`, 35 агентов в `.claude/agents/`, ядро в `.claude/gsd-core/`, hooks `gsd-*` в `.claude/hooks/`
- [x] Команды `/gsd-*` видны в сессии Claude Code
- [x] Сравнить `.claude/settings.json` до и после установки: не изменился, наши `protect-files.sh` и `lint.sh` на месте; hooks GSD и права на `.planning/*` записаны в `.claude/settings.local.json` (в `.gitignore`)
- [x] Решение владельца: hooks, права и `worktree.baseRef` GSD перенесены в `settings.json` после наших hooks, путь `/snap/node/...` убран из поиска `node`; `settings.local.json` пустой; hook `gsd-secret-read-guard.js` проверен pipe-тестом
- [ ] Перезапустить Claude Code и проверить hooks GSD в живой сессии
- [ ] Отключить автокоммиты GSD: ключ `commit_docs: false` в `.planning/config.json` (по умолчанию `true`, при `false` команда `gsd_run query commit` пропускает коммит). `/gsd-new-project` коммитит `PROJECT.md` до вопроса о настройках, поэтому `config.json` нужен заранее: проверить, что заранее созданный файл не ломает инициализацию; до этого отклонять запросы на запуск `gsd_run query commit`
- [ ] `.gitignore`: в git попадают `.claude/commands/`, `.claude/agents/`, `.planning/`; временные файлы GSD (если есть) исключить
- [ ] Записать решение «план и состояние ведёт GSD» в журнал `.planning/ARCHITECTURE-TARGET.md` §11; `ARCHITECTURE-TARGET.md` §10 ссылается на `.planning/ROADMAP.md`
- [ ] `docs/security-checklist.md`: секреты и `.env` (deny + `protect-files.sh`), права агента и MCP, новые зависимости только по согласованию, проверка ходов на сервере через `GetLegalMoves`, токен игрока (хеш в БД), входные данные REST и SignalR, CORS, данные из внешних источников (prompt injection); выяснить, как он соотносится с `SECURITY.md` и `/gsd-secure-phase` из GSD
- [ ] Ссылки на `.planning/` и `security-checklist.md` в `CLAUDE.md` и `README.md`; `CLAUDE.md` не длиннее 200 строк

### Шаг 2. Память агента и переход на GSD

- [ ] Переписать раздел «Память между сессиями» в `CLAUDE.md`: в начале сессии `/gsd-resume-work` или `/gsd-progress` (чтение `STATE.md`), в конце `/gsd-pause-work`; `plan.md` и `progress.md` упоминаются как архив модулей 1-2
- [ ] Согласовать корневой `CLAUDE.md` с `.claude/CLAUDE.md` от GSD: раздел «Порядок работы» (план через `/gsd-discuss-phase` и `/gsd-plan-phase`, правки через команды GSD), дубли стека и конвенций убрать из одного из файлов; решение записать в журнал `.planning/ARCHITECTURE-TARGET.md` §11
- [ ] Skill `/handoff`: удалить или переделать в обёртку над `/gsd-pause-work` (решить при переходе, согласовать с владельцем)
- [ ] `.claude/rules/markdown.md`, раздел «Документы проекта»: правила для `.planning/` вместо `plan.md` и `progress.md`
- [ ] В README описать память: `STATE.md` и артефакты фаз GSD в git, авто-память Claude Code (`MEMORY.md`), архив `plan.md` и `progress.md`; MemPalace (`/gsd-mempalace-*`) и Claude Mem не подключаем без отдельного согласования
- [ ] Заморозить `plan.md` и `progress.md`: последняя запись в `progress.md` и строка в начале `plan.md` со ссылкой на `.planning/`

### Шаг 3. Быстрые команды и auto hooks

- [ ] Hook `SessionStart`: подставляет в контекст текущую фазу и следующий шаг из `.planning/STATE.md` и `git log --oneline -5`
- [ ] Hook `PreCompact`: перед сжатием контекста напоминает агенту сохранить чек-пойнт (`/gsd-pause-work`) или пишет служебную отметку
- [ ] Проверка hooks агентом: pipe-тест скриптов и живая сессия
- [ ] Таблица быстрых команд в README: `/verify`, основные `/gsd-*` (`onboard`, `progress`, `next`, `discuss-phase`, `plan-phase`, `pause-work`, `resume-work`, `health`), встроенные `/context`, `/compact`, `/rewind`, `/usage`

### Шаг 4. Управление контекстом

- [ ] Инструмент: автосжатие Claude Code + `/compact` с указанием, что сохранить; чек-пойнты через `/rewind` и `/gsd-pause-work`; субагенты GSD с чистым контекстом; проверка заполнения `/gsd-health --context`; опционально строка состояния с процентом заполнения контекста (`statusLine`)
- [ ] Скриншот «до»: `/context` и `/usage` в длинной сессии (`docs/screenshots/usage-before.png`)
- [ ] Сжатие контекста (`/compact` или `PreCompact` + автосжатие), продолжение работы без потери задачи
- [ ] Скриншот «после»: `/context` и `/usage` (`docs/screenshots/usage-after.png`)
- [ ] Короткое правило в `CLAUDE.md`: когда сжимать, что сохранять перед сжатием

### Шаг 5. Discovery в GSD

Объём по решению владельца: до обсуждения фазы M1, без `/gsd-plan-phase` и выполнения.

- [x] `/gsd-onboard`, затем `/gsd-map-codebase`: `.planning/codebase/` (`STACK.md`, `ARCHITECTURE.md`, `CONVENTIONS.md`, `TESTING.md`, `CONCERNS.md` и др.)
- [x] `/gsd-ingest-docs` для `spec.md` и `architecture.md`; разобрать `INGEST-CONFLICTS.md` с владельцем
- [x] `/gsd-new-project`: `PROJECT.md`, `REQUIREMENTS.md`, `ROADMAP.md` с этапами M0-M7 (из `architecture.md` §10 и `spec.md` §13), `STATE.md`; roadmap утверждает владелец (создано через `/gsd-ingest-docs docs --mode new`, отдельный `/gsd-new-project` не понадобился; утверждение roadmap владельцем не зафиксировано)
- [x] `/gsd-discuss-phase 1` для M1 (поле, выставление, правило короля): решения в `CONTEXT.md` фазы, открытые вопросы из `architecture.md` §12 (в GSD это Phase 2: `/gsd-discuss-phase 2`, `.planning/phases/02-m1-dvizhok-pole-vystavlenie-pravilo-korolya/02-CONTEXT.md`)
- [x] `/gsd-discuss-phase 3` для M2 (сверх объёма ДЗ): OQ-1 и OQ-2 закрыты, `.planning/phases/03-m2-dvizhenie-fishek-edinstvo-gruppy-skolzhenie/03-CONTEXT.md`, `spec.md` v0.4, `architecture.md` §12 закрыт
- [ ] Расхождения с `spec.md` и `ARCHITECTURE-TARGET.md` вынесены владельцу, а не решены молча; `/gsd-health` без ошибок
- [ ] Скриншоты сессии Discovery (`docs/screenshots/discovery-*.png`)

### Шаг 6. Отчёт и сдача

- [ ] `README.md`, раздел «Результаты модуля 2»: GSD и созданные файлы, roadmap, `security-checklist`, память, команды и hooks, управление контекстом со скриншотами, Discovery
- [ ] Ловушки модуля 1 закрыты: e2e-скриншот `docs/screenshots/playwright.png` переснят, `playwright.png.png` переименован, скриншоты и `.mcp.json` добавлены в git
- [ ] `/verify` зелёный, запись в `progress.md`, предложено сообщение коммита

## 3. Этапы разработки игры

Подробности в `architecture.md` §10. После шага 5 из §2 этапы ведутся в `.planning/ROADMAP.md`, список ниже не обновляется.

- [ ] M0. Каркас репозитория, команды, `CLAUDE.md`, права, хук форматирования
- [ ] M1. Движок: поле, выставление, правило короля
- [ ] M2. Движение всех фишек, единство группы, скольжение, случайные партии
- [ ] M3. Победа, ничья, пропуск хода, сериализация, `Replay`
- [ ] M4. Сервер: REST, хаб, память; клиент: поле, подсветка, локальный режим
- [ ] M5. Онлайн-режим, PostgreSQL, миграции, комнаты по ссылке
- [ ] M6. Переподключение, присутствие, смоук-тест, доработка интерфейса
- [ ] M7. Оформление: SVG-фишки и фон, тема
