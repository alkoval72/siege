---
name: verify
description: Полная проверка проекта перед коммитом. Сборка, форматирование, линтеры и все тесты сервера и клиента, итоговый отчёт и предложение сообщения коммита.
disable-model-invocation: true
argument-hint: "[e2e]"
allowed-tools: Read, Grep, Bash(dotnet build *), Bash(dotnet test *), Bash(dotnet format *), Bash(npm --prefix client run *), Bash(git status *), Bash(git diff *), Bash(git log *)
---

# Проверка перед коммитом

Только проверяй и отчитывайся. Файлы не исправляй, если владелец об этом не попросил.

## Шаги

Выполни по порядку и для каждого шага запомни результат: зелёный, красный или пропущен.

**Сервер:**
1. `dotnet build server/Siege.sln`
2. `dotnet format server/Siege.sln --verify-no-changes`
3. `dotnet test server/Siege.sln`

**Клиент** (вывод npm всегда перенаправляй, например `2>&1 | tail -20`: без перенаправления npm в этом окружении может завершиться с кодом 1 без вывода):
4. `npm --prefix client run check:lint` (линтеры без автоисправления)
5. `npm --prefix client run format:check`
6. `npm --prefix client run type-check`
7. `npm --prefix client run test:unit`
8. Только если в аргументах есть `e2e`: `npm --prefix client run test:e2e`. Иначе отметь шаг как пропущенный.

**Репозиторий:**
9. `git status --short` и `git diff --stat`: что изменено и что не отслеживается.
10. Проверь изменённые `.md`, `.cs`, `.ts`, `.vue` на длинное тире (U+2014) через Grep. Найденные места перечисли с номером строки.

## Отчёт

1. Таблица: шаг, команда, результат. Для красного шага приведи первые строки ошибки.
2. Итог одной строкой: «можно коммитить» или «коммитить рано» и почему.
3. Если всё зелёное, предложи сообщение коммита на русском в формате Conventional Commits (`feat|fix|refactor|test|docs|chore(<scope>): …`, scope: `engine`, `server`, `client`, `docs`, `agent`), без длинного тире.

Сам `git commit` не выполняй: коммитит владелец.
