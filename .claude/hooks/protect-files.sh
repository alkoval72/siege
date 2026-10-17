#!/usr/bin/env bash
# PreToolUse: блокирует доступ агента к защищённым файлам.
# Защищены: .env и .env.* (секреты), содержимое .git/, lock-файлы пакетных менеджеров.
# Код выхода 2 отменяет вызов инструмента, текст из stderr получает агент.

set -euo pipefail

input="$(cat)"
tool="$(jq -r '.tool_name // empty' <<<"$input")"

block() {
  echo "Заблокировано hook'ом protect-files: $1" >&2
  echo "Защищённые файлы (.env*, .git/, lock-файлы) агент не читает и не правит. Если изменение нужно, попроси владельца сделать его вручную." >&2
  exit 2
}

# Проверка пути файла: секреты, служебные файлы git, lock-файлы
check_path() {
  local path="$1"
  local name
  name="$(basename "$path")"

  case "$name" in
    .env | .env.*)
      block "файл секретов $path"
      ;;
    package-lock.json | packages.lock.json | yarn.lock | pnpm-lock.yaml)
      block "lock-файл $path (меняется только через пакетный менеджер)"
      ;;
  esac

  case "$path" in
    .git/* | */.git/* | .git | */.git)
      block "служебный файл git $path"
      ;;
  esac
}

case "$tool" in
  Read | Edit | Write | MultiEdit | NotebookEdit)
    path="$(jq -r '.tool_input.file_path // .tool_input.notebook_path // empty' <<<"$input")"
    [[ -n "$path" ]] && check_path "$path"
    ;;

  Bash)
    cmd="$(jq -r '.tool_input.command // empty' <<<"$input")"

    # Любое упоминание файла .env или .env.* в команде (cat, grep, head, cp, echo > и т. п.)
    if grep -Eq '(^|[^A-Za-z0-9_.-])\.env(\.[A-Za-z0-9_-]+)?([^A-Za-z0-9_-]|$)' <<<"$cmd"; then
      block "команда обращается к файлу .env: $cmd"
    fi

    # Запись в .git/ или в lock-файлы в обход git и npm
    if grep -Eq '(>|>>|\btee\b|\bsed\b[^|;&]*-i|\brm\b|\bmv\b|\bcp\b)[^|;&]*(\.git/|package-lock\.json|packages\.lock\.json)' <<<"$cmd"; then
      block "команда изменяет .git/ или lock-файл: $cmd"
    fi
    ;;
esac

exit 0
