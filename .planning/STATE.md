---
gsd_state_version: "1.0"
milestone: v0.3
current_phase: 2
current_phase_name: "M1 Движок: поле, выставление, правило короля"
status: planning
stopped_at: Phase 5 context gathered
last_updated: "2026-10-27T13:08:33.175Z"
last_activity: 2026-10-27
last_activity_desc: roadmap создан из ingest, Phase 1 (M0) отмечена выполненной
state_head: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
progress:
  total_phases: 8
  completed_phases: 1
  total_plans: 0
  completed_plans: 0
---

# Project State

## Project Reference

See: .planning/PROJECT.md (updated 2026-10-27)

**Core value:** Двое игроков могут сыграть полную партию от старта до победы или ничьей, и каждый ход проверяется движком правил на сервере.
**Current focus:** Phase 2: M1 Движок: поле, выставление, правило короля

## Current Position

Phase: 2 of 8 (M1 Движок: поле, выставление, правило короля)
Plan: 0 of 0 in current phase
Status: Ready to plan (контекст Phase 2 собран 2026-10-27, OQ-3, OQ-4, OQ-6 закрыты; объём текущего Discovery заканчивается обсуждением M1, без `/gsd-plan-phase`)
Last activity: 2026-10-27 - Completed quick task 261027-oqg: перенос `docs/architecture.md` в `.planning/ARCHITECTURE-TARGET.md`

Progress: [█░░░░░░░░░] 13%

## Performance Metrics

**Velocity:**
- Total plans completed: 0
- Average duration: n/a
- Total execution time: 0.0 hours

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 1. M0 | 0 | выполнена до GSD | n/a |

*Updated after each plan completion*

## Accumulated Context

### Decisions

Locked-решений нет (ADR отсутствуют, все решения `proposed`), журнал в PROJECT.md Key Decisions. Решения владельца при создании roadmap:

- [Roadmap]: фазы следуют этапам M0-M7 из `ARCHITECTURE-TARGET.md` §10; нумерация сдвинута: Phase N = M(N-1), Phase 0 не используется (в инструментах GSD это служебная фаза).
- [Phase 1]: M0 выполнен в коде (Hex, HexTests, CLAUDE.md, rules, skills, hooks), помечен Complete, работа не планируется.
- [Phase 3]: варианты `Scout` сведены в `REQ-move-scout` 2026-10-27 (OQ-1 закрыт, `03-CONTEXT.md` D-01).
- [Conflicts]: циклы перекрёстных ссылок между документами (предупреждение 7) приняты владельцем как неблокирующие.

### Pending Todos

None yet.

### Blockers/Concerns

Открытых вопросов владельца нет. OQ-5 закрыт 2026-10-27 (`phases/05-m4-server-i-klient-lokalnyy-rezhim/05-CONTEXT.md`). OQ-3, OQ-4, OQ-6 закрыты 2026-10-27 (`phases/02-m1-dvizhok-pole-vystavlenie-pravilo-korolya/02-CONTEXT.md`). OQ-1, OQ-2 закрыты 2026-10-27 (`phases/03-m2-dvizhenie-fishek-edinstvo-gruppy-skolzhenie/03-CONTEXT.md`). Источник: `.planning/INGEST-CONFLICTS.md`. Решения нужно записать в `spec.md` или `ARCHITECTURE-TARGET.md` (и в их журналы решений), затем отразить здесь.

- **[Phase 3] OQ-1 и OQ-2 закрыты 2026-10-27** (`phases/03-m2-dvizhenie-fishek-edinstvo-gruppy-skolzhenie/03-CONTEXT.md`): путь `Scout` из 4 различных клеток, формула высот `Climber` `min(h1, h2) > max(hA, hB)`, по земле из двух общих клеток занята ровно одна, первоисточник правил `spec.md` v0.4.
- **[Phase 5] OQ-5 закрыт 2026-10-27** (`phases/05-m4-server-i-klient-lokalnyy-rezhim/05-CONTEXT.md`): `status` в контракте `inProgress`/`won`/`draw`, `result` = `{ winner, reason }`, причина `surrounded`/`bothSurrounded`/`noMoves`; перенесено в `ARCHITECTURE-TARGET.md` §4.2, §5.1, §5.3, §5.5, §6, §11.
- [Phase 4] Движок должен отдавать причину результата (Phase 5 D-03).

Прочие замечания из карты кодовой базы (не вопросы по правилам):

- [Phase 2] .NET 8 выходит из поддержки 2026-11-10; решить, переходить ли на .NET 10 (`TargetFramework`, `global.json`, пакеты), до роста кода движка.
- [Phase 5] Закрыто 2026-10-27 (Phase 5 D-05, D-07, D-08): зависимости `pinia`, `@microsoft/signalr`, `vue-router`, `Serilog.AspNetCore` согласованы; тесты сервера, Vitest `hex.ts`/`highlight.ts` и e2e hot-seat пишутся в фазе.
- [Phase 6] Локальная партия хранит два имени при одном токене (Phase 5 D-09), а `players.token_hash` уникален: решить, как сохранить имя второго игрока.
- [Phase 4] Имя поля хода: `Piece` в `ARCHITECTURE-TARGET.md` §4.2 и `pieceType` в JSON §4.5, зафиксировать до сериализатора.
- [Все фазы] Идентификаторы `REQ-{slug}` из ingest не соответствуют шаблону `CAT-NN` части инструментов `gsd-core` (отметка выполнения требований может потребовать ручной правки).
- [Все фазы] `commit_docs: false`: решение, коммитить ли `.planning/`, остаётся за владельцем; hooks GSD не закоммичены и не аудированы.

### Quick Tasks Completed

| # | Description | Date | Commit | Directory |
|---|-------------|------|--------|-----------|
| 261027-oqg | Перенос docs/architecture.md в .planning/ARCHITECTURE-TARGET.md, ссылки перенаправлены | 2026-10-27 | не закоммичено | [261027-oqg-perenesti-docs-architecture-md-v-plannin](./quick/261027-oqg-perenesti-docs-architecture-md-v-plannin/) |

## Deferred Items

Items acknowledged and deferred at milestone close, most recent first:

| Category | Item | Status | Deferred At | Milestone |
|----------|------|--------|-------------|-----------|
| *(none)* | | | | |

## Session Continuity

Last session: 2026-10-27T12:24:10.475Z
Stopped at: Phase 5 context gathered
Resume file: .planning/phases/05-m4-server-i-klient-lokalnyy-rezhim/05-CONTEXT.md
