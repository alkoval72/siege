# Phase 2: M1 Движок: поле, выставление, правило короля - Context

**Gathered:** 2026-10-27
**Status:** Ready for planning

<domain>
## Phase Boundary

Движок `Siege.Engine` начинает партию (`Game.Start`), перечисляет допустимые выставления (`Game.GetLegalMoves`) и применяет их (`Game.Apply`), включая обязательное выставление короля (`Sovereign`) не позднее 4-го хода игрока. Перемещения фишек (Phase 3) и конец партии с сериализацией и `Replay` (Phase 4) в фазу не входят.

Требования: `REQ-pieces-set`, `REQ-engine-extensibility`, `REQ-board-geometry`, `REQ-turn-action`, `REQ-placement`, `REQ-sovereign-deadline`.

</domain>

<decisions>
## Implementation Decisions

### Условие выставления (OQ-6, закрыт)
- **D-01:** Действует формулировка `docs/spec.md` §5.2: после первых двух фишек новая фишка ставится в пустую клетку, которая касается хотя бы одной фишки своей команды и не касается фишек соперника. Клетка, не касающаяся ни одной фишки, недопустима. `.planning/ARCHITECTURE-TARGET.md` §4.4 правится под spec (добавляется «хотя бы одной своей»).
- **D-02:** Первая фишка второго игрока: `GetLegalMoves` возвращает все 6 соседних с центром клеток для каждого типа фишки из запаса. Симметричные варианты не схлопываются.
- **D-03:** Ограничений на тип выставляемой фишки нет, кроме правила короля (D-06). Любая фишка из запаса, включая короля, допустима с первого хода.

### Ход игрока, правило короля и отсутствие ходов (OQ-4a, закрыт)
- **D-04:** Пропуска хода в игре нет. `PassMove` убирается из модели ходов (`.planning/ARCHITECTURE-TARGET.md` §4.2, §4.4, §4.5). Меняется `docs/spec.md` §5.4; требование `REQ-pass-turn` (Phase 4) заменено на `REQ-no-moves-loss`. **Reversibility:** reversible: контракты REST/SignalR и формат хранения ходов ещё не реализованы; после Phase 5 возврат потребовал бы правки контракта и данных.
- **D-05:** Если у игрока в начале его хода нет ни одного допустимого хода (выставления или перемещения), ему засчитывается поражение: партия заканчивается победой соперника.
- **D-06:** Каждый ход игрока это выставление или перемещение, поэтому счётчик ходов команды считает все её ходы. «Не позднее 4-го хода» значит: после трёх своих ходов без короля допустимо только выставление короля. Если на этом ходу королю нет допустимой клетки, применяется D-05 (поражение).
- **D-07:** Порядок проверок после хода: сначала окружение королей (победа или ничья по `docs/spec.md` §5.4), и только если партия не закончена, проверяется наличие допустимых ходов у того, чей ход (D-05).

### Выставление рядом со стопкой (OQ-4b, закрыт)
- **D-08:** Для условий «касается своей» и «не касается чужой» стопка считается цвета верхней фишки, нижние фишки не учитываются (согласуется с `docs/spec.md` §5.1 и `.planning/ARCHITECTURE-TARGET.md` §4.4).
- **D-09:** Выставлять можно только в пустую клетку. Наверх занятой клетки фишка попадает только движением `Climber` (Phase 3).

### Статус документов (OQ-3, закрыт)
- **D-10:** Решения D-01..D-09 переносятся в `docs/spec.md` и `.planning/ARCHITECTURE-TARGET.md` (с записями в их журналах решений: `spec.md` §12, `ARCHITECTURE-TARGET.md` §11) сразу после записи этого CONTEXT.md, до `/gsd-plan-phase 2`. Исследователь и планировщик читают уже согласованные документы.
- **D-11:** Шапка `docs/spec.md` меняется на «v0.3, открытые вопросы: формула высот `Climber`, возврат `Scout` (решаются в M2)». В `.planning/ARCHITECTURE-TARGET.md` §12 остаются пункты 1 (`Climber`) и 4 (`Scout`), пункты 2 и 3 закрыты этим обсуждением. Ссылка «раздел 11» в §4.4 исправляется на §12.

### Claude's Discretion
- Где реализуется переход партии в «поражение» по D-05. Правило зафиксировано сейчас, но статусы и конец партии относятся к Phase 4 (M3, `REQ-win-condition`, `REQ-draw-condition`). В Phase 2 достаточно, чтобы `GetLegalMoves` корректно возвращал пустой список (в том числе на 4-м ходу без клетки для короля), а смену статуса реализовать в Phase 4. Планировщик может перенести смену статуса в Phase 2, если это проще для тестов.
- Форма `GameOptions`, тексты `IllegalMoveException`, порядок ходов в списке `GetLegalMoves`: по `.planning/ARCHITECTURE-TARGET.md` §4.2-4.3 и `.claude/rules/engine.md`.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Правила игры (источник истины для движка)
- `docs/spec.md` §4: набор фишек и запас по 11 у команды.
- `docs/spec.md` §5.1-5.2: общие правила и выставление (после D-10 содержит решения этой фазы).
- `docs/spec.md` §5.4-5.5: конец игры (после D-10 без пропуска хода) и край поля.
- `docs/spec.md` §12: журнал решений.

### Движок
- `.planning/ARCHITECTURE-TARGET.md` §4.1-4.4: поле, модель (`GameState`, `Move`), публичный интерфейс `Game`, алгоритмы выставления.
- `.planning/ARCHITECTURE-TARGET.md` §9: тестирование движка.
- `.planning/ARCHITECTURE-TARGET.md` §11-12: журнал решений и оставшиеся точки сверки.

### Планирование и правила проекта
- `.planning/REQUIREMENTS.md`: `REQ-pieces-set`, `REQ-engine-extensibility`, `REQ-board-geometry`, `REQ-turn-action`, `REQ-placement`, `REQ-sovereign-deadline`; `REQ-no-moves-loss` (Phase 4, бывший `REQ-pass-turn`) по D-04/D-05.
- `.planning/INGEST-CONFLICTS.md`: исходные формулировки OQ-3, OQ-4, OQ-6.
- `.claude/rules/engine.md`: правила кода движка.
- `.claude/skills/engine-test/`: как писать тесты на правило (обязательны для каждого правила по `CLAUDE.md`).

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `server/Siege.Engine/Hex.cs`: `Hex(Q, R)`, `Directions` (6 направлений), `Center`, `DistanceFromCenter`, `Neighbors()` без учёта границ. Основа для проверки поля радиуса из `GameOptions` и соседства при выставлении.
- `server/Siege.Engine.Tests/HexTests.cs`: образец стиля тестов (`Метод_Условие_Результат`, xUnit).

### Established Patterns
- Пространство имён `Siege.Engine`, file-scoped, один публичный тип на файл, XML-doc на русском.
- `TreatWarningsAsErrors` включён, `dotnet format --verify-no-changes` входит в проверку.
- Ход проверяется только через членство в `GetLegalMoves` (`CLAUDE.md`): `Apply` не содержит второго набора правил.

### Integration Points
- Новые типы (`Team`, `PieceType`, `Piece`, `Move`, `PlaceMove`, `RelocateMove`, `GameState`, `GameOptions`, `Game`, `IllegalMoveException`) появляются в `server/Siege.Engine/`; `PassMove` не создаётся (D-04).
- Сервер (`Siege.Server`) пока отсутствует; движок остаётся чистой библиотекой без зависимостей.

</code_context>

<specifics>
## Specific Ideas

- Правило «нет ходов = поражение» владелец ввёл как общее правило игры, а не только для 4-го хода.

</specifics>

<deferred>
## Deferred Ideas

- Реализация и тест `REQ-no-moves-loss` (нет допустимых ходов = поражение): Phase 4 (M3), вместе с победой и ничьей.
- Оставшиеся точки сверки: формула высот `Climber` (OQ-2) и возврат `Scout` (OQ-1): `/gsd-discuss-phase 3`.
- Статус партии `InProgress`/`Won`/`Draw` против `"active"` (OQ-5): Phase 5.

</deferred>

---

*Phase: 02-m1-dvizhok-pole-vystavlenie-pravilo-korolya*
*Context gathered: 2026-10-27*
