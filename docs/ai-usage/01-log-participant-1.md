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

**Связанный PR:** #5

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

**Дата:** 02.05.2026
**Задача:** Issue #7 — вычисление literals, variables, `if` и `let`

**Prompt / темы обращений:**
Сессия с Codex по первому слою интерпретатора:
1. Определить ближайшую задачу этапа 3 после закрытия project skeleton.
2. Подготовить описание Issue #7.
3. Реализовать `eval` для `ENumber`, `EBool`, `ESymbol`, `EIf`, `ELet`.
4. Добавить тесты интерпретатора через прямое создание AST, без парсера.
5. Проверить сборку и зафиксировать ограничение локального запуска тестов.

**Ответ ИИ / краткое содержание:**
- Рекомендовано начать с `core: eval literals, if and let`.
- Реализован первый рабочий слой `Evaluator.eval`.
- Для `ESymbol` используется `Environment.lookup`.
- Для `EIf` проверяется, что условие вычисляется в `VBool`.
- Для `ELet` вычисляется value expression, затем body в расширенном окружении.
- Для пока не реализованных AST-узлов оставлены явные `OtherEvalError`.
- Добавлен helper для человекочитаемых имён runtime-типов в `TypeMismatch`.

**Что принято:**
- Тесты интерпретатора на этом этапе создают AST напрямую.
- Parser, builtins, lambda/application, closures, `letrec` и recursion не входят
  в Issue #7.
- Ошибка типа для `if` должна возвращать `TypeMismatch("Bool", actualType)`.

**Что изменено человеком:**
- Scope задачи ограничен первым слоем интерпретатора.
- Проверена локальная сборка через `dotnet build`.
- Локальный `dotnet test` не запускается из-за отсутствующего .NET 8 runtime на
  машине; проверка тестов ожидается в CI.

**Связанные файлы:**
- `src/Language/Evaluator.fs`
- `tests/Language.Tests/Tests.fs`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #8

---

**Дата:** 02.05.2026
**Задача:** Issue #9 — вычисление `lambda` и application

**Prompt / темы обращений:**
Сессия с Codex по следующему слою интерпретатора:
1. Переключиться с ветки прошлого PR на `main` и подтянуть свежие merge коллег.
2. Создать ветку `core/eval-lambda-application`.
3. Реализовать `eval ELambda` и `eval EApply` без изменения контрактов AST,
   runtime-значений и ошибок.
4. Добавить evaluator tests для замыканий, применения, порядка вычисления
   аргументов и ошибок `WrongArgumentCount` / `NotAFunction`.
5. Обновить спецификацию языка по изменённой семантике.

**Ответ ИИ / краткое содержание:**
- `main` обновлён fast-forward перед началом работы.
- `ELambda` теперь вычисляется в `VClosure` с текущим лексическим окружением.
- `EApply` вычисляет callee и аргументы до применения.
- Для `VClosure` проверяется arity, параметры связываются в окружении
  замыкания, затем вычисляется тело.
- Вызов не-функции возвращает `NotAFunction`, неверное количество аргументов —
  `WrongArgumentCount`.

**Что принято:**
- Issue #9 не меняет `letrec`, recursion, parser, списки и контракты ошибок.
- Тесты продолжают создавать AST напрямую, без зависимости от parser.
- Документация обновляется только по реализованной семантике evaluator.

**Что изменено человеком:**
- Issue #9 выбран после approval PR #8.
- Уточнено, что перед работой нужно подтянуть свежий `main`.

**Связанные файлы:**
- `src/Language/Evaluator.fs`
- `tests/Language.Tests/EvaluatorTests.fs`
- `docs/language-spec.md`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #21

---

**Дата:** 02.05.2026
**Задача:** Issue #10 — замыкания и лексическая область видимости

**Prompt / темы обращений:**
Сессия с Codex после approval PR #9:
1. Переключиться на `main` и подтянуть свежие merge.
2. Создать ветку `core/eval-closures-lexical-scope`.
3. Проверить, какая часть closure semantics уже реализована в Issue #9.
4. Добавить сфокусированные evaluator tests для lexical scope и shadowing.
5. Уточнить спецификацию языка по поведению замыканий.

**Ответ ИИ / краткое содержание:**
- Подтверждено, что `VClosure` уже хранит окружение создания, а применение
  выполняет тело в окружении замыкания.
- Добавлены регрессионные тесты на затенение captured-переменной параметром
  функции.
- Добавлен тест на затенение captured-переменной вложенным `let`.
- Добавлен тест на вложенное замыкание, которое сохраняет окружение вызова
  функции-фабрики.
- В спецификации уточнено, что замыкания используют окружение создания, а не
  окружение вызова.

**Что принято:**
- Issue #10 не меняет контракты `Expr`, `Value`, `Env`, `EvalError` и
  сигнатуру `eval`.
- Реализация evaluator из Issue #9 остаётся корректной; задача #10 усиливает
  покрытие и фиксирует семантику в документации.
- Parser, `letrec`, recursion, списки и builtins не входят в scope.

**Что изменено человеком:**
- Issue #10 выбран после approval PR #9.
- Scope уточнён как проверка и закрепление lexical scope semantics.

**Связанные файлы:**
- `tests/Language.Tests/EvaluatorTests.fs`
- `docs/language-spec.md`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #24

---

**Дата:** 03.05.2026
**Задача:** Refactor — общий helper для имени runtime-типа `Value`

**Prompt / темы обращений:**
Сессия с Codex после review PR #23:
1. Разобрать договорённость с Участником 3 про `valueTypeName`.
2. Подтянуть свежий `main` после merge PR с `Builtins.makeBuiltins`.
3. Вынести `valueTypeName` из `Evaluator.fs` в общий internal helper.
4. Перевести `Evaluator.fs` и `Builtins.fs` на общий helper.
5. Проверить сборку и Release-тесты.

**Ответ ИИ / краткое содержание:**
- Создан модуль `ValueFormatting` с `internal` helper `valueTypeName`.
- `Evaluator.fs` больше не хранит приватную копию `valueTypeName`.
- `Builtins.fs` использует тот же helper для `TypeMismatch` и `NotAFunction`.
- Публичные контракты `Expr`, `Value`, `Env`, `EvalError`, `ParseError`,
  `parse` и `eval` не менялись.

**Что принято:**
- Helper остаётся `internal`, чтобы не расширять публичный API языка.
- Это технический refactor после появления второго места использования.
- Семантика evaluator и builtins не меняется.

**Что изменено человеком:**
- Решено сделать отдельный маленький refactor issue/PR после merge PR #23.

**Связанные файлы:**
- `src/Language/ValueFormatting.fs`
- `src/Language/Evaluator.fs`
- `src/Language/Builtins.fs`
- `src/Language/Language.fsproj`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #26

---

**Дата:** 03.05.2026
**Задача:** Issue #11 — `letrec` и рекурсия

**Prompt / темы обращений:**
Сессия с Codex по последней задаче этапа Core Interpreter:
1. Переключиться на `main` и подтянуть свежие merge после approval refactor PR.
2. Создать ветку `core/eval-letrec-recursion`.
3. Реализовать `eval ELetRec` для рекурсивных функций без изменения контрактов
   `Expr`, `Value`, `Env`, `EvalError` и сигнатуры `eval`.
4. Добавить evaluator tests для доступа функции к собственному имени,
   рекурсивного factorial через AST, ошибки некорректного `letrec` и ошибки
   неверного применения рекурсивной функции.
5. Обновить спецификацию языка по семантике `letrec`.

**Ответ ИИ / краткое содержание:**
- `ELetRec` реализован для случая, когда `valueExpr` является `ELambda`.
- Для рекурсивной функции создаётся `VClosure`, окружение которого содержит
  связывание имени функции с самим этим замыканием.
- Некорректный `letrec` с не-lambda значением возвращает `OtherEvalError`.
- Добавлены тесты на self-binding, factorial, non-lambda `letrec` и
  `WrongArgumentCount` внутри рекурсивного вызова.

**Что принято:**
- Issue #11 не меняет runtime-контракты и не вводит mutable environment.
- Parser, примеры `.x`, списки и новые builtins не входят в scope.
- Factorial проверяется через прямой AST и уже существующие arithmetic builtins.

**Что изменено человеком:**
- Issue #11 выбран после approval refactor PR.

**Связанные файлы:**
- `src/Language/Evaluator.fs`
- `tests/Language.Tests/EvaluatorTests.fs`
- `docs/language-spec.md`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #27

---

**Дата:** 03.05.2026
**Задача:** Issue #32 — core-поддержка `delay` и `force`

**Prompt / темы обращений:**
Сессия с Codex по core-части явной ленивости:
1. Разобрать umbrella Issue #30 и sub-issue #32.
2. Реализовать `EDelay` / `EForce` в AST и evaluator без изменений парсера.
3. Добавить memoization cache в `VThunk`.
4. Покрыть evaluator tests через прямое создание AST.
5. Обновить спецификацию языка и проверить сборку/тесты.

**Ответ ИИ / краткое содержание:**
- `delay` и `force` оформлены как формы языка, а не обычные builtins.
- В AST добавлены `EDelay` и `EForce`.
- `Thunk` теперь хранит исходное выражение, окружение создания и mutable
  `CachedValue`.
- `EDelay` создаёт `VThunk` без вычисления выражения.
- `EForce` вычисляет thunk в окружении создания, мемоизирует результат и
  возвращает cached value при повторном force.
- Добавлены evaluator tests для отложенного вычисления, lexical environment,
  cache и ошибки типа.

**Что принято:**
- Parser support для `(delay expr)` / `(force expr)` остаётся отдельной задачей
  Участника 2.
- Examples и пользовательская документация остаются отдельной задачей
  Участника 3.
- Мутация допускается только в `CachedValue`, потому что она нужна для
  memoization thunk-ов.

**Что изменено человеком:**
- Команда согласовала разбить lazy evaluation на umbrella Issue и отдельные
  sub-issues по зонам ответственности.
- Работа ведётся в отдельной ветке `core/lazy-delay-force`.

**Связанные файлы:**
- `src/Language/Ast.fs`
- `src/Language/Values.fs`
- `src/Language/Evaluator.fs`
- `tests/Language.Tests/EvaluatorTests.fs`
- `docs/language-spec.md`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #36

---

**Дата:** 04.05.2026
**Задача:** Issue #60 — реализовать вычисление `EList` в evaluator

**Prompt / темы обращений:**
Сессия с Codex по техническому долгу в AST/evaluator:
1. Разобрать предложение команды про `EList`, который присутствует в AST, но
   в evaluator возвращает заглушку `OtherEvalError`.
2. Выбрать между удалением `EList` из контракта и реализацией его семантики.
3. Реализовать `EList` без изменения публичных контрактов `Expr`, `Value`,
   `Env`, `EvalError` и сигнатуры `eval`.
4. Добавить evaluator tests для пустого списка, вычисления элементов в
   окружении и propagation ошибки элемента.
5. Обновить спецификацию языка.

**Ответ ИИ / краткое содержание:**
- Рекомендовано не удалять `EList`, потому что это часть уже зафиксированного
  AST-контракта.
- `EList` реализован через существующий механизм последовательного вычисления
  выражений и возвращает `VList`.
- Ошибка вычисления любого элемента списка возвращается как ошибка всего
  выражения.
- Builtin `(list ...)` не менялся и продолжает работать как раньше.

**Что принято:**
- Scope ограничен evaluator semantics для существующего AST-варианта.
- Parser, builtin `list`, `Value`, `EvalError` и формат trace-событий не
  меняются.

**Связанные файлы:**
- `src/Language/Evaluator.fs`
- `tests/Language.Tests/EvaluatorTests.fs`
- `docs/language-spec.md`
- `docs/ai-usage/01-log-participant-1.md`

**Связанный PR:** #61

---
