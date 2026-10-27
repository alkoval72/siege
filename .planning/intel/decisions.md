# Решения (decisions)

Дата синтеза: 2026-10-27. Режим: new.

В наборе документов нет ADR. Записи ниже взяты из журналов решений `docs/spec.md` §12 и `docs/architecture.md` §11 и из технологической таблицы `docs/spec.md` §8. Документы не помечены как Accepted или locked, поэтому все записи имеют статус `proposed` (не locked), блокирующих LOCKED-решений нет. Перед использованием роадмаппером противоречия между документами смотри в `/home/alexander/Projects/alliance/siege/.planning/INGEST-CONFLICTS.md`.

Решения владельца по GSD из `docs/plan.md` §2 (тип DOC) вынесены в `context.md`, тема «Решения владельца по GSD».

## Клиентский фреймворк: Vue 3 + TypeScript
- source: docs/spec.md §12
- status: proposed
- decision: Клиент на Vue 3 + TypeScript.
- scope: клиент

## Размер поля: радиус 10, 331 клетка
- source: docs/spec.md §12
- status: proposed
- decision: Радиус поля 10, всего 331 клетка.
- scope: поле, правила края поля

## Подсветка недопустимых клеток
- source: docs/spec.md §12
- status: proposed
- decision: Красным подсвечиваются только клетки у группы, к цвету добавляются значки.
- scope: интерфейс, подсветка

## Вход в игру
- source: docs/spec.md §12
- status: proposed
- decision: Анонимный вход по имени.
- scope: идентичность игрока

## Первый ход
- source: docs/spec.md §12
- status: proposed
- decision: Первый ход определяется случайно.
- scope: правила, старт партии

## Ничья по повторению
- source: docs/spec.md §12
- status: proposed
- decision: Откладывается на период после MVP.
- scope: правила, конец игры

## Hot-seat в MVP
- source: docs/spec.md §12
- status: proposed
- decision: Hot-seat входит в MVP и реализуется первым.
- scope: режимы игры, порядок реализации

## Название
- source: docs/spec.md §12
- status: proposed
- decision: Кодовое имя Siege, игровое название выбирается позже.
- scope: именование

## Ветки
- source: docs/spec.md §12
- status: proposed
- decision: Разработка в `develop`, `main` для стабильных состояний.
- scope: процесс разработки

## Технологический стек MVP
- source: docs/spec.md §8
- status: proposed
- decision: Движок правил, библиотека на C# без зависимостей от сети, интерфейса и БД; сервер ASP.NET Core и SignalR; БД PostgreSQL (партии, ходы, игроки); клиент Vue 3 + TypeScript, SVG; запуск локально через Docker Compose; тема оформления в отдельном файле, движок о ней не знает.
- scope: стек, запуск

## Локальный режим: через сервер
- source: docs/architecture.md §11
- status: proposed
- decision: Локальный режим идёт через сервер (альтернатива: движок в браузере через WASM). Причина: один движок и один клиентский путь.
- scope: hot-seat, архитектура режимов

## Хранение партии: только ходы
- source: docs/architecture.md §11
- status: proposed
- decision: Хранятся только ходы, без снимков состояния. Причина: одна истина, простые повторы и отладка.
- scope: БД, история партии

## Допустимые ходы для клиента: все сразу
- source: docs/architecture.md §11
- status: proposed
- decision: Сервер отправляет клиенту все допустимые ходы сразу, а не по запросу. Причина: мгновенная подсветка без лишних запросов.
- scope: контракт GameState, подсветка

## Активные партии: в памяти с записью в БД
- source: docs/architecture.md §11
- status: proposed
- decision: Активные партии живут в памяти сервера, ходы записываются в БД. Причина: просто для одного сервера, восстановление из БД.
- scope: сервер, GameRegistry

## Координаты: осевые
- source: docs/architecture.md §11
- status: proposed
- decision: Осевые координаты (q, r) (варианты: кубические, смещённые). Причина: компактно, легко считать соседей и расстояния.
- scope: движок, поле

## Ходы: SignalR, REST только для создания, входа, чтения
- source: docs/architecture.md §11
- status: proposed
- decision: Ходы идут только через SignalR; REST используется для создания партии, входа и чтения состояния. Причина: единый путь для обоих режимов.
- scope: контракты сервера

## Проверка хода: членство в GetLegalMoves
- source: docs/architecture.md §11
- status: proposed
- decision: Ход проверяется как членство в `GetLegalMoves`, второго набора правил нет.
- scope: сервер, движок

## Доступ к БД: EF Core + Npgsql без слоя Repository
- source: docs/architecture.md §11
- status: proposed
- decision: EF Core + Npgsql, `DbContext` напрямую без слоя Repository (вариант: Dapper). Причина: миграции и привычный стек; Repository поверх EF Core избыточен (стандарт компании).
- scope: БД, сервер

## Типы клиента: вручную
- source: docs/architecture.md §11
- status: proposed
- decision: DTO-типы клиента пишутся вручную, генерация добавляется при необходимости.
- scope: клиент, `api/types.ts`

## Тема: отдельный файл на клиенте
- source: docs/architecture.md §11
- status: proposed
- decision: Тема оформления хранится в отдельном файле на клиенте (вариант: в движке). Причина: смена оформления не трогает правила.
- scope: тема, клиент

## Идентичность: токен в localStorage, хеш в БД
- source: docs/architecture.md §11
- status: proposed
- decision: Токен хранится в `localStorage` клиента, в БД лежит его хеш (варианты: сессия, cookie). Причина: анонимный вход без регистрации.
- scope: идентичность игрока, безопасность
