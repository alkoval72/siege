# CLAUDE.md: Siege

## Проект и стек

Siege это браузерная сетевая стратегия для двух игроков на гексагональном поле. Механика близка к Hive, а оформление и тексты собственные, в средневековой теме.
Сервер является единственным источником истины: движок правил на C# (`Siege.Engine`) проверяет ходы, ASP.NET Core 8 и SignalR рассылают состояние, PostgreSQL (EF Core) хранит партии как список ходов.
Клиент на Vue 3 + TypeScript + Pinia рисует поле в SVG и правил игры не знает.

Перед работой с правилами и контрактами читай:
- `docs/spec.md`: правила игры. Для движка это источник истины.
- `docs/architecture.md`: компоненты, контракты REST и SignalR, схема БД, алгоритмы движка.

Если код, `spec.md` и `architecture.md` расходятся, остановись и спроси. Молча не выбирай.

## Команды

Все команды запускаются из корня репозитория.

**Сервер (.NET 8):**

| Действие | Команда |
|---|---|
| Восстановить пакеты | `dotnet restore server/Siege.sln` |
| Сборка | `dotnet build server/Siege.sln` |
| Запуск (http://localhost:5000) | `dotnet run --project server/Siege.Server` |
| Все тесты | `dotnet test server/Siege.sln` |
| Один класс тестов | `dotnet test server/Siege.sln --filter "FullyQualifiedName~Siege.Engine.Tests.HexTests"` |
| Один тест | `dotnet test server/Siege.sln --filter "FullyQualifiedName=Siege.Engine.Tests.HexTests.Distance_FromCenter_IsZero"` |
| Форматирование | `dotnet format server/Siege.sln` |
| Проверка форматирования | `dotnet format server/Siege.sln --verify-no-changes` |

**Клиент (Vue 3, npm):**

| Действие | Команда |
|---|---|
| Установка | `npm --prefix client ci` |
| Dev-сервер (http://localhost:5173, прокси `/api`, `/hubs` → :5000) | `npm --prefix client run dev` |
| Сборка | `npm --prefix client run build` |
| Линтер (oxlint + eslint, с автоисправлением) | `npm --prefix client run lint` |
| Форматирование / проверка | `npm --prefix client run format` / `npm --prefix client run format:check` |
| Проверка типов | `npm --prefix client run type-check` |
| Unit-тесты (Vitest) | `npm --prefix client run test:unit` |
| Один файл / один тест | `cd client && npx vitest run src/game/__tests__/hex.spec.ts -t "соседи"` |
| E2E (Playwright, сам поднимает dev-сервер) | `npm --prefix client run test:e2e` |
| Один e2e-тест | `cd client && npx playwright test e2e/board.spec.ts` |
| Браузер для e2e (один раз) | `cd client && npx playwright install chromium` |

Долгоживущие процессы (`dev`, `dotnet run`, `test:unit:watch`) запускай только в фоне. На переднем плане они блокируют сессию.

## Проверка результата

Это правило проекта. Оно переопределяет правило из `~/.claude/CLAUDE.md` «не пиши и не запускай тесты по своей инициативе».

- После правок агент сам запускает проверки затронутой части:
  - `server/**` → `dotnet build`, `dotnet test`, `dotnet format --verify-no-changes`;
  - `client/**` → `lint`, `type-check`, `test:unit`;
  - видимые изменения интерфейса → дополнительно `test:e2e` и/или проверка в браузере через Playwright MCP (сценарий, скриншот, консоль без ошибок).
- Задача не выполнена, пока проверки не зелёные. Если довести их до зелёного не удалось, сообщи об этом и приложи вывод ошибки.
- Любое изменение правил игры в `Siege.Engine` сопровождается тестом. Тест на правило агент пишет сам, без отдельной просьбы.
- Для остальных частей новые тесты пишутся только по запросу.

## Конвенции

**Общие:**
- Комментарии, XML-doc, сообщения коммитов и тексты в документации пишутся на русском. Идентификаторы в коде пишутся на английском.
- В комментариях к коду и сообщениях коммитов не используй длинное тире (U+2014). Оформление `.md`-файлов описано в `.claude/rules/markdown.md`.
- Движок называет фишки по поведению: `Sovereign`, `Climber`, `Jumper`, `Scout`, `Runner`. Тематические названия (король, рыцарь и т. д.) живут только в `client/src/themes/`.
- Правило игры меняется вместе со `spec.md`: каждое отклонение от базовых правил фиксируется там же.

**C# (`server/`):**
- Пространства имён `Siege.<Проект>[.<Папка>]`, file-scoped. Один публичный тип на файл, имя файла совпадает с именем типа.
- `Siege.Engine` не зависит ни от каких пакетов и проектов: в нём нет ASP.NET, EF, Serilog и ввода-вывода.
- Состояние движка неизменяемо: `record` / `readonly record struct`, коллекции `IReadOnly*` или `System.Collections.Immutable`. `Apply` возвращает новое состояние.
- Ход проверяется через членство в `GetLegalMoves`. Второй набор правил в сервере не пишем.
- БД: работаем с `DbContext` напрямую из сервисов, слой Repository поверх EF Core не делаем. Строка подключения берётся из `ConnectionStrings__Default`.
- Логирование через Serilog.
- Тесты xUnit: файл `<Тип>Tests.cs`, метод `Метод_Условие_Результат`, табличные случаи через `[Theory]`.
- `TreatWarningsAsErrors` включён: предупреждения исправляем, а не подавляем.

**TypeScript / Vue (`client/`):**
- Только `<script setup lang="ts">` и Composition API. Pinia-хранилища пишем в setup-стиле.
- Компоненты `PascalCase.vue`, модули `camelCase.ts`, unit-тесты лежат в `__tests__/` рядом с модулем (`src/game/__tests__/hex.spec.ts`), e2e-тесты лежат в `client/e2e/*.spec.ts`.
- Клиент правил не вычисляет: допустимые ходы берутся только из `legalMoves` сервера. Подсветка «недопустимо» нужна только для отображения.
- DTO описываются вручную в `src/api/types.ts` и зеркалят JSON сервера (camelCase, дискриминатор хода `kind`).
- Подсветка клеток всегда сочетает цвет и значок (точка или крестик).

**Где что лежит:**
```
server/Siege.Engine/        правила игры (чистая библиотека)
server/Siege.Engine.Tests/  тесты движка
server/Siege.Server/        REST, SignalR-хаб, EF Core
client/src/api/             http.ts, hub.ts, types.ts: общение с сервером
client/src/game/            hex.ts (координаты → пиксели), highlight.ts
client/src/stores/          Pinia (gameStore)
client/src/components/      BoardView, HexCell, PieceToken, ReservePanel, StatusBar
client/src/views/           HomeView, GameView
client/src/themes/medieval/ theme.json и SVG фишек
docs/                       spec, architecture, plan, memory
```

## Правила работы

**Git:**
- Вся работа ведётся в ветке `develop`. Ветки не создавай, на `main` не переключайся.
- Агент не делает `git commit` и `git push`. После логического блока правок предложи сообщение коммита на русском в формате Conventional Commits: `feat|fix|refactor|test|docs|chore(<scope>): …`, где scope: `engine`, `server`, `client`, `docs`, `agent`.

**Порядок работы:**
- Для сложной задачи сначала составь план и дождись подтверждения. Мелкие однозначные правки делай сразу.
- Не добавляй зависимости (`npm install <pkg>`, `dotnet add package`) без согласования.

**Запрещено:**
- Читать, выводить или изменять `.env` и `.env.*`. Исключение: `.env.example` без реальных значений. Секреты в код и в документацию не вставляй.
- Вручную править `.git/` и lock-файлы (`package-lock.json`). Lock-файл меняется только через `npm install`.
- Выполнять `rm -rf`, `git reset --hard`, `git clean -fd`, `git push --force`.
- Ослаблять проверки ради зелёного результата: отключать правила линтера, `TreatWarningsAsErrors`, ставить `Skip` у тестов, удалять падающие тесты.
- Копировать названия, тексты правил, графику и логотип оригинальной игры (`spec.md` §9).

## Память между сессиями

Handoff-файлы (лежат в git):
- `docs/plan.md`: план с чекбоксами;
- `docs/memory.md`: журнал. Каждая запись содержит дату и разделы «Сделано», «Решения», «Дальше», «Ловушки».

В начале сессии прочитай `docs/plan.md` и последнюю запись `docs/memory.md` и продолжай с первого незакрытого пункта.
В конце логического блока отметь выполненное в `plan.md` и добавь запись в `memory.md`.
