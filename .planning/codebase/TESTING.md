---
last_mapped_commit: c3a1544532204b0d2dd82c3ccb7fae639bc11cc2
last_mapped_at: 2026-10-27
---
# Паттерны тестирования

**Дата анализа:** 2026-10-27

## Тестовый фреймворк

**Сервер (движок), C#:**
- xUnit 2.4.2 через `Microsoft.NET.Test.Sdk` 17.6.0 и `xunit.runner.visualstudio` 2.4.5 (`server/Siege.Engine.Tests/Siege.Engine.Tests.csproj`).
- Покрытие: `coverlet.collector` 6.0.0 в том же csproj.
- Утверждения: встроенный `Assert` из xUnit (`Assert.Equal`, `Assert.All`, `Assert.Contains`).
- Глобальный using: `server/Siege.Engine.Tests/GlobalUsings.cs` содержит `global using Xunit;`.
- Проект тестов ссылается на `server/Siege.Engine/Siege.Engine.csproj`.

**Клиент, TypeScript и Vue:**
- Vitest ^4.1.11, конфиг `client/vitest.config.ts`. Окружение `jsdom`, `e2e/**` исключён из unit-прогона. Конфиг наследуется от `client/vite.config.ts` через `mergeConfig`.
- Утверждения: `expect` из Vitest.
- Монтирование компонентов: `@vue/test-utils` ^2.5.0 (`mount`).
- Типы тестов: `@types/jsdom`, `jsdom` ^30.0.1, тип `node` в `client/tsconfig.vitest.json`.

**E2E:**
- Playwright ^1.63.0, конфиг `client/playwright.config.ts`.
- Каталог тестов `client/e2e/`, имя файла `*.spec.ts`.
- Браузер: только Chromium с профилем `Desktop Chrome` и viewport 1920x1080.
- В CI (`process.env.CI`): `npm run preview` на порту 4173, retries 2. Локально: `npm run dev` на порту 5173, существующий сервер переиспользуется.

**Команды запуска:**

```bash
dotnet test server/Siege.sln                                              # все тесты сервера
dotnet test server/Siege.sln --filter "FullyQualifiedName~Siege.Engine.Tests.HexTests"   # один класс
dotnet test server/Siege.sln --filter "FullyQualifiedName=Siege.Engine.Tests.HexTests.DistanceFromCenter_Center_IsZero"  # один тест

npm --prefix client run test:unit                                         # Vitest, все unit-тесты
npm --prefix client run test:unit:watch                                   # Vitest в режиме наблюдения (только в фоне)
cd client && npx vitest run src/__tests__/App.spec.ts -t "название"       # один файл или тест
npm --prefix client run test:e2e                                          # Playwright, сам поднимает dev-сервер
cd client && npx playwright test e2e/vue.spec.ts                          # один e2e-файл
cd client && npx playwright install chromium                              # браузер для e2e (один раз)
```

Покрытие:
- Отчёт xUnit с coverlet вызывается стандартным `dotnet test` с коллектором (`--collect:"XPlat Code Coverage"`). В проекте настроек порога нет.
- Для клиента команда покрытия не описана в `client/package.json`.

## Расположение тестов

**Сервер:**
- Тесты лежат в отдельном проекте `server/Siege.Engine.Tests/`, а не рядом с кодом.
- Один файл на тип: `server/Siege.Engine.Tests/HexTests.cs` тестирует `server/Siege.Engine/Hex.cs`.
- Пространство имён тестов: `Siege.Engine.Tests`.

**Клиент:**
- Unit-тесты лежат в `__tests__/` рядом с модулем. Пример: `client/src/__tests__/App.spec.ts`. Для модуля `client/src/game/hex.ts` правило из `CLAUDE.md` указывает путь `client/src/game/__tests__/hex.spec.ts`.
- E2E-тесты лежат в `client/e2e/`, пример: `client/e2e/vue.spec.ts`.
- Исключения из сборки: `client/tsconfig.app.json` исключает `src/**/__tests__/*`, типы тестов проверяются `client/tsconfig.vitest.json`.

**Именование:**
- C#: `<Тип>Tests.cs`, класс `<Тип>Tests`, `public class HexTests`.
- TypeScript: `<модуль>.spec.ts`.
- E2E: `<сценарий>.spec.ts`.

**Структура каталогов клиента:**

```text
client/
├── e2e/
│   └── vue.spec.ts
└── src/
    ├── __tests__/
    │   └── App.spec.ts
    └── game/
        └── hex.ts           (тесты пока отсутствуют, по правилу: src/game/__tests__/hex.spec.ts)
```

## Структура тестов

**Сервер (xUnit):**
- Метод теста именуется `Метод_Условие_Результат` (`.claude/rules/engine.md`, `.editorconfig`). Примеры из `server/Siege.Engine.Tests/HexTests.cs`: `Center_Always_IsOrigin`, `DistanceFromCenter_AnyHex_IsMaxOfAxes`, `Neighbors_AnyHex_ReturnsHexesInDirectionsOrder`.
- Один тест проверяет одно поведение. Структура: подготовка, действие, проверка, разделённые пустой строкой (`.claude/skills/engine-test/SKILL.md`).
- Однотипные случаи объединяются в `[Theory]` с `[InlineData]`.
- Тесты не используют тематические названия фишек.

Пример из `server/Siege.Engine.Tests/HexTests.cs`:

```csharp
[Theory]
[InlineData(1, 0, 1)]
[InlineData(-3, 1, 3)]
[InlineData(5, 5, 10)]
public void DistanceFromCenter_AnyHex_IsMaxOfAxes(int q, int r, int expected)
{
    Assert.Equal(expected, new Hex(q, r).DistanceFromCenter);
}
```

Тест на перебор клеток в круге радиуса 10 (`DistanceFromCenter_HexesWithinRadius_CountMatchesBoardSize`) подсчитывает клетки по `Enumerable.Range` и сверяет с ожидаемым числом (1, 7, 331 для радиусов 0, 1, 10). Это пример проверки размера поля через шестиугольные числа.

**Клиент (Vitest):**
- Блок `describe` с именем компонента или модуля, внутри `it` с описанием на русском.
- Пример из `client/src/__tests__/App.spec.ts`:

```typescript
import { describe, it, expect } from 'vitest'

import { mount } from '@vue/test-utils'
import App from '../App.vue'

describe('App', () => {
  it('показывает название игры', () => {
    const wrapper = mount(App)
    expect(wrapper.text()).toContain('Siege')
  })
})
```

**E2E (Playwright):**
- Тест на уровне файла через `test(...)`, селекторы по роли или тегу (`page.locator('h1')`).
- Пример из `client/e2e/vue.spec.ts`: открывает `/` и проверяет текст заголовка `Siege`.

**Patterns:**
- Setup: создание значений прямо в тесте. Общих `beforeEach` в проекте пока нет.
- Teardown: не требуется, движок чистый и без состояния (`.claude/rules/engine.md`).
- Assertion: `Assert.Equal` для значений, `Assert.All` для коллекций, `Assert.Contains` для вхождений (C#). `expect(...).toContain` (TS).

## Моки

**Фреймворк:** на сервере моков нет. Движок чистый: без зависимостей, сети, БД, `DateTime.Now` и `Random` (`.claude/rules/engine.md`). Жребий первого хода приходит снаружи параметром, поэтому его можно передать в тест как значение.

**Клиент:** моки в существующих тестах не используются. Vitest поддерживает `vi.mock`, `vi.fn`, но в проекте пока нет примеров.

**Что мокать:**
- Сетевой слой клиента (`client/src/api/http.ts`, `client/src/api/hub.ts`) при unit-тестах компонентов и Pinia-стора: подменяются ответы сервера, включая `legalMoves`.
- Внешние зависимости сервера (EF Core, SignalR) при тестах сервера: при появлении серверных тестов вне движка подменяются через интерфейсы.

**Что не мокать:**
- Правила движка: тесты обращаются к реальному `Hex`, `Apply` и `GetLegalMoves`. Второй набор правил в тестах не пишется (`CLAUDE.md`).
- Геометрию клиента `client/src/game/*.ts`: это чистые функции, их тестируют напрямую.

## Фикстуры и фабрики

**Тестовые данные:**
- Сейчас значения создаются литералами: `new Hex(2, -1)`, `[new(1, 0), new(1, -1), ...]`.
- Для позиций партии по `docs/architecture.md` (§9) предусмотрен конструктор позиций: список «клетка, команда, тип» или компактная текстовая запись. Такого конструктора в `server/Siege.Engine.Tests/` пока нет, его стоит добавить вместе с первым тестом правила фишки.

**Расположение:**
- C#: вспомогательные типы и методы держать в `server/Siege.Engine.Tests/`, рядом с тестами. Общий файл фикстур пока не создан.
- TypeScript: фикстуры рядом с `__tests__/`, при необходимости в `client/src/__tests__/fixtures/`.

## Покрытие

**Требования:** порог покрытия не задан ни в `server/Siege.Engine.Tests/Siege.Engine.Tests.csproj`, ни в `client/vitest.config.ts`.

**Просмотр покрытия:**

```bash
dotnet test server/Siege.sln --collect:"XPlat Code Coverage"
```

Для клиента команда покрытия не настроена. Vitest принимает флаг `--coverage` только после установки провайдера, устанавливать пакеты без согласования владельца нельзя (`CLAUDE.md`).

## Типы тестов

**Unit-тесты сервера (движок):**
- Область: `Hex` и далее правила фишек, `GetLegalMoves`, `Apply`, единство группы, скольжение, край поля.
- Обязательны по правилу: каждое новое или изменённое правило движка сопровождается тестами через skill `engine-test`, с допустимыми и недопустимыми случаями и краем поля.
- Планируемые тесты из `docs/architecture.md` (§9): случайные партии с проверками инвариантов, `Replay` даёт то же состояние, что и последовательность `Apply`.

**Unit-тесты клиента:**
- Область: чистые функции `client/src/game/*.ts` и компоненты через `@vue/test-utils`.
- Пишутся только по запросу владельца (`.claude/rules/client.md`). Текущий тест `App.spec.ts` существует.

**Интеграционные тесты сервера:**
- Не написаны. Планируются на этапе M5 (`docs/architecture.md`, онлайн-режим, PostgreSQL). Тесты сервера с БД будут опираться на реальную СУБД, решение по выбору подхода за владельцем.

**E2E-тесты:**
- Фреймворк: Playwright, один смоук-тест `client/e2e/vue.spec.ts`.
- План (`docs/architecture.md`, §9): смоук-тест с двумя контекстами браузера, несколько ходов партии.
- Для видимых изменений интерфейса после правки запускается `test:e2e` или проверка через Playwright MCP с сценарием, скриншотом и чистой консолью (`CLAUDE.md`).

## Проверка перед коммитом

- Skill `verify` (`.claude/skills/verify/SKILL.md`) выполняет `dotnet build`, `dotnet format --verify-no-changes`, `dotnet test`, `check:lint`, `format:check`, `type-check`, `test:unit`, а `test:e2e` только при аргументе `e2e`.
- Задача не выполнена, пока проверки не зелёные (`CLAUDE.md`).
- Новые тесты для клиента и прочих частей пишутся только по явному запросу. Для движка тесты обязательны.

## Типичные паттерны

**Асинхронные тесты (Playwright):**

```typescript
test('главная страница открывается', async ({ page }) => {
  await page.goto('/')
  await expect(page.locator('h1')).toHaveText('Siege')
})
```

**Тесты ошибок (xUnit):**
- Пока не встречаются. Нарушение правила в движке оформляется как результат, а не исключение (см. `CONVENTIONS.md`). Тесты для него проверяют значение результата, а не `Assert.Throws`.

**Тесты ошибок (Vitest):**
- Пока не встречаются. Ожидаемый шаблон: `expect(() => fn()).toThrow(...)` для исключений и проверка состояния после `MoveRejected` для клиента (`docs/architecture.md`, раздел 6).

---

*Анализ тестирования: 2026-10-27*
