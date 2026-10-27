# Сводка синтеза документов (SYNTHESIS)

Дата: 2026-10-27. Режим: new. Это единая точка входа для `gsd-roadmapper`.

## Документы

Обработано 4 документа (все классификации средней уверенности, UNKNOWN нет):

- SPEC: 1 (`docs/architecture.md`)
- PRD: 1 (`docs/spec.md`, прочитан целиком по решению владельца; источник истины для правил игры)
- DOC: 2 (`docs/plan.md`, `docs/progress.md`, наименьший приоритет, только контекст)
- ADR: 0

Приоритет по типам (ADR > SPEC > PRD > DOC) для пары `spec.md` и `architecture.md` не применялся: по решению владельца все расхождения между ними вынесены владельцу, молчаливых выборов нет.

## Решения

Всего 21 запись в [decisions.md](decisions.md): 9 из журнала `spec.md` §12, 1 из стека `spec.md` §8, 11 из журнала `architecture.md` §11. Locked-решений: 0 (ADR в наборе нет, все записи имеют статус `proposed`).

## Требования

35 требований в [requirements.md](requirements.md), ID вида `REQ-{slug}`:

- Фишки и поле: `REQ-pieces-set`, `REQ-engine-extensibility`, `REQ-board-geometry`, `REQ-board-edge`
- Общие правила: `REQ-turn-order`, `REQ-turn-action`, `REQ-hive-unity`, `REQ-slide-rule`, `REQ-crawl-touch-group`, `REQ-covered-piece-immobile`
- Выставление: `REQ-placement`, `REQ-sovereign-deadline`
- Движение: `REQ-move-sovereign`, `REQ-move-climber`, `REQ-move-jumper`, `REQ-move-runner`, а также конкурирующие варианты `REQ-move-scout-v1` (spec) и `REQ-move-scout-v2` (architecture)
- Конец игры: `REQ-win-condition`, `REQ-draw-condition`, `REQ-pass-turn`
- Режимы, сеть, хранение: `REQ-server-authority`, `REQ-hotseat-mode`, `REQ-online-mode`, `REQ-anonymous-login`, `REQ-reconnect`, `REQ-game-storage`
- Интерфейс: `REQ-highlight`, `REQ-ui-widescreen`, `REQ-ui-panels`, `REQ-theme-separation`, `REQ-graphics-assets`, `REQ-original-assets-only`
- Качество: `REQ-engine-tests`, `REQ-mvp-done`

Критерии приёмки явно заданы только в `spec.md` §13 (отражены в `REQ-mvp-done` и ссылках из отдельных требований).

## Ограничения

19 записей в [constraints.md](constraints.md) из `architecture.md`: api-contract 2, schema 5, nfr 5, protocol 7. Это контракты движка, REST, SignalR, схема GameState и БД, алгоритмы движка, клиентская архитектура, тестирование, запуск и риски.

## Контекст

9 тем в [context.md](context.md): этапы M0-M7, точки сверки с официальными правилами, что не входит в MVP, сценарии, организация работы, текущее состояние проекта, решения владельца по GSD, порядок Discovery, ловушки.

## Конфликты

0 блокеров, 7 предупреждений (competing variants / вопросы владельцу), 9 информационных, авторазрешённых по приоритету 0. Подробности: [INGEST-CONFLICTS.md](../INGEST-CONFLICTS.md).

Предупреждения, которые нужно разрешить владельцу до маршрутизации:

- Поведение `Scout` (возврат на клетки): `docs/spec.md` §5.3 против `docs/architecture.md` §4.4 и §12 п.4
- Формула скольжения `Climber` по высотам и отсутствие первоисточника правил
- Заявление `spec.md` «открытых вопросов нет» против четырёх точек в `architecture.md` §12
- Учёт пропусков в правиле 4-го хода и выставление у высокой стопки
- Идентификаторы статуса партии: `InProgress`/`Won`/`Draw` против `"active"`
- Формулировка условия выставления («хотя бы одной своей»)
- Циклы перекрёстных ссылок между документами (синтезатор понизил с BLOCKER до WARNING, нужно подтверждение)

## Указатели

- [decisions.md](decisions.md)
- [requirements.md](requirements.md)
- [constraints.md](constraints.md)
- [context.md](context.md)
- Классификации: `classifications/`
- Отчёт о конфликтах: `/home/alexander/Projects/alliance/siege/.planning/INGEST-CONFLICTS.md`
