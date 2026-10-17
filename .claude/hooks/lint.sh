#!/usr/bin/env bash
# PostToolUse: форматирует и проверяет файл сразу после правки агентом.
#   server/**/*.cs              dotnet format (пробелы, стиль, анализаторы)
#   client/**/*.{ts,vue,js,mts}  prettier --write и eslint --fix
#   *.md, *.cs, *.ts, *.vue      поиск длинного тире (U+2014), запрещённого правилами проекта
# Если файл переформатирован, агент получает уведомление. Если остались ошибки,
# код выхода 2 возвращает их агенту, чтобы он исправил файл.

set -uo pipefail

input="$(cat)"
path="$(jq -r '.tool_input.file_path // .tool_response.filePath // empty' <<<"$input")"
[[ -z "$path" || ! -f "$path" ]] && exit 0

root="${CLAUDE_PROJECT_DIR:-$(git -C "$(dirname "$path")" rev-parse --show-toplevel)}"
rel="${path#"$root"/}"
problems=""

before="$(sha1sum "$path")"

case "$rel" in
  server/*.cs)
    if ! out="$(cd "$root" && dotnet format server/Siege.sln --include "$rel" 2>&1)"; then
      problems+="dotnet format:"$'\n'"$out"$'\n'
    fi
    ;;
  client/*.ts | client/*.vue | client/*.js | client/*.mts)
    crel="${rel#client/}"
    if ! out="$(cd "$root/client" && npx prettier --write "$crel" 2>&1)"; then
      problems+="prettier:"$'\n'"$out"$'\n'
    fi
    if ! out="$(cd "$root/client" && npx eslint --fix "$crel" 2>&1)"; then
      problems+="eslint:"$'\n'"$out"$'\n'
    fi
    ;;
esac

case "$rel" in
  *.md | *.cs | *.ts | *.vue)
    dash="$(printf '\xe2\x80\x94')"
    if lines="$(grep -n "$dash" "$path")"; then
      problems+="Длинное тире (U+2014) запрещено правилами проекта, замени его на двоеточие, запятую или перестрой фразу:"$'\n'"$lines"$'\n'
    fi
    ;;
esac

after="$(sha1sum "$path")"

if [[ -n "$problems" ]]; then
  {
    echo "Hook lint.sh нашёл проблемы в $rel:"
    echo "$problems"
  } >&2
  exit 2
fi

if [[ "$before" != "$after" ]]; then
  jq -n --arg rel "$rel" '{
    systemMessage: ("lint.sh: файл " + $rel + " автоматически отформатирован"),
    hookSpecificOutput: {
      hookEventName: "PostToolUse",
      additionalContext: ("Hook lint.sh переформатировал " + $rel + ". Перечитай файл перед следующей правкой.")
    }
  }'
fi

exit 0
