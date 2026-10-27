---
phase: quick-261027-oqg
plan: 01
subsystem: docs
tags: [docs, planning, architecture, relink]
requires: []
provides:
  - ".planning/ARCHITECTURE-TARGET.md как единственное расположение целевой архитектуры"
affects:
  - CLAUDE.md
  - README.md
  - .claude/rules
  - .claude/skills/engine-test
  - docs/spec.md
  - docs/plan.md
  - .planning
key-files:
  created:
    - .planning/ARCHITECTURE-TARGET.md
  modified:
    - CLAUDE.md
    - README.md
    - .claude/rules/engine.md
    - .claude/rules/markdown.md
    - .claude/skills/engine-test/SKILL.md
    - docs/spec.md
    - docs/plan.md
    - .planning/PROJECT.md
    - .planning/ROADMAP.md
    - .planning/REQUIREMENTS.md
    - .planning/STATE.md
    - .planning/phases/02-m1-dvizhok-pole-vystavlenie-pravilo-korolya/02-CONTEXT.md
    - .planning/phases/03-m2-dvizhenie-fishek-edinstvo-gruppy-skolzhenie/03-CONTEXT.md
    - .planning/phases/05-m4-server-i-klient-lokalnyy-rezhim/05-CONTEXT.md
  deleted:
    - docs/architecture.md
decisions:
  - "OD-1: целевая архитектура живёт в .planning/ARCHITECTURE-TARGET.md, а не в .planning/codebase/ARCHITECTURE.md (тот файл перезаписывает /gsd-map-codebase)"
  - "OD-2: docs/spec.md в этой задаче меняется только в ссылке на архитектуру"
metrics:
  tasks: 3
  commits: 0
  duration: "около 10 минут"
  completed: 2026-10-27
status: complete
commits: 0
plan_head_before: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
plan_head_after: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
actuals:
  tokens: 50000
  tasks: 3
  commits: 0
---

# Quick 261027-oqg: перенос целевой архитектуры в .planning

Целевая архитектура перенесена из `docs/architecture.md` в `.planning/ARCHITECTURE-TARGET.md`, все живые ссылки в 14 файлах перенаправлены на новый путь.

## Что сделано

**Задача 1 (tracer): перенос, §3, §11, CLAUDE.md, README.md.**

- `docs/architecture.md` перенесён обычным `mv` в `.planning/ARCHITECTURE-TARGET.md`. Заголовок, текст и нумерация разделов 1-13 сохранены (контрольная сумма списка заголовков совпала: 13 разделов, 11 подразделов).
- §3: из блока `docs/` убрана строка с файлом архитектуры, добавлены `.planning/` и `ARCHITECTURE-TARGET.md (этот документ)` с описаниями в той же колонке.
- §11: добавлена запись «Расположение целевой архитектуры (2026-10-27)» с вариантами, выбором и причиной.
- `CLAUDE.md`: 4 правки (список чтения, правило расхождений, ссылка на §3, дерево «Где что лежит» со строкой `.planning/`). Длина файла меньше 200 строк.
- `README.md`: 3 правки (список документации, таблица инструкций, фраза о расхождениях).

**Задача 2: правила, skill, spec.md, plan.md.**

- `.claude/rules/engine.md`: 1 замена; `.claude/skills/engine-test/SKILL.md`: 1 замена; `docs/spec.md`: 1 замена (только ссылка, OD-2).
- `.claude/rules/markdown.md`: 4 замены, плюс `.planning/ARCHITECTURE-TARGET.md` добавлен в перечень файлов, на которые распространяется правило (frontmatter `paths` не менялся).
- `docs/plan.md`: перенаправлены 3 открытых пункта `- [ ]`; 7 отмеченных пунктов и проза раздела 3 не тронуты (контрольная сумма `- [x]` совпала).

**Задача 3: артефакты .planning.**

Замены с `replace_all` в порядке «полный путь, затем голое имя»: `PROJECT.md`, `ROADMAP.md`, `REQUIREMENTS.md` (плюс 7 цитат `architecture §` на `ARCHITECTURE-TARGET §`), `STATE.md` (только голые имена), `02-CONTEXT.md`, `03-CONTEXT.md`, `05-CONTEXT.md`. Число упоминаний нового имени после замен: 6, 5, 11, 4, 10, 10, 19.

## Результат итоговой проверки

Все проверки зелёные.

- Проверки задач 1, 2, 3 (`<verify>`): пройдены.
- Проверка 1 (перенос и живые ссылки, нет неверного пути `docs/ARCHITECTURE-TARGET`): пройдена.
- Проверка 2 (длинное тире в изменённых файлах): отсутствует.
- Проверка 3: контрольные суммы защищённых файлов совпали (`4eefb95fa1b211964803ed63553ebcb5` и `870b1d48102976f3a96b7cbad7ee9ae9`), `Hex.cs` и `hex.ts` не изменены, индекс пуст, статус `docs/architecture.md` равен ` D`.
- Проверки `dotnet` и `npm` не запускались: код не менялся.

## Отклонения от плана

Нет, план выполнен как написан.

## Follow-up (устаревшие ссылки, намеренно не тронуты)

- `.planning/codebase/**` (`ARCHITECTURE.md`, `CONCERNS.md`, `CONVENTIONS.md`, `INTEGRATIONS.md`, `STACK.md`, `STRUCTURE.md`, `TESTING.md`): снимок кода, исправляется повторным запуском `/gsd-map-codebase`.
- Комментарии в коде `server/Siege.Engine/Hex.cs` (строка 11) и `client/src/game/hex.ts` (строка 1): ограничение «без правок кода»; править отдельной задачей с проверками `dotnet` и `npm`.
- `docs/plan.md`: 7 отмеченных пунктов и проза раздела «## 3. Этапы разработки игры» (замороженный архив).
- История и сгенерированные файлы: `docs/progress.md`, `*-DISCUSSION-LOG.md`, `.planning/INGEST-CONFLICTS.md`, `.planning/intel/**`.
- `.claude/gsd-file-manifest.json`, `.claude/scripts/lib/macos-conformance-tier.generated.cjs` и `.claude/gsd-core/workflows/ingest-docs.md` содержат слово `architecture.md` как часть установки GSD: не трогать.
- `docs/spec.md` по OD-2 будет архивирован отдельной задачей после переписывания `REQUIREMENTS.md`.
- Записи о быстрой задаче в `STATE.md` вносит оркестратор; `ROADMAP.md` (статус фаз), `docs/progress.md` и чекбоксы `docs/plan.md` не менялись по правилам проекта.

## Для владельца

Перенос сделан обычным `mv`. В git он виден как неиндексированное удаление `docs/architecture.md` и новый неотслеживаемый файл в `.planning/` (`.planning/` сейчас целиком неотслеживаемый; решение, коммитить ли его, за владельцем). Ничего не закоммичено и не проиндексировано.

## Предложенное сообщение коммита

```text
docs(docs): перенести целевую архитектуру в .planning/ARCHITECTURE-TARGET.md

- docs/architecture.md перенесён в .planning/ARCHITECTURE-TARGET.md, дерево §3 и журнал §11 обновлены
- ссылки перенаправлены в CLAUDE.md, README.md, правилах engine.md и markdown.md, skill engine-test
- в spec.md изменена только ссылка на архитектуру, в plan.md только открытые пункты
- обновлены PROJECT, ROADMAP, REQUIREMENTS, STATE и CONTEXT фаз 2, 3, 5
```

## Known Stubs

Нет.

## Self-Check: PASSED

- `.planning/ARCHITECTURE-TARGET.md` существует, `docs/architecture.md` отсутствует.
- Коммитов нет по правилам проекта (`commits: 0`), индекс пуст.
