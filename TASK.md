# Итоговый план работы над курсовым проектом

Проект: собственный функциональный язык программирования  
Команда: 3 человека  
Основная стратегия: строгий динамически типизированный Lisp-like язык на F# с eval-apply интерпретатором, лексическими замыканиями, `letrec`, списками, функциями высшего порядка, `Maybe`, явной ленивостью через `delay`/`force` и одной собственной фишкой — трассировщиком вычислений.



> Обновление: в этот план добавлены конкретная схема взаимодействия участников, рабочий pipeline, CI pipeline, порядок первых PR и ответ на вопрос, кто должен первым реализовать часть своей зоны ответственности, чтобы остальные могли работать параллельно.

---

## 1. Цель проекта

Сделать не приложение на функциональном языке, а маленький собственный функциональный язык программирования.

Минимальная цепочка выполнения:

```text
исходный код .x
    ↓
парсер
    ↓
AST
    ↓
интерпретатор
    ↓
результат выполнения / ошибка
```

Главный критерий готовности: можно запустить файл `.x`, содержащий программу на нашем языке, и получить корректный результат.

---

## 2. Выбранный путь: интерпретатор, а не полноценный компилятор

### Решение

Основная реализация — интерпретатор.

Компиляция в байт-код — не обязательная часть MVP. Её можно добавить только после готового интерпретатора как stretch goal.

### Почему интерпретатор

- проще довести до рабочего состояния;
- лучше подходит для демонстрации лямбда-исчисления, замыканий, `letrec`, ленивости;
- меньше инфраструктурного кода;
- проще тестировать;
- лучше соответствует учебной задаче реализации функционального языка.

### Что значит “частичная компиляция”

Частичная компиляция — это не замена интерпретатора, а дополнительный backend для части языка.

Например:

```text
AST
 ├── Interpreter backend
 └── Bytecode compiler backend для простых выражений
```

Можно скомпилировать только:

- числа;
- булевы значения;
- арифметику;
- сравнения;
- `if`;
- простые `let`;
- вызовы встроенных функций.

А сложные конструкции оставить интерпретатору:

- замыкания;
- `letrec`;
- `delay`/`force`;
- `Maybe`;
- функции высшего порядка.

Пример байт-кода:

```text
PUSH_INT 5
PUSH_INT 3
ADD
PRINT
```

Смысл частичной компиляции:

- показать архитектурное понимание разницы между интерпретацией и компиляцией;
- получить “интересную фишку” без риска завалить основной проект;
- использовать интерпретатор как эталонную семантику;
- сделать независимую проверку части языка через VM.

Решение команды: байт-код не входит в нижнюю границу. Делать только если основное ядро готово.

---

## 3. Нижняя граница проекта для команды из 3 человек

Ниже этого уровня опускаться нельзя.

### Ядро языка

- [ ] числа;
- [ ] булевы значения;
- [ ] переменные;
- [ ] `if`;
- [ ] `let`;
- [ ] `lambda`;
- [ ] применение функций;
- [ ] `letrec`;
- [ ] рекурсия;
- [ ] замыкания;
- [ ] списки;
- [ ] функции высшего порядка;
- [ ] встроенные функции;
- [ ] нормальные ошибки выполнения.

### Инфраструктура

- [ ] CLI-запуск `.x` файлов;
- [ ] тесты парсера;
- [ ] тесты интерпретатора;
- [ ] примеры программ;
- [ ] README;
- [ ] GitHub Pages;
- [ ] документация по архитектуре;
- [ ] документация по использованию ИИ.

### Демонстрационные программы

- [ ] `factorial.x`;
- [ ] `fibonacci.x`;
- [ ] `closure.x`;
- [ ] `sum-list.x`;
- [ ] `map.x`;
- [ ] `filter.x`;
- [ ] `fold.x`;
- [ ] `maybe.x`;
- [ ] `lazy.x`;
- [ ] `trace.x`.

---

## 4. Типизация и стратегия вычисления

### Типизация

Выбранный вариант:

```text
strong dynamic typing
```

То есть типы проверяются во время выполнения, но не происходит неявных преобразований.

Примеры ошибок:

```lisp
(+ 1 true)      ; ошибка
(head 10)      ; ошибка
(5 10)         ; ошибка
```

### Почему не статическая типизация

Статическая типизация потребовала бы:

- AST для типов;
- type environment;
- type checker;
- проверку функций;
- проверку списков;
- проверку `letrec`;
- возможно унификацию;
- возможно Hindley–Milner inference.

Это слишком большой объём для текущей цели.

### Стратегия вычисления

По умолчанию язык строгий/eager.

Ленивость реализуется явно:

```lisp
(delay expr)
(force lazy-value)
```

---

## 5. Реализуемые элементы теории ФП

### Обязательные

- лямбда-исчисление;
- переменные;
- lambda-абстракция;
- применение функций;
- `let`;
- `letrec`;
- рекурсия;
- функции первого класса;
- функции высшего порядка;
- замыкания;
- лексическая область видимости;
- иммутабельность;
- списки;
- eval-apply интерпретатор.

### Желательные

- явная ленивость через `delay`/`force`;
- `Maybe`;
- `fmap`;
- `bind`;
- обработка ошибок через `Result` в реализации;
- трассировка вычислений.

### Не брать в MVP

- полноценная статическая типизация;
- Hindley–Milner;
- настоящие typeclasses;
- полноценная IDE;
- Jupyter;
- VS Code extension;
- трансляция всего языка в JavaScript.

---

## 6. Собственная интересная фишка: трассировщик вычислений

### Идея

Добавить режим запуска, который показывает, как программа вычисляется.

Например:

```bash
dotnet run -- examples/factorial.x --trace
```

Пример вывода:

```text
eval: (fact 3)
  lookup fact => <closure>
  eval arg: 3
  apply fact(3)
    eval: (= n 0) => false
    eval: (* n (fact (- n 1)))
      lookup n => 3
      apply fact(2)
        ...
result: 6
```

### Почему это хорошая фишка

- не указана напрямую в ТЗ;
- полезна для защиты;
- показывает работу интерпретатора;
- помогает отлаживать язык;
- помогает объяснить замыкания, рекурсию, `letrec`, `force`;
- хорошо документируется и демонстрируется.

### Форматы

Минимум:

```bash
--trace
```

Бонус:

```bash
--trace-json
```

JSON-вывод можно потом визуализировать на GitHub Pages.

---

## 7. Архитектура проекта

```text
/src
  /Language
    Ast.fs
    Values.fs
    Environment.fs
    EvalError.fs
    Parser.fs
    Evaluator.fs
    Builtins.fs
    Trace.fs
    PrettyPrinter.fs

  /Cli
    Program.fs

/tests
  ParserTests.fs
  EvaluatorTests.fs
  BuiltinTests.fs
  ExampleTests.fs

/examples
  factorial.x
  fibonacci.x
  closure.x
  sum-list.x
  map.x
  filter.x
  fold.x
  maybe.x
  lazy.x
  trace.x

/docs
  index.md
  syntax.md
  semantics.md
  standard-library.md
  examples.md
  architecture.md
  ai-usage.md
  trace.md

/docs/ai-usage
  00-policy.md
  01-prompts-log.md
  02-team-decisions.md
  03-codex-tasks.md
  04-final-summary.md

.github
  workflows
    ci.yml

README.md
AGENTS.md
```

---

## 8. Разделение обязанностей

## Участник 1 — Semantics / Evaluator

### Зона ответственности

- AST;
- типы значений;
- окружение;
- интерпретатор;
- `let`;
- `lambda`;
- application;
- closures;
- `letrec`;
- рекурсия;
- ошибки вычисления.

### Файлы

```text
src/Language/Ast.fs
src/Language/Values.fs
src/Language/Environment.fs
src/Language/EvalError.fs
src/Language/Evaluator.fs
tests/EvaluatorTests.fs
```

### Definition of Done

- факториал вычисляется;
- `closure.x` работает корректно;
- ошибка `unbound variable` обрабатывается нормально;
- ошибка вызова не-функции обрабатывается нормально;
- есть тесты на `let`, `lambda`, `letrec`, closure.

---

## Участник 2 — Parser / CLI / Syntax

### Зона ответственности

- синтаксис языка;
- парсер;
- pretty-printer;
- CLI;
- чтение `.x` файлов;
- сообщения об ошибках парсинга.

### Файлы

```text
src/Language/Parser.fs
src/Language/PrettyPrinter.fs
src/Cli/Program.fs
tests/ParserTests.fs
docs/syntax.md
```

### Definition of Done

- все примеры из `examples/` парсятся;
- парсер выдаёт понятные ошибки;
- CLI умеет запускать файл;
- CLI поддерживает режимы `--help`, `--ast`, `--trace`;
- синтаксис описан в документации.

---

## Участник 3 — StdLib / Examples / Docs / AI Usage

### Зона ответственности

- встроенные функции;
- списки;
- функции высшего порядка;
- `Maybe`;
- `fmap`;
- `bind`;
- `delay`/`force`;
- примеры программ;
- документация;
- GitHub Pages;
- лог использования ИИ.

### Файлы

```text
src/Language/Builtins.fs
src/Language/Trace.fs
tests/BuiltinTests.fs
tests/ExampleTests.fs
examples/*.x
docs/*.md
docs/ai-usage/*.md
```

### Definition of Done

- работают `list`, `head`, `tail`, `cons`, `empty?`;
- работают `map`, `filter`, `fold`;
- работает `Maybe`;
- работает `delay`/`force`;
- есть документация по стандартной библиотеке;
- есть документация по использованию ИИ;
- GitHub Pages открывается и содержит руководство.


---

## 8.1. Конкретная схема взаимодействия участников

В проекте есть два уровня работы:

```text
1. Общий контракт языка
   AST, Value, Env, ошибки, синтаксис, правила вычисления.

2. Индивидуальные зоны ответственности
   Parser, Evaluator, StdLib, Docs, Examples, Trace.
```

Сначала команда фиксирует общий контракт. Только после этого участники параллельно реализуют свои подсистемы.

Главная опасность:

```text
Парсер написан под один AST.
Интерпретатор написан под другой AST.
Примеры написаны под третий синтаксис.
Документация описывает четвёртый язык.
```

Чтобы этого не произошло, любые изменения в следующих сущностях считаются изменением общего контракта и проходят только через Issue + Pull Request:

```text
Expr
Value
Env
EvalError
ParseError
синтаксис special forms
семантика let / letrec / lambda / application
формат built-in функций
формат trace-событий
```

### Кто с кем синхронизируется

| Участник | Основные синхронизации | О чём договариваться |
|---|---|---|
| Участник 1 — Semantics / Evaluator | Участник 2 и Участник 3 | AST, Value, ошибки, семантика, evaluator contract |
| Участник 2 — Parser / CLI / Syntax | Участник 1 и Участник 3 | синтаксис, формат AST, CLI-режимы, примеры |
| Участник 3 — StdLib / Examples / Docs / AI Usage | Участник 1 и Участник 2 | built-ins, примеры, документация, AI usage log |

### Что нельзя менять без согласования

```text
Expr
Value
Env
EvalError
ParseError
parse signature
eval signature
синтаксис let / letrec / lambda / if
формат built-in функции
формат trace-события
```

Изменение этих вещей оформляется как отдельный Issue и отдельный PR.

### Daily update

Каждый участник в общем чате пишет короткий апдейт:

```markdown
## Daily update

Сделал:
- ...

Делаю дальше:
- ...

Блокер:
- ...

ИИ:
- не использовал / использовал, лог в docs/ai-usage/...
```

### Milestone sync

После каждого milestone команда проверяет:

```text
1. Проект собирается?
2. dotnet test проходит?
3. Примеры запускаются?
4. Документация соответствует текущему языку?
5. AI usage log обновлён?
6. Нет ли расхождения между parser/evaluator/examples?
```

---

## 8.2. Кто должен начать первым

Первым должен начать **участник, отвечающий за Core / Architecture**.

Его первая задача — не написать весь интерпретатор, а создать **общий контракт проекта**.

### Первый критический PR

Ветка:

```text
core/project-skeleton
```

Содержимое первого PR:

```text
solution / projects
src/Language/Ast.fs
src/Language/Values.fs
src/Language/Environment.fs
src/Language/Errors.fs
src/Language/Parser.fs        // заглушка с сигнатурой
src/Language/Evaluator.fs     // заглушка с сигнатурой
src/Cli/Program.fs            // минимальный CLI или заглушка
tests/Language.Tests/SmokeTests.fs
.github/workflows/ci.yml
README.md draft
AGENTS.md
docs/language-spec.md
docs/ai-usage/00-policy.md
```

### Почему именно Core / Architecture начинает первым

Потому что от этой части зависят все остальные:

```text
AST contract → Parser
AST contract → Evaluator
Value contract → Builtins
Parser + Evaluator → CLI
Evaluator + Builtins → Examples
Examples + Spec → Docs
```

Если сначала parser-человек начнёт parser без AST, а evaluator-человек начнёт evaluator без `Value`, они будут вынуждены придумывать локальные контракты. Потом это придётся склеивать.

### Минимальный контракт после первого PR

```fsharp
type Expr =
    | ENumber of int
    | EBool of bool
    | ESymbol of string
    | EIf of Expr * Expr * Expr
    | ELet of string * Expr * Expr
    | ELetRec of string * Expr * Expr
    | ELambda of string list * Expr
    | EApply of Expr * Expr list
    | EList of Expr list
```

```fsharp
type Value =
    | VNumber of int
    | VBool of bool
    | VList of Value list
    | VClosure of string list * Expr * Env
    | VBuiltin of string * (Value list -> Result<Value, EvalError>)
    | VMaybe of Value option
    | VThunk of Thunk

and Env = Map<string, Value>
```

```fsharp
type ParseError =
    | InvalidSyntax of string
    | UnexpectedToken of string
    | UnexpectedEndOfInput

and EvalError =
    | UnboundVariable of string
    | TypeMismatch of expected: string * actual: Value
    | NotAFunction of Value
    | WrongArgumentCount of expected: int * actual: int
    | DivisionByZero
    | OtherEvalError of string
```

```fsharp
val parse : string -> Result<Expr, ParseError>
val eval : Env -> Expr -> Result<Value, EvalError>
```

После этого:

```text
Участник 2 может писать Parser.fs под готовый Expr.
Участник 3 может писать EvaluatorTests.fs руками, создавая AST без парсера.
Участник 1 может развивать Environment/Evaluator и ревьюить архитектурные изменения.
```

---

## 8.3. Параллельная разработка после первого PR

После merge `core/project-skeleton` работа идёт параллельно.

### Участник 1 — Semantics / Evaluator

```text
Issue A1: реализовать Environment
Issue A2: eval для чисел/булевых
Issue A3: eval для if
Issue A4: eval для let
Issue A5: eval для lambda/application
Issue A6: closures
Issue A7: letrec
Issue A8: trace hooks в evaluator
```

### Участник 2 — Parser / CLI / Syntax

```text
Issue B1: parser для atom/list S-expression
Issue B2: parser для if
Issue B3: parser для let
Issue B4: parser для lambda/application
Issue B5: parser для letrec
Issue B6: parser errors
Issue B7: CLI запуск файла
Issue B8: --ast / --trace flags
```

### Участник 3 — StdLib / Examples / Docs / AI Usage

```text
Issue C1: builtins + - * / = < >
Issue C2: list/head/tail/cons/empty?
Issue C3: map/filter/fold
Issue C4: Maybe/fmap/bind
Issue C5: delay/force
Issue C6: examples/*.x
Issue C7: docs + GitHub Pages
Issue C8: AI usage logs
```

---

## 9. Организация работы с GitHub

### Основной процесс

Используем GitHub Flow:

```text
main
  ↑
pull request
  ↑
feature branch
```

Каждая задача делается в отдельной ветке:

```text
feature/parser-basic
feature/evaluator-let
feature/closures
feature/lists
feature/lazy
feature/docs
```

### Правила

- никто не пушит напрямую в `main`;
- каждая задача оформляется через Issue;
- каждая задача закрывается Pull Request;
- PR должен иметь описание;
- перед merge должен пройти CI;
- минимум один человек должен посмотреть PR;
- крупные PR запрещены;
- один PR — одна логическая задача.



### Полный рабочий pipeline команды

```text
Issue
  ↓
Branch
  ↓
Local implementation
  ↓
Local tests
  ↓
Pull Request
  ↓
CI: restore + build + test
  ↓
Code review
  ↓
Docs / examples / AI log updated
  ↓
Merge to main
```

### Формат веток

```text
core/project-skeleton
core/eval-let
core/eval-closures
parser/basic-sexpr
parser/special-forms
cli/run-file
stdlib/lists
stdlib/higher-order-functions
feature/lazy
feature/trace
docs/github-pages
```

Не использовать:

```text
maks-branch
test
new
fix
final-final
```

### Branch protection для `main`

Включить:

```text
Require a pull request before merging
Require approvals: 1
Require status checks to pass: CI / build-test
Do not allow force pushes
Do not allow deletions
```

### Минимальный CI workflow

```yaml
name: CI

on:
  pull_request:
    branches: [ main ]
  push:
    branches: [ main ]

jobs:
  build-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release
```


### Формат Issue

```markdown
## Задача

Что нужно сделать.

## Контекст

Какие файлы затрагиваются.

## Acceptance criteria

- [ ] ...
- [ ] ...

## Тесты

Какие тесты должны пройти.

## AI usage

Планируется ли помощь ИИ: да/нет.
Если да, где будет сохранён prompt/log.
```

### Формат PR

```markdown
## Что сделано

Кратко.

## Как проверить

Команды запуска.

## Тесты

- [ ] dotnet test
- [ ] examples запускаются

## AI usage

- prompt/log: docs/ai-usage/...
- что было изменено человеком:
```

---

## 10. Организация работы с ИИ

### Принцип

ИИ используется как помощник, ревьюер и генератор черновиков, но не как скрытый автор проекта.

### Роли при неравном доступе

Так как Codex есть только у одного участника, этот участник не должен становиться единственным разработчиком.

Его роль:

```text
AI coordinator / architecture reviewer
```

Он помогает:

- разбивать задачи;
- составлять prompts;
- проверять сложные места;
- запускать Codex на ограниченные задачи;
- делать AI-review чужих PR;
- генерировать тест-кейсы;
- улучшать документацию.

Но каждый участник остаётся владельцем своей подсистемы.

### Что делают участники без платных подписок

Они могут:

- писать код самостоятельно;
- использовать бесплатный ChatGPT в пределах доступного;
- готовить prompts в markdown;
- просить AI coordinator прогнать конкретный prompt;
- использовать результаты только после понимания и ручной проверки.

### Что должен делать AI coordinator

- не генерировать большие куски проекта за всех;
- запускать Codex только на issues с чётким scope;
- сохранять prompt и итог;
- требовать от владельца зоны ручной проверки;
- не merge-ить AI-код без review.

### Структура AI-логов

```markdown
# AI usage log

## Дата

### Участник

### Задача

### Prompt

### Ответ ИИ / краткое содержание

### Что было принято

### Что было изменено человеком

### Связанные файлы

### Связанный PR
```

---

## 11. AGENTS.md для Codex

В корне репозитория нужно создать `AGENTS.md`.

Пример:

````markdown
# AGENTS.md

## Project

We are implementing a small functional programming language in F#.

## Main goal

Do not generate a full unrelated language implementation. Follow the existing architecture.

## Architecture

- AST is defined in `src/Language/Ast.fs`
- Runtime values are defined in `src/Language/Values.fs`
- Evaluator is environment-based and returns `Result<Value, EvalError>`
- Parser reads Lisp-like S-expressions

## Coding rules

- Prefer immutable data structures.
- Use pattern matching.
- Avoid hidden mutation unless required for lazy thunk memoization.
- Do not throw exceptions for normal language errors.
- Return `Result`.
- Add tests for every semantic feature.
- Keep PRs small.

## Commands

Run tests:

```bash
dotnet test
```

Run example:

```bash
dotnet run --project src/Cli -- examples/factorial.x
```

## Documentation

If semantics change, update docs.
If AI is used, update `docs/ai-usage`.
````

---

## 12. Этапы разработки

## Этап 0 — Проектирование

### Цель

Зафиксировать язык до начала активной разработки.

### Задачи

- [ ] выбрать название языка;
- [ ] утвердить синтаксис;
- [ ] утвердить список фич;
- [ ] создать структуру репозитория;
- [ ] создать `AGENTS.md`;
- [ ] создать AI usage policy;
- [ ] создать первые Issues.

### Результат

```text
docs/language-spec.md
docs/ai-usage/00-policy.md
AGENTS.md
README.md draft
```


---

## Этап 1 — Project Skeleton

### Цель

Создать техническую основу, чтобы вся команда могла начать параллельную разработку.

### Ответственный

Участник 1 / Core / Architecture owner.

### Задачи

- [ ] создать solution;
- [ ] создать проекты `Language`, `Cli`, `Language.Tests`;
- [ ] добавить AST;
- [ ] добавить Value;
- [ ] добавить Env;
- [ ] добавить ошибки;
- [ ] добавить сигнатуры `parse` и `eval`;
- [ ] добавить smoke test;
- [ ] добавить CI;
- [ ] добавить README draft;
- [ ] добавить `AGENTS.md`.

### Результат

```text
Проект собирается.
CI запускается.
Остальные участники могут работать от общих типов.
```

---

## Этап 2 — Parser + AST

### Цель

Превратить текст программы в AST.

### Задачи

- [ ] описать AST;
- [ ] реализовать парсер чисел, булевых значений, символов;
- [ ] реализовать парсер списочных выражений;
- [ ] реализовать парсер `if`, `let`, `lambda`, `letrec`;
- [ ] добавить тесты парсера;
- [ ] добавить `--ast`.

### Результат

```text
examples/factorial.x парсится в AST
```

---

## Этап 3 — Core Interpreter

### Цель

Запустить базовые программы.

### Задачи

- [ ] реализовать Value;
- [ ] реализовать Environment;
- [ ] реализовать eval;
- [ ] реализовать арифметику;
- [ ] реализовать сравнения;
- [ ] реализовать `if`;
- [ ] реализовать `let`;
- [ ] реализовать `lambda`;
- [ ] реализовать application;
- [ ] реализовать closures;
- [ ] реализовать `letrec`.

### Результат

```text
factorial.x возвращает 120
fibonacci.x возвращает корректное значение
closure.x показывает лексическую область видимости
```

---

## Этап 4 — Functional Features

### Цель

Сделать язык явно функциональным.

### Задачи

- [ ] списки;
- [ ] `head`;
- [ ] `tail`;
- [ ] `cons`;
- [ ] `empty?`;
- [ ] `map`;
- [ ] `filter`;
- [ ] `fold`;
- [ ] `Maybe`;
- [ ] `fmap`;
- [ ] `bind`.

### Результат

```text
map.x, filter.x, fold.x, maybe.x работают
```

---

## Этап 5 — Lazy Evaluation

### Цель

Реализовать явную ленивость.

### Задачи

- [ ] добавить `VThunk`;
- [ ] реализовать `delay`;
- [ ] реализовать `force`;
- [ ] добавить memoization;
- [ ] добавить тест, что повторный `force` не пересчитывает выражение;
- [ ] добавить пример `lazy.x`.

### Результат

```text
delay/force работают и документированы
```

---

## Этап 6 — Interesting Feature: Evaluation Trace

### Цель

Сделать собственную фишку проекта.

### Задачи

- [ ] добавить `Trace.fs`;
- [ ] добавить режим `--trace`;
- [ ] логировать вход в eval;
- [ ] логировать lookup переменных;
- [ ] логировать применение функций;
- [ ] логировать force thunk;
- [ ] добавить пример `trace.x`;
- [ ] описать трассировщик в документации.

### Результат

```text
dotnet run --project src/Cli -- examples/factorial.x --trace
```

---

## Этап 7 — Quality

### Цель

Стабилизировать проект.

### Задачи

- [ ] улучшить ошибки;
- [ ] проверить все примеры;
- [ ] добавить CI;
- [ ] привести код к единому стилю;
- [ ] убрать мёртвый код;
- [ ] проверить README;
- [ ] проверить документацию;
- [ ] проверить AI usage logs.

### Результат

```text
dotnet test проходит
все examples запускаются
README соответствует реальности
```

---

## Этап 8 — Финальная документация и защита

### Цель

Сделать проект понятным для преподавателя.

### Задачи

- [ ] GitHub Pages;
- [ ] architecture.md;
- [ ] syntax.md;
- [ ] standard-library.md;
- [ ] examples.md;
- [ ] ai-usage.md;
- [ ] authors/contributions;
- [ ] финальная проверка ссылок;
- [ ] подготовка сценария демонстрации.

### Результат

Готовый проект с кодом, примерами, документацией и прозрачным описанием использования ИИ.


---

## 12.1. Интеграционные vertical slices

Разработка должна идти не только горизонтально по модулям, но и вертикальными срезами.

### Vertical Slice 1 — arithmetic

Программа:

```lisp
(+ 1 2)
```

Должен пройти полный путь:

```text
source file
→ parser
→ AST
→ evaluator
→ result 3
→ test
→ docs
```

---

### Vertical Slice 2 — let / if

```lisp
(let x 10
  (if (> x 5)
      x
      0))
```

Проверяет:

```text
parser special forms
environment
if semantics
comparison builtins
```

---

### Vertical Slice 3 — lambda / closure

```lisp
(let make-adder
  (lambda (x)
    (lambda (y) (+ x y)))
  ((make-adder 10) 5))
```

Ожидаемый результат:

```text
15
```

Проверяет:

```text
lambda
application
closure
lexical scope
```

---

### Vertical Slice 4 — recursion / factorial

```lisp
(letrec fact
  (lambda (n)
    (if (= n 0)
        1
        (* n (fact (- n 1)))))
  (fact 5))
```

Ожидаемый результат:

```text
120
```

Проверяет:

```text
letrec
recursion
factorial requirement
```

---

### Vertical Slice 5 — lists / higher-order functions

```lisp
(map (lambda (x) (* x x)) (list 1 2 3 4))
```

Ожидаемый результат:

```text
(list 1 4 9 16)
```

Проверяет:

```text
lists
builtins
higher-order functions
lambda as value
```

---

### Vertical Slice 6 — lazy evaluation

```lisp
(let x (delay (+ 1 2))
  (force x))
```

Ожидаемый результат:

```text
3
```

Проверяет:

```text
VThunk
delay
force
memoization
```

---

### Vertical Slice 7 — trace

```bash
dotnet run --project src/Cli -- examples/factorial.x --trace
```

Проверяет:

```text
CLI flag
Trace.fs
Evaluator trace hooks
trace documentation
```

---

## 13. Рекомендуемый порядок приоритета

### Must have

- parser;
- AST;
- interpreter;
- `let`;
- `lambda`;
- `letrec`;
- recursion;
- closures;
- lists;
- examples;
- README;
- AI usage log.

### Should have

- `map`;
- `filter`;
- `fold`;
- `Maybe`;
- `fmap`;
- `bind`;
- GitHub Pages;
- CI;
- `delay`/`force`.

### Could have

- evaluation trace;
- `--trace-json`;
- simple bytecode for arithmetic subset;
- visualizer on GitHub Pages.

### Won't have

- full compiler;
- full static type inference;
- browser IDE;
- VS Code extension;
- Jupyter support;
- full JavaScript transpiler.

---

## 14. Финальный сценарий демонстрации

На защите показать:

1. кратко README;
2. синтаксис языка;
3. AST на простом примере через `--ast`;
4. запуск `factorial.x`;
5. запуск `closure.x`;
6. запуск `map.x`;
7. запуск `maybe.x`;
8. запуск `lazy.x`;
9. запуск `factorial.x --trace`;
10. GitHub Pages;
11. AI usage log;
12. вклад каждого участника.


---

## 16. Источники для организационных решений

- GitHub Flow: <https://docs.github.com/en/get-started/quickstart/github-flow>
- GitHub Actions для .NET: <https://docs.github.com/actions/automating-builds-and-tests/building-and-testing-net>
- Branch protection rules: <https://docs.github.com/articles/enabling-required-reviews-for-pull-requests>
- OpenAI Codex / рекомендации по работе с агентами и `AGENTS.md`: <https://openai.com/business/guides-and-resources/how-openai-uses-codex/>

---

## 17. Главный принцип

Лучше маленький, но целостный язык, чем большой набор недоделанных фич.

Основной критерий:

```text
Любую реализованную возможность команда должна уметь объяснить:
- зачем она нужна;
- как она выглядит в синтаксисе;
- как она представлена в AST;
- как она вычисляется;
- какие ошибки возможны;
- какими тестами она покрыта.
```
