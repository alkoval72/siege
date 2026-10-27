# Phase 5: M4 Сервер и клиент: локальный режим - Context

**Gathered:** 2026-10-27
**Status:** Ready for planning

<domain>
## Phase Boundary

Появляется проект `Siege.Server` (ASP.NET Core 8): REST для создания и чтения партии, хаб SignalR `/hubs/game`, реестр активных партий в памяти. Клиент на Vue 3 получает экраны `HomeView` и `GameView`, поле в SVG на 331 клетку, панели запаса, индикатор очереди, подсветку допустимых и недопустимых клеток, анимацию хода и сообщение о результате. Двое играют в одном браузере (hot-seat) от старта до победы или ничьей, каждый ход проверяет серверный движок.

Требования: `REQ-turn-order`, `REQ-server-authority`, `REQ-hotseat-mode`, `REQ-highlight`, `REQ-ui-widescreen`, `REQ-ui-panels`.

Не входит в фазу: PostgreSQL, анонимная идентичность онлайн-игроков, ссылки-приглашения (Phase 6), надёжное переподключение и присутствие (Phase 7), SVG-фишки и оформление темы (Phase 8).

Фаза зависит от движка Phase 2-4 (`Game.Start`, `GetLegalMoves`, `Apply`, статусы и конец партии). На 2026-10-27 в движке есть только `Hex`, а Phase 4 ещё не обсуждалась.

</domain>

<decisions>
## Implementation Decisions

### Статус и результат партии (OQ-5, закрыт)
- **D-01:** `status` в контракте `GameState` (JSON) повторяет enum движка в camelCase: `inProgress`, `won`, `draw`. Таблицы перевода между движком и контрактом нет, значение `"active"` из примера `ARCHITECTURE-TARGET.md` §5.3 заменяется. Те же значения хранятся в `games.status` в Phase 6. **Reversibility:** costly: после Phase 6 значения лежат в колонке `games.status` и в `api/types.ts`, смена потребует миграции данных и правки клиента.
- **D-02:** `result` в `GameState` равен `null`, пока партия идёт, а после окончания имеет вид `{ "winner": "red" | "blue" | null, "reason": "surrounded" | "bothSurrounded" | "noMoves" }`. Значение `surrounded` означает, что окружён король проигравшего. `bothSurrounded` означает ничью, `winner` в этом случае равен `null`. `noMoves` означает, что у проигравшего нет допустимых ходов (Phase 2 D-05). Значения `reason` совпадают с колонкой `games.result_reason` (`ARCHITECTURE-TARGET.md` §6).
- **D-03:** Движок должен сообщать причину результата: в `GameState` движка рядом со статусом и победителем появляется причина (enum с тремя значениями из D-02). Это входное требование к Phase 4. Если Phase 4 будет спланирована раньше, чем документы обновлены по D-04, планировщик Phase 4 берёт причину из этого решения.
- **D-04:** Решения D-01..D-03 переносятся в документы сразу после записи этого CONTEXT.md, до `/gsd-plan-phase 5`:
  - `.planning/ARCHITECTURE-TARGET.md`: §4.2 (причина результата в `GameState` движка), §5.3 (пример JSON: `status`, `result`, `mode: "local"`), §6 (перечень значений `games.status` и `games.result_reason`), §5.1 (поле `opponentName` по D-09), запись в журнале §11;
  - `.planning/REQUIREMENTS.md`: у `REQ-server-authority` снимается пометка OQ-5;
  - `.planning/ROADMAP.md` и `.planning/STATE.md`: OQ-5 отмечается закрытым.

### Зависимости и платформа
- **D-05:** Владелец согласовал добавление пакетов: в `client/` `pinia`, `@microsoft/signalr`, `vue-router`; в `Siege.Server` `Serilog.AspNetCore`; в `Siege.Server.Tests` `Microsoft.AspNetCore.Mvc.Testing` и `Microsoft.AspNetCore.SignalR.Client` (по D-07). Версии выбирает исследователь. Другие пакеты требуют отдельного согласования. В движке зависимостей по-прежнему нет.
- **D-06:** `Siege.Server` и `Siege.Server.Tests` создаются на .NET 8, как движок. Переход всего решения на .NET 10 выполняется отдельно, одним шагом, в момент, который выберет владелец.

### Тесты
- **D-07:** Интеграционные тесты сервера пишутся в этой фазе: проект `server/Siege.Server.Tests`, `WebApplicationFactory` и клиент SignalR. Минимальные сценарии: создание локальной партии, ход, отказ по `illegal_move`, `stale_version` и `not_your_turn`, в локальном режиме один токен ходит за обе команды. Testcontainers и PostgreSQL появятся в Phase 6.
- **D-08:** Тесты клиента: Vitest для `game/hex.ts` и `game/highlight.ts` (зелёные клетки, красные клетки у группы, пустое поле с подсветкой только центра) и один e2e в Playwright: создать локальную партию и сделать несколько ходов. Заглушка `client/e2e/vue.spec.ts` заменяется. Для e2e нужен запущенный сервер (см. Claude's Discretion).

### Сценарий hot-seat
- **D-09:** На `HomeView` вводятся два имени («Игрок 1», «Игрок 2», каждое до 24 символов). Сервер бросает жребий и назначает команды. Контракт `POST /api/games` получает необязательное поле `opponentName`, обязательное при `mode: "local"`. В `GameState` поле `players` заполняется обоими именами. Индикатор очереди показывает имя и команду, например «Ходит Аня (красные)».
- **D-10:** Поле не поворачивается, экрана передачи хода нет: игра с открытой информацией. При смене очереди меняется индикатор, и активной становится панель запаса того, чей ход.
- **D-11:** После окончания партии выводится результат с причиной по D-02. Поле остаётся видно, ходы заблокированы. Кнопка «Новая партия» ведёт на `HomeView`, где поля имён заполнены именами из прошлой партии. Кнопки «Реванш» нет.
- **D-12:** Обновление страницы во время партии работает, пока сервер не перезапущен. Адрес партии `/game/:id`, `gameId` и `playerToken` лежат в `localStorage` (`ARCHITECTURE-TARGET.md` §7). При загрузке вызываются `GET /api/games/{id}` и `JoinGame`. Если партии нет в памяти (`game_not_found`, например после перезапуска сервера), показывается «Партия не найдена» и кнопка на главную. Надёжное переподключение и восстановление из БД относятся к Phase 6-7.

### Внешний вид до темы M7
- **D-13:** Фишка-заглушка `PieceToken` рисуется кругом цвета команды с буквой типа. Команды различаются не только цветом, но и обводкой (например, сплошная и двойная). Буква, цвет и обводка берутся из темы, поэтому в Phase 8 меняется только источник картинки.
- **D-14:** Каркас `client/src/themes/medieval/theme.json` создаётся в этой фазе. В нём для каждого поведенческого идентификатора лежат название (король, осадная башня, рыцарь, разведчик, гонец, по `spec.md` §4), краткое описание движения (по `spec.md` §5.3, своими словами, без текстов оригинальной игры) и буква заглушки, а для команд цвет и стиль обводки. Компоненты берут названия только из темы, в коде их нет. SVG, фон и значок осады добавляются в Phase 8.
- **D-15:** Раскладка `GameView` на 1920×1080: слева панель запаса красных, справа панель синих, поле в центре, сверху строка статуса (очередь хода, результат, уведомления). Описание выбранной фишки показывается под панелью её команды. При 1366×768 поле уменьшается через `viewBox` SVG, панели остаются читаемыми.
- **D-16:** Отказ в ходе (`MoveRejected`) и прочие ошибки показываются коротким уведомлением на русском в строке статуса, которое исчезает само через несколько секунд. Тексты сопоставлены кодам в клиенте (`illegal_move`, `not_your_turn`, `stale_version`, `game_finished`, `unauthorized`, `game_not_found`). `message` сервера пишется в консоль и в отладочный лог, пользователю не показывается. После отказа клиент запрашивает свежее состояние (`ARCHITECTURE-TARGET.md` §7).

### Claude's Discretion
- Хранение партий в Phase 5 только в памяти (`GameRegistry`, `GameSession`). Шаг «записать ход в БД» из `ARCHITECTURE-TARGET.md` §5.4 появится в Phase 6. Нужна ли сейчас точка расширения под запись ходов, решает планировщик, но слой Repository поверх EF Core не делается (`CLAUDE.md`). Токен локальной партии генерируется по `ARCHITECTURE-TARGET.md` §5.6 и пока хранится только в памяти.
- Сериализация контракта: `System.Text.Json` с camelCase для свойств и enum (`JsonStringEnumConverter` с camelCase-политикой), общая для REST и SignalR. Формат ходов совпадает с `ARCHITECTURE-TARGET.md` §4.5. Имя поля хода (`Piece` или `pieceType`) решается в Phase 4.
- Прокси Vite `/api` и `/hubs` (с WebSocket) на адрес сервера в режиме разработки. Порт сервера выбирает планировщик.
- Запуск сервера для e2e: второй элемент `webServer` в `client/playwright.config.ts` (`dotnet run`) или отдельная инструкция. Решение не должно ломать `npm --prefix client run test:e2e` без ручных шагов.
- Длительность и способ анимации перемещения по `lastMove` (CSS-переход или SMIL), подсветка последнего хода, отмена выбора фишки повторным кликом или клавишей Esc.
- Отображение стопки: верхняя фишка и счётчик высоты по `spec.md` §6. Просматривать состав стопки в фазе 5 не обязательно.
- Красная подсветка вычисляется в `game/highlight.ts` только для отображения: пустые клетки у группы за вычетом зелёных (`ARCHITECTURE-TARGET.md` §7). Правилом она не является.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Контракт и сервер
- `.planning/ARCHITECTURE-TARGET.md` §5.1-5.6: REST, SignalR, сообщение `GameState`, жизненный цикл хода, сессии и локальный режим, идентичность (после D-04 содержит D-01, D-02, D-09).
- `.planning/ARCHITECTURE-TARGET.md` §4.2-4.3, §4.5: модель движка, интерфейс `Game`, сериализация ходов.
- `.planning/ARCHITECTURE-TARGET.md` §6: схема БД, значения `games.status` и `games.result_reason` (нужны для согласования с D-01, D-02, сама БД появится в Phase 6).
- `.planning/ARCHITECTURE-TARGET.md` §11: журнал решений.

### Клиент и интерфейс
- `.planning/ARCHITECTURE-TARGET.md` §3: целевая структура `client/src/` (`api/`, `game/`, `stores/`, `components/`, `views/`, `themes/medieval/`).
- `.planning/ARCHITECTURE-TARGET.md` §7: `gameStore`, геометрия SVG, алгоритм подсветки, ход, анимация, стопки, тема, ошибки, восстановление.
- `.planning/ARCHITECTURE-TARGET.md` §8: прокси `/api` и `/hubs` в режиме разработки.
- `.planning/ARCHITECTURE-TARGET.md` §9: тесты сервера и клиента.
- `docs/spec.md` §4: фишки и их образы в теме «Средневековье» (названия для D-14).
- `docs/spec.md` §5.3-5.4: движение фишек (основа описаний в теме) и конец игры (причины результата).
- `docs/spec.md` §6: поле и интерфейс (подсветка со значками, панели, стопки, анимации, разрешения экрана).
- `docs/spec.md` §7: режимы, локальный режим первым.
- `docs/spec.md` §9: только оригинальные названия, тексты и графика.

### Предыдущие решения и правила проекта
- `.planning/phases/02-m1-dvizhok-pole-vystavlenie-pravilo-korolya/02-CONTEXT.md`: D-04, D-05, D-07 (пропуска хода нет, поражение без ходов, порядок проверок).
- `.planning/phases/03-m2-dvizhenie-fishek-edinstvo-gruppy-skolzhenie/03-CONTEXT.md`: форма `RelocateMove(From, To)` в `GetLegalMoves` (на одну цель одна запись).
- `.planning/INGEST-CONFLICTS.md`: исходная формулировка OQ-5.
- `.planning/REQUIREMENTS.md`: требования фазы, список в `<domain>`.
- `.claude/rules/client.md`: правила кода клиента.
- `.claude/rules/markdown.md`: оформление документов при правке `ARCHITECTURE-TARGET.md` по D-04.
- `CLAUDE.md`: проверки после правок (`dotnet build/test/format`, `lint`, `type-check`, `test:unit`, `test:e2e` для видимых изменений), запрет Repository, Serilog, `ConnectionStrings__Default`.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `client/src/game/hex.ts`: `hexToPixel(hex, size)` для ориентации острой вершиной вверх, типы `Hex` и `Point`. Это основа `BoardView` и `HexCell`.
- `server/Siege.Engine/Hex.cs`: `Hex`, `Directions`, `Neighbors()`. Сервер получает клетки поля через движок, а на клиенте соседство повторяется только для красной подсветки.
- `client/playwright.config.ts`: `webServer` уже поднимает `npm run dev`. Сюда добавляется запуск сервера для e2e (D-08).

### Established Patterns
- Клиент: Vue 3 + TypeScript + Vite, oxlint + eslint + prettier, Vitest (`__tests__/` рядом с модулем), Playwright (`client/e2e/*.spec.ts`).
- Сервер: пространства имён `Siege.<Проект>[.<Папка>]`, file-scoped, один публичный тип на файл, `TreatWarningsAsErrors` (`server/Directory.Build.props`), XML-doc на русском.
- Ход проверяется только членством в `GetLegalMoves`, клиент правил не знает.

### Integration Points
- Новые проекты `server/Siege.Server` и `server/Siege.Server.Tests` добавляются в `server/Siege.sln`. `Siege.Server` ссылается на `Siege.Engine`.
- `client/vite.config.ts`: прокси `/api` и `/hubs`.
- `client/src/main.ts`, `client/src/App.vue`: подключение Pinia и роутера, заглушка заменяется маршрутами `HomeView` и `GameView`.
- Движок Phase 4 должен отдавать причину результата (D-03).

</code_context>

<specifics>
## Specific Ideas

- Индикатор очереди показывает имя и цвет команды: «Ходит Аня (красные)».
- На `HomeView` после партии имена подставлены из прошлой игры, поэтому новая партия начинается в один клик.
- Для доступности команды различаются обводкой, а подсветка значком, а не только цветом (`spec.md` §6).

</specifics>

<deferred>
## Deferred Ideas

- Кнопка «Реванш» (новая партия с теми же именами в один клик): не нужна по D-11, при желании в Phase 7 или позже.
- Восстановление локальной партии после перезапуска сервера: Phase 6 (партии в БД) и Phase 7 (переподключение).
- Просмотр состава стопки по наведению: не обсуждался, при необходимости в Phase 8.
- Переход решения на .NET 10: отдельная задача, момент выбирает владелец (D-06, `STATE.md`).
- Хранение двух имён локальной партии в БД: Phase 6. По `ARCHITECTURE-TARGET.md` §6 оба `*_player_id` указывают на одного игрока, а `players.token_hash` уникален, поэтому имя второго игрока по D-09 пока некуда записать.

</deferred>

---

*Phase: 05-m4-server-i-klient-lokalnyy-rezhim*
*Context gathered: 2026-10-27*
