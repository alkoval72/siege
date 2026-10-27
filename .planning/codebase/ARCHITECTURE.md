---
last_mapped_commit: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
last_mapped_at: 2026-10-27
---
<!-- refreshed: 2026-10-27 -->
# Architecture

**Analysis Date:** 2026-10-27

## Статус документа

Документ разделяет две вещи: то, что реализовано в коде сейчас, и то, что описано только в целевой архитектуре `docs/architecture.md` (v0.1). Реализовано на момент анализа:

- `server/Siege.Engine/` (библиотека правил, один тип `Hex`) и `server/Siege.Engine.Tests/` (xUnit);
- `client/` (Vue 3 приложение-заглушка: `main.ts`, `App.vue`, геометрия `game/hex.ts`).

Не реализовано (только в `docs/architecture.md`): `Siege.Server` (REST, SignalR, EF Core), `GameState`, `Game.Apply`, `GetLegalMoves`, Pinia-хранилище, компоненты поля, тема, PostgreSQL. Пути из целевой структуры в этом документе помечены как «планируется».

## Pattern Overview

**Overall:** Целевой паттерн: сервер как единственный источник истины с чистым доменным ядром (движок правил без зависимостей), тонкий клиент, который рисует состояние. На текущем этапе реализовано только ядро-фундамент и клиентская заглушка.

**Key Characteristics:**
- Движок `Siege.Engine` не имеет `PackageReference` и `ProjectReference` (`server/Siege.Engine/Siege.Engine.csproj` содержит только `RootNamespace`), работает только на BCL.
- Состояние проектируется неизменяемым: `Apply(state, move)` возвращает новое состояние (`docs/architecture.md` §1, §4.3). В коде пока есть только `readonly record struct Hex`.
- Единый источник допустимости ходов: ход проверяется через членство в `GetLegalMoves` (`docs/architecture.md` §11). Второго набора правил на сервере и клиенте не будет.
- Клиент не знает правил: геометрия в `client/src/game/hex.ts` только переводит координаты клетки в пиксели. Допустимые ходы клиент получит от сервера в `legalMoves` (`.claude/rules/client.md`).
- Общие настройки сборки .NET вынесены в `server/Directory.Build.props`: `net8.0`, `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`, `AnalysisLevel=latest-recommended`. Все проекты решения наследуют их.

## Layers

**Правила игры (domain core):**
- Purpose: геометрия поля и (в целевом состоянии) состояние партии, ходы, правила фишек.
- Location: `server/Siege.Engine/`
- Contains (реализовано): `Hex.cs` с `Directions` (шесть направлений в порядке обхода), `Center`, `DistanceFromCenter`, `Neighbors()`.
- Contains (планируется): `GameState`, `Move` (`PlaceMove`, `RelocateMove`, `PassMove`), `Piece`, `Team`, `PieceType`, `GameOptions`, `Game` (`Start`, `GetLegalMoves`, `Apply`, `Replay`), алгоритмы Тарьяна для единства группы и скольжения (`docs/architecture.md` §4).
- Depends on: только BCL (`System`, `System.Collections.Generic`, `System.Linq`).
- Used by: тестами `server/Siege.Engine.Tests/` и (планируется) `Siege.Server`.

**Тесты движка:**
- Purpose: проверка геометрии и (в будущем) правил.
- Location: `server/Siege.Engine.Tests/`
- Contains: `HexTests.cs` (фактов и теорий xUnit), `GlobalUsings.cs` (глобальный `using Xunit;`).
- Depends on: `Siege.Engine` через `ProjectReference`, пакеты xunit 2.4.2, Microsoft.NET.Test.Sdk 17.6.0, coverlet.collector 6.0.0.
- Used by: `dotnet test server/Siege.sln`.

**Сервер (планируется):**
- Purpose: REST (создание, вход, чтение состояния), SignalR-хаб `/hubs/game`, активные сессии в памяти, запись ходов в БД.
- Location: `server/Siege.Server/` (не создан, `docs/architecture.md` §3, §5).
- Contains (по плану): REST-контроллеры, `GameHub`, `GameService`, `GameRegistry`, `GameSession`, `DbContext` (EF Core + Npgsql).
- Depends on: `Siege.Engine`, ASP.NET Core 8, SignalR, EF Core, Serilog (по `CLAUDE.md`).
- Used by: клиент через REST и SignalR.

**Клиент (реализовано частично):**
- Purpose: отрисовка. Сейчас: точка входа и заглушка с заголовком «Siege».
- Location: `client/src/`
- Contains (реализовано): `main.ts` (создание приложения и монтирование в `#app`), `App.vue` (компонент-заглушка), `game/hex.ts` (`hexToPixel`), `__tests__/App.spec.ts`.
- Contains (планируется по `docs/architecture.md` §3 и `.claude/rules/client.md`): `api/` (`http.ts`, `hub.ts`, `types.ts`), `stores/` (Pinia `gameStore`), `components/` (`BoardView`, `HexCell`, `PieceToken`, `ReservePanel`, `StatusBar`), `views/` (`HomeView`, `GameView`), `themes/medieval/` (`theme.json`, SVG фишек).
- Depends on: `vue` (единственная runtime-зависимость в `client/package.json`). Pinia, vue-router и SignalR-клиент в `package.json` отсутствуют.
- Used by: браузер через `client/index.html` (`<div id="app">`, `<script type="module" src="/src/main.ts">`).

**Документация как спецификация:**
- Purpose: источник истины для правил и контрактов.
- Location: `docs/spec.md` (правила, §4-5), `docs/architecture.md` (компоненты, контракты, схема БД, §4 алгоритмы), `CLAUDE.md`.
- Используется агентом перед работой с правилами (`CLAUDE.md`, `.claude/rules/engine.md`). Расхождение кода и документов требует вопроса владельцу, а не молчаливого выбора.

## Data Flow

### Фактически реализованный путь (сейчас)

1. Браузер загружает `client/index.html`, который подключает `client/src/main.ts`.
2. `main.ts` вызывает `createApp(App).mount('#app')` (`client/src/main.ts`).
3. `App.vue` рендерит `<h1>Siege</h1>`. Геометрия и движок в этом пути не участвуют.

Отдельно существуют два независимых потока, которые пока не соединены:
- Геометрия: `Hex` в C# (`server/Siege.Engine/Hex.cs`) и `hexToPixel` в TypeScript (`client/src/game/hex.ts`) реализуют одну и ту же формулу осевых координат. Формулы совпадают с `docs/architecture.md` §7: `x = s·√3·(q + r/2)`, `y = 1.5·s·r`. Общего кода между ними нет, и `hexToPixel` сейчас никем не импортируется.
- Тесты: `HexTests.cs` проверяют `Hex.Directions`, `Center`, `DistanceFromCenter` и `Neighbors()`. Тест `DistanceFromCenter_HexesWithinRadius_CountMatchesBoardSize` подтверждает, что радиус 10 дает 331 клетку (`3R(R+1)+1`).

### Целевой путь партии (по `docs/architecture.md` §5.3-5.4, не реализован)

1. Клиент отправляет `SubmitMove(gameId, expectedVersion, move)` в хаб `/hubs/game`.
2. Сервер находит `GameSession` в `GameRegistry` (при отсутствии загружает из БД и воспроизводит ходы через `Game.Replay`).
3. Сервер берет блокировку партии (`SemaphoreSlim`), проверяет токен, очередь хода и `expectedVersion`.
4. Сервер проверяет членство хода в `Game.GetLegalMoves(state)`.
5. `Game.Apply` вычисляет новое состояние; ход записывается в таблицу `moves`, и только после успешной записи состояние в памяти заменяется.
6. Сервер рассылает `GameState` группе `game:{id}`. Ошибки уходят как `MoveRejected(code, message)`.

### Состояние

- Текущее: состояния нет, кроме одного readonly record struct `Hex`, который не хранит изменяемых данных.
- Целевое: `GameState` неизменяем (`docs/architecture.md` §4.2). Истина хранится как начальные условия плюс список ходов в таблицах `games` и `moves`; снимков нет (`docs/architecture.md` §6). Клиентское состояние будет в Pinia-хранилище `gameStore` (планируется).

## Key Abstractions

**`Hex` (реализовано):**
- Purpose: клетка поля в осевых координатах `(q, r)`.
- Definition: `server/Siege.Engine/Hex.cs`, `public readonly record struct Hex(int Q, int R)`.
- Pattern: value-тип с равенством по значению. Статические `Directions` (`IReadOnlyList<Hex>`, шесть направлений в порядке `(+1,0), (+1,-1), (0,-1), (-1,0), (-1,+1), (0,+1)`) и `Center`. Экземплярные `DistanceFromCenter` и `Neighbors()`. Границы поля не учитываются: `Neighbors()` может вернуть клетки за пределами радиуса (это проверяется тестом `Neighbors_HexNearBoardEdge_IncludesHexesBeyondRadius`).

**`Move` и `GameState` (планируется):**
- Purpose: ход как иерархия записей, состояние как неизменяемый снимок.
- Pattern (по `docs/architecture.md` §4.2): `abstract record Move` с наследниками `PlaceMove(PieceType Piece, Hex To)`, `RelocateMove(Hex From, Hex To)`, `PassMove`. Сериализация JSON с полем-дискриминатором `kind` (§4.5).

**`Game` (планируется):**
- Purpose: единая точка входа в правила.
- Pattern: статический фасад `Start`, `GetLegalMoves`, `Apply`, `Replay`. Жребий первого хода передается снаружи, движок его не выбирает (`docs/architecture.md` §4.3).

**`GameRegistry` и `GameSession` (планируется):**
- Purpose: активные партии в памяти, выгрузка через 30 минут без подключений.
- Pattern: словарь сессий плюс блокировка на сессию (`docs/architecture.md` §5.5). Для одного сервера, несколько экземпляров не поддерживаются (§13).

**Ход и `legalMoves` на клиенте (планируется):**
- Клиент показывает ход, только если он есть в списке `legalMoves` от сервера. Красная подсветка «недопустимо» вычисляется на клиенте только для отображения (`.claude/rules/client.md`).

## Entry Points

**Клиентская точка входа (реализовано):**
- Location: `client/src/main.ts` (монтирование), `client/index.html` (HTML-оболочка с `#app`).
- Triggers: загрузка страницы в браузере; команды `npm --prefix client run dev` (Vite, http://localhost:5173) и `npm --prefix client run build`.
- Responsibilities: создать приложение Vue и смонтировать корневой компонент `App.vue`.

**Точка входа тестов движка (реализовано):**
- Location: `server/Siege.Engine.Tests/HexTests.cs`.
- Triggers: `dotnet test server/Siege.sln`.

**Серверная точка входа (планируется):**
- Location: `server/Siege.Server/Program.cs` (не существует).
- Responsibilities по плану: настройка DI, Serilog, EF Core с `ConnectionStrings__Default`, маршрутизация REST и `MapHub` на `/hubs/game`.

**Клиентская сборка и dev-прокси (частично):**
- Location: `client/vite.config.ts` содержит плагины `vue()` и `vueDevTools()` и алиас `@` на `client/src`. Прокси `/api` и `/hubs` (с WebSocket) по `docs/architecture.md` §8 не настроен.
- Тесты Vitest: `client/vitest.config.ts` (окружение `jsdom`, исключение `e2e/**`). Playwright: `client/playwright.config.ts`, тест `client/e2e/vue.spec.ts`.

## Architectural Constraints

- **Threading:** серверного кода нет, модель потоков не определена. Целевое решение: блокировка партии через `SemaphoreSlim` на сессию (`docs/architecture.md` §5.4). Движок чистый и потокобезопасен, пока состояние не меняется (неизменяемость).
- **Global state:** глобального изменяемого состояния в коде нет. `Hex.Directions` и `Hex.Center` являются статическими свойствами с неизменяемыми значениями (`server/Siege.Engine/Hex.cs`). Целевое хранилище активных партий (`GameRegistry`) будет единственным разделяемым состоянием сервера.
- **Circular imports:** циклов нет. Зависимости однонаправлены: `Siege.Engine.Tests` → `Siege.Engine`; клиент не зависит от сервера в коде (контракт будет описан в `client/src/api/types.ts`).
- **Зависимости движка:** `Siege.Engine` должен оставаться без `PackageReference` и `ProjectReference` (`.claude/rules/engine.md`). Попытка подключить EF, Serilog или ASP.NET в движок нарушает архитектуру.
- **Один экземпляр сервера:** активные партии в памяти, несколько серверов за балансировщиком не поддерживаются (`docs/architecture.md` §13).
- **Версии:** .NET SDK зафиксирован в `global.json` (`8.0.100`, `rollForward: latestFeature`). Цель: `net8.0`. Для нового кода избегать API, помеченных устаревшими в сторону .NET 10 (`CLAUDE.md` уровня alliance).
- **Типы клиента и сервера:** пишутся вручную (`docs/architecture.md` §7, §13). Расхождение ловится тестами, автогенерации нет.
- **Геометрия дублируется:** формула осевых координат есть в `Hex.cs` и в `client/src/game/hex.ts`. Изменение соглашения (порядок направлений, ориентация клеток) нужно вносить в обе реализации.

## Anti-Patterns

### Правила в клиенте

**What happens:** логика соседства, связности группы или допустимости ходов в TypeScript.
**Why it's wrong:** два набора правил расходятся, клиент начинает показывать ходы, которые сервер отвергнет. Нарушается принцип «клиент глупый» (`docs/architecture.md` §1, пункт 5).
**Do this instead:** брать допустимые ходы только из `legalMoves`. В `client/src/game/` держать только геометрию (`hex.ts`) и отображение (`highlight.ts`, планируется). Пример текущего правильного подхода: `client/src/game/hex.ts` содержит только `hexToPixel`.

### Мутабельное состояние движка

**What happens:** методы движка, которые меняют переданные объекты или поля списков.
**Why it's wrong:** теряется возможность воспроизведения (`Replay`), сравнения версий и отката, а также проверки инвариантов на случайных партиях (`docs/architecture.md` §9).
**Do this instead:** `record` и `readonly record struct`, коллекции `IReadOnly*` или `System.Collections.Immutable`, как в `server/Siege.Engine/Hex.cs`.

### Запись состояния до записи в БД

**What happens:** обновление состояния в памяти до успешного сохранения хода.
**Why it's wrong:** после ошибки БД память и история расходятся.
**Do this instead:** по `docs/architecture.md` §5.4: сначала запись хода в `moves`, затем замена состояния в `GameSession`. Ошибка записи не меняет память, клиент получает ошибку.

### Repository поверх EF Core

**What happens:** абстракция `IGameRepository` над `DbContext`.
**Why it's wrong:** стандарт проекта считает ее избыточной (`docs/architecture.md` §11, `CLAUDE.md`).
**Do this instead:** работать с `DbContext` напрямую из сервисов (`GameService`) или обработчиков.

### Тематические имена фишек в движке

**What happens:** названия «Король», «Рыцарь» и т. п. в коде движка и комментариях.
**Why it's wrong:** тема отделена от правил, смена оформления не должна трогать движок (`docs/architecture.md` §11, `.claude/rules/engine.md`).
**Do this instead:** использовать поведенческие имена `Sovereign`, `Climber`, `Jumper`, `Scout`, `Runner`. Тематические названия живут в `client/src/themes/medieval/theme.json` (планируется).

## Error Handling

**Strategy (реализовано):** в `Siege.Engine` ошибок и исключений пока нет. Проверки аргументов не выполняются, `Neighbors()` и `DistanceFromCenter` работают на любых целых координатах.

**Strategy (целевая):**
- `Game.Apply` бросает `IllegalMoveException`, если ход недопустим (`docs/architecture.md` §4.3).
- Сервер переводит отказы в `MoveRejected(code, message)` с кодами `stale_version`, `not_your_turn`, `illegal_move`, `game_finished`, `unauthorized`, `game_not_found` (§5.2).
- REST: 409 при отсутствии свободного места для второго игрока (§5.1).
- Клиент при `MoveRejected` запрашивает свежее состояние и показывает короткое сообщение (§7).

## Cross-Cutting Concerns

**Logging:** целевое решение Serilog (`CLAUDE.md`). В `Siege.Engine` логирования нет и не должно быть (только BCL, без ввода-вывода, `.claude/rules/engine.md`). Сейчас Serilog не подключен.

**Validation:** в движке ходы проверяются членством в `GetLegalMoves` (планируется). Сейчас проверок входных данных нет. Имя игрока обрезается до 24 символов на сервере (`docs/architecture.md` §5.6, планируется).

**Authentication:** целевая анонимная идентичность. Сервер выдает случайный токен (32 байта), в БД хранится только SHA-256 хеш, клиент хранит токен в `localStorage`. Токен передается аргументом хаба или заголовком `X-Player-Token`, никогда в адресе (`docs/architecture.md` §5.2, §5.6). В `CLAUDE.md` указан JWT на базе Identity как общий стандарт компании, но для Siege выбрана анонимная схема (расхождение с общим стандартом здесь не возникает, поскольку проект описан в собственной архитектуре).

**Configuration:** строка подключения из переменной окружения `ConnectionStrings__Default` (`docs/architecture.md` §6). Файл `.env` в анализе не читался и не должен использоваться как источник значений в документации.

**Testing:** движок тестируется xUnit (`server/Siege.Engine.Tests/`), клиент Vitest (`client/src/__tests__/`) и Playwright (`client/e2e/`). Детали в `TESTING.md` (не входит в текущий фокус).

## Открытые расхождения с целевой архитектурой

- `docs/architecture.md` §3 перечисляет `docs/about.md`, `docker-compose.yml`, `server/Siege.Server/`, `server/Siege.Server.Tests/`. На момент анализа их в репозитории нет.
- `docs/architecture.md` §4.2 описывает `GameState`, `Piece`, `Team`, `PieceType`, `Move`, которых в коде еще нет. Реализован только `Hex`.
- `client/src/` не содержит `api/`, `stores/`, `components/`, `views/`, `themes/`, `game/highlight.ts`.
- `client/package.json` не содержит Pinia и SignalR-клиента, хотя они нужны по §7 и §8 `docs/architecture.md`.

---

*Architecture analysis: 2026-10-27*
