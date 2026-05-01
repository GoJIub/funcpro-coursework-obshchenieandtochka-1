# AI Prompts Log

Лог всех обращений к ИИ в рамках проекта.

---

**Дата:** 01.05.2026
**Задача:** Issue #4 — создание контракта языка и инструкций для Codex

**Prompt / темы обращений:**
Сессия с Codex по старту работы Участника 1:
1. Разобрать `TASK.md`, `README.md` и `WORKFLOW.md`.
2. Определить, что нужно делать прямо сейчас, чтобы не блокировать команду.
3. Подготовить описание Issue #4.
4. Создать ветку `docs/language-contract`.
5. Создать начальную спецификацию языка и инструкции для Codex.
6. Синхронизировать лог использования ИИ с решением команды о логах по
   участникам.

**Ответ ИИ / краткое содержание:**
- Рекомендовано начать с первого разблокирующего контракта языка.
- Сформирован текст Issue #4.
- Создана ветка `docs/language-contract`.
- Зафиксированы начальные контракты `Expr`, `Value`, `Env`, `ParseError`,
  `EvalError`, `Parser.parse`, `Evaluator.eval`.
- Добавлены `AGENTS.md` и `docs/language-spec.md`.
- После merge PR #2 лог перенесён в файл Участника 1:
  `docs/ai-usage/01-log-participant-1.md`.

**Что принято:**
- Сначала нужно зафиксировать проектировочный контракт языка.
- `AGENTS.md` и `docs/language-spec.md` относятся к этапу 0.
- F#-каркас проекта вынесен в отдельную ветку `core/project-skeleton`.
- Лог использования ИИ ведётся в файле Участника 1.

**Что изменено человеком:**
- Уточнён milestone/контекст с учётом того, что команда всё ещё закрывает этап 0.
- Уточнён путь лога использования ИИ после договорённости с Участником 3.
- Документационные файлы переведены на русский язык.

**Связанные файлы:**
- `AGENTS.md`
- `docs/language-spec.md`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** pending

---

**Дата:** 02.05.2026
**Задача:** Issue #3 — создание F# project skeleton

**Prompt / темы обращений:**
Сессия с Codex по этапу 1:
1. Разделить работу этапа 0 и этапа 1 на отдельные ветки и PR.
2. Подготовить ветку `core/project-skeleton` поверх актуального `main`.
3. Создать F# solution, проекты `Language`, `Cli`, `Language.Tests`.
4. Добавить файлы `Ast.fs`, `Values.fs`, `Environment.fs`, `EvalError.fs`,
   `Parser.fs`, `Evaluator.fs`.
5. Добавить smoke tests и GitHub Actions workflow.
6. Проверить локальную сборку и разобрать падение CI.
7. Исправить smoke test, который сравнивал `Result<Value, EvalError>` через
   `Assert.Equal`.

**Ответ ИИ / краткое содержание:**
- Создана и обновлена ветка `core/project-skeleton`.
- Этап 1 отделён от документационной ветки `docs/language-contract`.
- Добавлены solution, проекты, CI и минимальные файлы языкового ядра.
- Parser и evaluator оставлены заглушками, чтобы не заходить в задачи
  следующих этапов.
- После падения CI предложено заменить `Assert.Equal` на pattern matching,
  потому что `Value` содержит вариант `VBuiltin` с функцией.

**Что принято:**
- Этап 1 оформляется одним PR, потому что все пункты образуют единый bootstrap.
- Issue #3 остаётся составной задачей `core: create project skeleton`.
- Smoke test для `Environment.lookup` проверяет результат через pattern
  matching.
- CI запускает `dotnet restore`, `dotnet build` и `dotnet test`.

**Что изменено человеком:**
- Команда согласовала, что не нужно создавать отдельные Issue на каждый пункт
  этапа 1.
- PR #6 создан вручную через GitHub UI.
- После падения CI проверен лог GitHub Actions и внесено точечное исправление
  теста.

**Связанные файлы:**
- `FuncProCoursework.sln`
- `.github/workflows/ci.yml`
- `.gitignore`
- `src/Language/*`
- `src/Cli/*`
- `tests/Language.Tests/*`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #6

---
