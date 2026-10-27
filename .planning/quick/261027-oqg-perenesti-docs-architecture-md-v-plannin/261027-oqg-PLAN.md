---
phase: quick-261027-oqg
plan: 01
type: execute
wave: 1
depends_on: []
files_modified:
  - docs/architecture.md
  - .planning/ARCHITECTURE-TARGET.md
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
files_deleted:
  - docs/architecture.md
autonomous: true
requirements: []  # быстрая задача владельца, требований из ROADMAP нет; трассировка по решениям OD-1, OD-2 ниже

estimate:
  tokens: 50000
  raw_tokens: 50000
  tasks: 3
  confidence: low

must_haves:
  truths:
    - "Целевая архитектура лежит в .planning/ARCHITECTURE-TARGET.md, файла docs/architecture.md нет; заголовок, текст и нумерация разделов 1-13 сохранены (OD-1)"
    - "Дерево §3 перенесённого файла показывает .planning/ARCHITECTURE-TARGET.md и не показывает файл архитектуры в docs/; журнал §11 содержит запись от 2026-10-27 о переносе с причиной (OD-1)"
    - "Агент, открывший любой живой источник (CLAUDE.md, README.md, правила, skill engine-test, spec.md, PROJECT/ROADMAP/REQUIREMENTS/STATE, CONTEXT фаз, открытые пункты plan.md), получает путь .planning/ARCHITECTURE-TARGET.md, и этот файл существует"
    - "spec.md изменён только в ссылке на архитектуру (OD-2); история и сгенерированные файлы (progress.md, *-DISCUSSION-LOG.md, INGEST-CONFLICTS.md, intel, codebase) и код не тронуты"
    - "Ни коммита, ни индексирования: перенос виден в git status как неиндексированное удаление docs/architecture.md"
  artifacts:
    - path: ".planning/ARCHITECTURE-TARGET.md"
      provides: "Целевая архитектура: компоненты, контракты REST и SignalR, схема БД, алгоритмы движка, журнал решений"
      contains: "ARCHITECTURE-TARGET.md   целевая архитектура"
    - path: "CLAUDE.md"
      provides: "Список документов для чтения перед работой и дерево «Где что лежит» с новым путём"
      contains: ".planning/ARCHITECTURE-TARGET.md"
  key_links:
    - from: "CLAUDE.md"
      to: ".planning/ARCHITECTURE-TARGET.md"
      via: "список «Перед работой с правилами и контрактами читай»"
      pattern: "\\.planning/ARCHITECTURE-TARGET\\.md"
    - from: ".claude/rules/engine.md"
      to: ".planning/ARCHITECTURE-TARGET.md"
      via: "источник алгоритмов движка §4"
      pattern: "\\.planning/ARCHITECTURE-TARGET\\.md` §4"
    - from: ".claude/rules/markdown.md"
      to: ".planning/ARCHITECTURE-TARGET.md"
      via: "раздел «Документы проекта»: журнал решений нумерованного документа"
      pattern: "ARCHITECTURE-TARGET\\.md"
---

<objective>
Перенести целевую архитектуру `docs/architecture.md` в `.planning/ARCHITECTURE-TARGET.md` и перенаправить на новый путь все живые ссылки.

Решения владельца (сессия `/gsd-explore` 2026-10-27, закрыты, не пересматриваются):

- OD-1: источник истины для планирования переезжает в `.planning/`. Целевая архитектура живёт в `.planning/ARCHITECTURE-TARGET.md`, а не в `.planning/codebase/ARCHITECTURE.md`: тот файл это снимок кода, его перезаписывает `/gsd-map-codebase`.
- OD-2: `docs/spec.md` будет архивирован позже отдельной задачей (после переписывания `REQUIREMENTS.md`). В этой задаче в `spec.md` меняется только ссылка на архитектуру.

Purpose: всё планирование в одной папке под GSD; агенты, читающие инструкции и артефакты фаз, находят целевую архитектуру по рабочему пути.

Output: `.planning/ARCHITECTURE-TARGET.md` с исправленным деревом §3 и новой записью журнала §11; обновлённые ссылки в 14 живых файлах; `261027-oqg-SUMMARY.md` со списком оставшихся устаревших ссылок и предложенным сообщением коммита.
</objective>

<project_rules>
Правила проекта (`CLAUDE.md`, `~/.claude/CLAUDE.md`) имеют приоритет над поведением исполнителя GSD по умолчанию:

- Никаких `git commit`, `git add`, `git mv`, `git stash`, веток и worktree. Работа идёт последовательно в основном рабочем дереве на ветке `develop`. Протокол «коммит после каждой задачи» исполнителя GSD в этом плане отключён: коммитит владелец (`commit_docs: false`).
- Файл переносится обычным `mv`, не `git mv`.
- Правки текста делаются инструментом Edit (при одинаковой замене по файлу с `replace_all: true`), не через `sed`: после каждой правки hook `lint.sh` проверяет `.md` на длинное тире, обходить его нельзя.
- Если правка затрагивает текст, где упоминается `.env` (например, строка `.gitignore` в дереве §3), правь только через Edit: hook `protect-files.sh` блокирует Bash-команды с этим словом. Bash-команды плана такого текста не содержат.
- Оформление всех `.md` по `.claude/rules/markdown.md`: без длинного тире (U+2014), русские кавычки «ёлочки» в прозе, один абзац на строку, даты `ГГГГ-ММ-ДД`, без эмодзи.
- Кода задача не меняет, поэтому проверки `dotnet` и `npm` не запускаются. Комментарии в `server/Siege.Engine/Hex.cs` и `client/src/game/hex.ts` со ссылкой на архитектуру не трогать, только перечислить в SUMMARY.
- Правило handoff из `CLAUDE.md` (отметить пункт в `docs/plan.md`, добавить запись в `docs/progress.md`) к этой задаче не применяется: владелец запретил правку `docs/progress.md`, а в `docs/plan.md` меняются только ссылки в открытых пунктах. Состояние фиксирует SUMMARY этой быстрой задачи.
- Не редактировать (история или сгенерированные файлы): `docs/progress.md`, `*-DISCUSSION-LOG.md`, `.planning/INGEST-CONFLICTS.md`, `.planning/intel/**`, `.planning/codebase/**`, всё в `.claude/gsd-core/`, `.claude/agents/`, `.claude/commands/`, `.claude/gsd-*`.
</project_rules>

<execution_context>
@/home/alexander/Projects/alliance/siege/.claude/gsd-core/workflows/execute-plan.md
@/home/alexander/Projects/alliance/siege/.claude/gsd-core/templates/summary.md
</execution_context>

<context>
@.planning/STATE.md
@CLAUDE.md
@.claude/rules/markdown.md
@docs/architecture.md

Состояние на момент планирования (2026-10-27, HEAD `c3a1544`):

- `docs/architecture.md` отслеживается git и уже изменён в рабочем дереве (` M`), в индексе ничего нет. После `mv` статус должен стать ` D docs/architecture.md`.
- В `docs/architecture.md` 25 строк заголовков (`#`, 13 разделов `## N.`, 11 подразделов `### N.N`), контрольная сумма списка заголовков `grep -E '^#{1,3} '` = `47c834db3039c83fed69addd41a4b92e`; единственная ссылка на собственное имя файла: строка дерева §3 `│   ├── architecture.md` в блоке `docs/`. Других самоссылок нет.
- Колонка описаний в дереве §3: описание начинается с 34-го символа строки (например, `├── CLAUDE.md` и 20 пробелов, `│   ├── plan.md` и 18 пробелов).
- В дереве «Где что лежит» в `CLAUDE.md` описание начинается с 29-го символа (`docs/` и 23 пробела).
- В `.planning/REQUIREMENTS.md` источники цитируются сокращённо: `spec §N` и `architecture §N` (7 вхождений `architecture §` с обычным пробелом).
- В `docs/plan.md` ссылки на архитектуру есть в 7 отмеченных пунктах `- [x]` (не трогать; контрольная сумма всех строк `- [x]` = `e4f33a300c88cde886e45a7d2e8dfc5c`), в 3 открытых пунктах `- [ ]` (строки 111, 118, 148: править) и в прозе раздела «## 3. Этапы разработки игры» («Подробности в ... §10», не трогать: раздел заморожен, вне объёма).
- Контрольные суммы защищённых файлов (для проверки, что они не изменились): `docs/progress.md` + `.planning/INGEST-CONFLICTS.md` + `.planning/phases/*/*-DISCUSSION-LOG.md` = `4eefb95fa1b211964803ed63553ebcb5`; все файлы `.planning/intel` и `.planning/codebase` (`LC_ALL=C sort -z`) = `870b1d48102976f3a96b7cbad7ee9ae9`.
</context>

<tasks>

<task type="tracer">
  <name>Задача 1: перенос файла, правка дерева §3 и журнала §11, ссылки в CLAUDE.md и README.md</name>
  <files>docs/architecture.md, .planning/ARCHITECTURE-TARGET.md, CLAUDE.md, README.md</files>
  <action>
Сквозной путь от точки входа агента до документа: после задачи `CLAUDE.md` и `README.md` ведут на существующий `.planning/ARCHITECTURE-TARGET.md` (OD-1).

1. Перенести файл командой `mv docs/architecture.md .planning/ARCHITECTURE-TARGET.md` из корня репозитория. Не `git mv`, не `git add` (решение владельца, см. `<project_rules>`).

2. В `.planning/ARCHITECTURE-TARGET.md` (по OD-1) заголовок `# Архитектура: Siege`, строки статуса и даты, текст разделов и нумерацию 1-13 не менять. Только две правки:
   - §3 «Структура репозитория», блок `docs/`: удалить строку `│   ├── architecture.md`. Сразу после строки `│   └── about.md                 отчёт о настройке среды агента` (перед `├── .claude/`) вставить две строки: первая `├── .planning/` и 19 пробелов, затем `GSD: roadmap, состояние, фазы`; вторая `│   └── ARCHITECTURE-TARGET.md` и 3 пробела, затем `целевая архитектура (этот документ)`. Описания должны начинаться в той же колонке (34-й символ), что у соседних строк дерева.
   - §11 «Журнал решений»: добавить строку в конец таблицы, после строки «Имена в локальном режиме (2026-10-27)». Ячейки: «Решение»: `Расположение целевой архитектуры (2026-10-27)`; «Варианты»: `docs/` / `.planning/codebase/` / `.planning/ARCHITECTURE-TARGET.md` (каждый путь в обратных кавычках, разделитель ` / `); «Выбор»: `.planning/ARCHITECTURE-TARGET.md` в обратных кавычках; «Почему»: «Всё планирование в одной папке под GSD; `.planning/codebase/ARCHITECTURE.md` это снимок кода, его перезаписывает `/gsd-map-codebase`». Старое имя файла в нижнем регистре в этой строке не писать (по нему работает отрицательная проверка), каталог `docs/` указать без имени файла.
   - Ссылки на `spec.md` внутри документа (в обратных кавычках, не ссылки Markdown) оставить как есть.

3. В `CLAUDE.md`:
   - список «Перед работой с правилами и контрактами читай»: пункт про `docs/architecture.md` заменить на пункт с путём `.planning/ARCHITECTURE-TARGET.md` и описанием «целевая архитектура (компоненты, контракты REST и SignalR, схема БД, алгоритмы движка).»;
   - правило «Если код, `spec.md` и ... расходятся, остановись и спроси»: имя архитектурного документа заменить на `ARCHITECTURE-TARGET.md`;
   - строка «**Где что лежит** (целевая структура из ... §3, ...)»: ссылку заменить на `.planning/ARCHITECTURE-TARGET.md` §3;
   - дерево «Где что лежит»: в строке `docs/` убрать из перечня описания слово про архитектуру, чтобы описание стало `spec, plan, progress` (колонку не сдвигать); сразу после неё добавить строку `.planning/` и 18 пробелов, затем `GSD: roadmap, состояние, фазы, ARCHITECTURE-TARGET.md` (описание с 29-го символа, как у остальных строк).
   Остальной текст `CLAUDE.md` не менять; длина файла остаётся меньше 200 строк.

4. В `README.md`:
   - список «Документация»: пункт-ссылку на архитектуру заменить на `[ARCHITECTURE-TARGET.md](.planning/ARCHITECTURE-TARGET.md)` с описанием «целевая архитектура (компоненты, контракты, схема БД, алгоритмы движка);»;
   - таблица «Какие инструкции читает агент»: в ячейке «Файл» второй путь заменить на `.planning/ARCHITECTURE-TARGET.md`;
   - фраза «Если код, `spec.md` и ... расходятся, агент останавливается»: имя заменить на `ARCHITECTURE-TARGET.md`.
  </action>
  <verify>
    <automated>test -f .planning/ARCHITECTURE-TARGET.md && test ! -e docs/architecture.md && [ "$(git status --porcelain -- docs/architecture.md)" = " D docs/architecture.md" ] && [ "$(grep -E '^#{1,3} ' .planning/ARCHITECTURE-TARGET.md | md5sum | cut -d' ' -f1)" = 47c834db3039c83fed69addd41a4b92e ] && grep -q '^├── \.planning/ ' .planning/ARCHITECTURE-TARGET.md && grep -q '^│   └── ARCHITECTURE-TARGET\.md   целевая архитектура' .planning/ARCHITECTURE-TARGET.md && grep -q '^| .*(2026-10-27) .*\.planning/ARCHITECTURE-TARGET\.md' .planning/ARCHITECTURE-TARGET.md && ! grep -n 'architecture\.md' .planning/ARCHITECTURE-TARGET.md CLAUDE.md README.md && ! grep -n 'spec, architecture' CLAUDE.md && grep -q '^\.planning/ .*ARCHITECTURE-TARGET\.md' CLAUDE.md && grep -q '`\.planning/ARCHITECTURE-TARGET\.md`' CLAUDE.md && grep -q '(\.planning/ARCHITECTURE-TARGET\.md)' README.md && [ "$(wc -l < CLAUDE.md)" -lt 200 ]</automated>
  </verify>
  <done>Файл лежит в `.planning/ARCHITECTURE-TARGET.md`, `docs/architecture.md` нет, git показывает неиндексированное удаление; в файле 13 разделов и 11 подразделов, дерево §3 содержит `.planning/` и `ARCHITECTURE-TARGET.md`, журнал §11 содержит запись от 2026-10-27; в `CLAUDE.md` и `README.md` нет старого имени, оба ведут на новый путь.</done>
</task>

<task type="auto">
  <name>Задача 2: ссылки в правилах агента, skill engine-test, spec.md и открытых пунктах plan.md</name>
  <files>.claude/rules/engine.md, .claude/rules/markdown.md, .claude/skills/engine-test/SKILL.md, docs/spec.md, docs/plan.md</files>
  <action>
Перенаправить ссылки, по которым агент ищет алгоритмы и правила оформления (OD-1). Порядок замены в каждом файле: сначала полный путь `docs/architecture.md` на `.planning/ARCHITECTURE-TARGET.md`, затем оставшееся голое имя на `ARCHITECTURE-TARGET.md`. Обратный порядок даёт неверный путь из каталога `docs/` и нового имени.

1. `.claude/rules/engine.md`, пункт «Правила игры берутся из `docs/spec.md` §4-5, алгоритмы из ... §4»: путь заменить на `.planning/ARCHITECTURE-TARGET.md`, ссылку на §4 сохранить.

2. `.claude/rules/markdown.md`:
   - вводная фраза «Правило действует для всех `.md`-файлов проекта: ...»: после `docs/*.md` добавить `.planning/ARCHITECTURE-TARGET.md` (через запятую, в обратных кавычках), чтобы перенесённый документ остался в перечне, как был в составе `docs/*.md`; frontmatter `paths` не менять;
   - раздел «Структура», пункт о нумерованных документах: в скобках `(`spec.md`, ...)` второе имя заменить на `ARCHITECTURE-TARGET.md`;
   - раздел «Код, пути и ссылки», пример относительной ссылки: заменить на `[ARCHITECTURE-TARGET.md](.planning/ARCHITECTURE-TARGET.md)`;
   - раздел «Документы проекта», пункт «Решения, меняющие `spec.md` или ..., записываются в журнал решений этого документа»: имя заменить на `ARCHITECTURE-TARGET.md`. Остальные пункты раздела (про `plan.md` и `progress.md`) не менять: их переделка это отдельный открытый пункт `docs/plan.md`.

3. `.claude/skills/engine-test/SKILL.md`, шаг «Определи, какое правило проверяется, и найди его в `docs/spec.md` (§4-5) и в ... (§4)»: путь заменить на `.planning/ARCHITECTURE-TARGET.md`, `(§4)` сохранить.

4. `docs/spec.md` (по OD-2 только ссылка): в разделе про локальный режим фразу «Способ запуска ... определяется в ...» перенаправить на полный путь `.planning/ARCHITECTURE-TARGET.md` (полный, потому что `spec.md` лежит в `docs/`). Версию, шапку, журнал решений §12 и любой другой текст `spec.md` не менять.

5. `docs/plan.md`: менять только открытые пункты `- [ ]` со ссылкой на архитектуру, их три:
   - пункт «Записать решение «план и состояние ведёт GSD» в журнал ... §11; ... §10 ссылается на `.planning/ROADMAP.md`»: первое вхождение заменить на `.planning/ARCHITECTURE-TARGET.md`, второе на `ARCHITECTURE-TARGET.md`;
   - пункт «Согласовать корневой `CLAUDE.md` с `.claude/CLAUDE.md` от GSD ...; решение записать в журнал ... §11»: заменить на `.planning/ARCHITECTURE-TARGET.md`;
   - пункт «Расхождения с `spec.md` и ... вынесены владельцу»: заменить на `ARCHITECTURE-TARGET.md`.
   Отмеченные пункты `- [x]` (их 7 со ссылкой на архитектуру) и прозу раздела «## 3. Этапы разработки игры» не трогать: это история и замороженный раздел. Чекбоксы не переключать, пункты не добавлять.
  </action>
  <verify>
    <automated>! grep -n 'architecture\.md' .claude/rules/engine.md .claude/rules/markdown.md .claude/skills/engine-test/SKILL.md docs/spec.md && ! grep -n '^- \[ \].*architecture\.md' docs/plan.md && [ "$(grep '^- \[x\]' docs/plan.md | md5sum | cut -d' ' -f1)" = e4f33a300c88cde886e45a7d2e8dfc5c ] && grep -q '^Подробности в `architecture\.md` §10' docs/plan.md && [ -z "$(grep -L 'ARCHITECTURE-TARGET\.md' .claude/rules/engine.md .claude/rules/markdown.md .claude/skills/engine-test/SKILL.md docs/spec.md docs/plan.md)" ] && grep -q '`\.planning/ARCHITECTURE-TARGET\.md` §4' .claude/rules/engine.md && grep -q '`\.planning/ARCHITECTURE-TARGET\.md`' docs/spec.md</automated>
  </verify>
  <done>В правилах, skill и `spec.md` старого имени нет, все пять файлов ссылаются на `ARCHITECTURE-TARGET.md`; в `docs/plan.md` открытые пункты перенаправлены, 7 отмеченных пунктов и проза раздела 3 остались без изменений.</done>
</task>

<task type="auto">
  <name>Задача 3: ссылки в артефактах .planning: PROJECT, ROADMAP, REQUIREMENTS, STATE и CONTEXT фаз</name>
  <files>.planning/PROJECT.md, .planning/ROADMAP.md, .planning/REQUIREMENTS.md, .planning/STATE.md, .planning/phases/02-m1-dvizhok-pole-vystavlenie-pravilo-korolya/02-CONTEXT.md, .planning/phases/03-m2-dvizhenie-fishek-edinstvo-gruppy-skolzhenie/03-CONTEXT.md, .planning/phases/05-m4-server-i-klient-lokalnyy-rezhim/05-CONTEXT.md</files>
  <action>
Перенаправить ссылки, по которым исследователь, планировщик и исполнитель фаз находят целевую архитектуру (OD-1). Замены механические, в каждом файле Edit с `replace_all: true` в строгом порядке:

1. полный путь `docs/architecture.md` заменить на `.planning/ARCHITECTURE-TARGET.md`;
2. затем оставшееся голое имя `architecture.md` (нижний регистр) заменить на `ARCHITECTURE-TARGET.md`.

Обратный порядок даёт неверный путь из каталога `docs/` и нового имени. Заглавное `ARCHITECTURE.md` (снимок `.planning/codebase/`) под замену не попадает, потому что замена чувствительна к регистру.

По файлам (ожидаемое число вхождений полного пути / всех вхождений имени на момент планирования):

- `.planning/PROJECT.md` (2 / 6): в абзаце «Источники истины» после замены путь читается как `.planning/ARCHITECTURE-TARGET.md` (v0.1); прочий текст не менять.
- `.planning/ROADMAP.md` (4 / 5): обзор этапов M0-M7, «Open questions» фаз 2, 3, 4, 5. Структуру фаз, списки планов и чекбоксы не менять.
- `.planning/REQUIREMENTS.md` (1 / 4): дополнительно третьим шагом заменить сокращённые цитаты источника (слово `architecture`, за ним обычный пробел и знак параграфа перед номером раздела) на `ARCHITECTURE-TARGET §` (7 вхождений, в том числе в таблице трассировки OQ-3), чтобы цитата совпадала с именем документа в строке «Источник:». Цитаты `spec §N` не трогать.
- `.planning/STATE.md` (0 / 4): только голые имена. Frontmatter и прочие разделы не менять; записи о быстрой задаче в `STATE.md` вносит оркестратор, не исполнитель.
- `02-CONTEXT.md` (9 / 10), `03-CONTEXT.md` (5 / 10), `05-CONTEXT.md` (9 / 19): заменить все вхождения, в том числе в формулировках решений D-NN и в списках канонических ссылок. Содержимое и нумерация разделов документа не изменились, поэтому ссылки вида «§4.4» остаются верными, а прошлые решения сохраняют смысл, меняется только путь. Номера решений, формулировки и прочий текст не менять. Файлы `*-DISCUSSION-LOG.md` в этих папках не открывать на запись.
  </action>
  <verify>
    <automated>! grep -n 'architecture\.md' .planning/PROJECT.md .planning/ROADMAP.md .planning/REQUIREMENTS.md .planning/STATE.md .planning/phases/*/0?-CONTEXT.md && ! grep -n 'architecture §' .planning/REQUIREMENTS.md && [ "$(grep -o 'ARCHITECTURE-TARGET §' .planning/REQUIREMENTS.md | wc -l)" = 7 ] && [ -z "$(grep -L 'ARCHITECTURE-TARGET\.md' .planning/PROJECT.md .planning/ROADMAP.md .planning/REQUIREMENTS.md .planning/STATE.md .planning/phases/*/0?-CONTEXT.md)" ] && ! grep -rn 'docs/ARCHITECTURE-TARGET' .planning/*.md .planning/phases</automated>
  </verify>
  <done>В четырёх артефактах `.planning/` и трёх CONTEXT фаз нет старого имени, все семь файлов ссылаются на `ARCHITECTURE-TARGET.md`; цитаты источника в `REQUIREMENTS.md` согласованы; неверного пути в каталоге `docs/` нигде нет.</done>
</task>

</tasks>

<threat_model>
## Trust Boundaries

| Boundary | Description |
|----------|-------------|
| исполнитель → рабочее дерево | агент массово правит инструкции и артефакты, которые читают все следующие агенты |
| исполнитель → git | правило проекта: коммитит и индексирует только владелец |

## STRIDE Threat Register

| Threat ID | Category | Component | Severity | Disposition | Mitigation Plan |
|-----------|----------|-----------|----------|-------------|-----------------|
| T-oqg-01 | Tampering | история и сгенерированные файлы (`docs/progress.md`, `*-DISCUSSION-LOG.md`, `INGEST-CONFLICTS.md`, `intel/`, `codebase/`) | medium | mitigate | перечень запретов в `<project_rules>`; итоговая проверка сверяет контрольные суммы `4eefb95fa1b211964803ed63553ebcb5` и `870b1d48102976f3a96b7cbad7ee9ae9` |
| T-oqg-02 | Repudiation | git: коммит, индексирование, `git mv` вместо решения владельца | medium | mitigate | запрет в `<project_rules>`; проверки `git diff --cached --quiet` и статус ` D docs/architecture.md` |
| T-oqg-03 | Denial of Service | ссылки, ведущие агентов на отсутствующий файл или неверный путь в `docs/` | medium | mitigate | строгий порядок замен; отрицательные grep по старому имени и по неверному пути, положительные `grep -L` по новому имени, `test -f` нового файла |
| T-oqg-04 | Tampering | код (`server/Siege.Engine/Hex.cs`, `client/src/game/hex.ts`) | low | mitigate | код не правится; проверка `git diff --quiet` для этих файлов; ссылки в комментариях уходят в SUMMARY как follow-up |
| T-oqg-SC | Tampering | npm/pip/cargo installs | high | accept | установок пакетов в задаче нет |
</threat_model>

<verification>
Итоговая проверка из корня репозитория (все команды должны завершиться успешно):

1. Перенос и живые ссылки: `test -f .planning/ARCHITECTURE-TARGET.md && test ! -e docs/architecture.md && ! grep -n 'architecture\.md' CLAUDE.md README.md .claude/rules/engine.md .claude/rules/markdown.md .claude/skills/engine-test/SKILL.md docs/spec.md .planning/PROJECT.md .planning/ROADMAP.md .planning/REQUIREMENTS.md .planning/STATE.md .planning/phases/*/0?-CONTEXT.md .planning/ARCHITECTURE-TARGET.md && ! grep -n '^- \[ \].*architecture\.md' docs/plan.md && ! grep -rn 'docs/ARCHITECTURE-TARGET' CLAUDE.md README.md .claude/rules .claude/skills docs .planning/*.md .planning/phases`
2. Длинное тире в изменённых файлах отсутствует: `! grep -n "$(printf '\xe2\x80\x94')" CLAUDE.md README.md .claude/rules/engine.md .claude/rules/markdown.md .claude/skills/engine-test/SKILL.md docs/spec.md docs/plan.md .planning/ARCHITECTURE-TARGET.md .planning/PROJECT.md .planning/ROADMAP.md .planning/REQUIREMENTS.md .planning/STATE.md .planning/phases/*/0?-CONTEXT.md`
3. Защищённые файлы, код и git не тронуты: `[ "$(cat docs/progress.md .planning/INGEST-CONFLICTS.md .planning/phases/*/*-DISCUSSION-LOG.md | md5sum | cut -d' ' -f1)" = 4eefb95fa1b211964803ed63553ebcb5 ] && [ "$(find .planning/intel .planning/codebase -type f -print0 | LC_ALL=C sort -z | xargs -0 cat | md5sum | cut -d' ' -f1)" = 870b1d48102976f3a96b7cbad7ee9ae9 ] && git diff --quiet -- server/Siege.Engine/Hex.cs client/src/game/hex.ts && git diff --cached --quiet && [ "$(git status --porcelain -- docs/architecture.md)" = " D docs/architecture.md" ]`

Если проверка 3 падает на контрольной сумме, а исполнитель эти файлы не открывал на запись, остановиться и сообщить владельцу (файлы могли измениться вне задачи), а не подгонять сумму.
</verification>

<success_criteria>
- `.planning/ARCHITECTURE-TARGET.md` существует, `docs/architecture.md` нет; содержимое и нумерация сохранены, дерево §3 и журнал §11 обновлены (OD-1).
- Ни в одном живом файле из объёма нет старого имени (в `docs/plan.md` проверяются открытые пункты); нигде нет неверного пути в `docs/` с новым именем.
- `spec.md` изменён только в одной ссылке (OD-2).
- Длинного тире в изменённых файлах нет.
- История, сгенерированные файлы и код не изменены; ничего не закоммичено и не проиндексировано.
- SUMMARY создан и перечисляет оставшиеся устаревшие ссылки и предложенное сообщение коммита.
</success_criteria>

<output>
Создать `.planning/quick/261027-oqg-perenesti-docs-architecture-md-v-plannin/261027-oqg-SUMMARY.md` по шаблону summary, во frontmatter `status: complete`. Не коммитить (ни SUMMARY, ни остальное).

Обязательные разделы SUMMARY, на русском, по правилам `.claude/rules/markdown.md`:

- Что сделано: перенос, правки §3 и §11, список изменённых файлов с числом замен; результат итоговой проверки.
- Follow-up (оставшиеся устаревшие ссылки, намеренно не тронуты):
  - `.planning/codebase/**` (`ARCHITECTURE.md`, `CONCERNS.md`, `CONVENTIONS.md`, `INTEGRATIONS.md`, `STACK.md`, `STRUCTURE.md`, `TESTING.md`): снимок кода, исправляется повторным запуском `/gsd-map-codebase`;
  - комментарии в коде `server/Siege.Engine/Hex.cs` (строка 11) и `client/src/game/hex.ts` (строка 1): ограничение «без правок кода»; править отдельной задачей с проверками `dotnet` и `npm`;
  - `docs/plan.md`: 7 отмеченных пунктов и проза раздела «## 3. Этапы разработки игры» (замороженный архив);
  - история без изменений: `docs/progress.md`, `*-DISCUSSION-LOG.md`, `.planning/INGEST-CONFLICTS.md`, `.planning/intel/**`.
- Для владельца: перенос сделан обычным `mv`, в git он виден как удаление `docs/architecture.md` и новый неотслеживаемый файл в `.planning/` (решение, коммитить ли `.planning/`, за владельцем).
- Предложенное сообщение коммита (Conventional Commits, на русском, без длинного тире), например: `docs(docs): перенести целевую архитектуру в .planning/ARCHITECTURE-TARGET.md` и тело со списком перенаправленных ссылок.
</output>
