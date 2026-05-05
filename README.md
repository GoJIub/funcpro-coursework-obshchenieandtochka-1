# LispNT

Курсовой проект по функциональному программированию: **LispNT** (Lisp&Tochka),
небольшой строгий динамически типизированный функциональный язык с Lisp-like
синтаксисом, интерпретатором, лексическими замыканиями, рекурсией, списками,
функциями высшего порядка, `Maybe`, явной ленивостью и трассировщиком
вычислений.

Исходные программы пишутся в `.x` файлах и запускаются через CLI:

```text
source .x -> parser -> AST -> evaluator -> value / error
```

## Возможности

| Возможность из задания | Статус | Где смотреть |
|---|---:|---|
| Именованные переменные (`let`) | Да | `docs/syntax.md`, `examples/let-sugar.x` |
| Рекурсия | Да | `letrec`, `examples/factorial.x`, `examples/fibonacci.x` |
| Ленивое вычисление | Да | `delay` / `force`, `examples/lazy.x` |
| Функции | Да | `lambda`, `=>`, application |
| Замыкания | Да | `examples/closure.x`, `examples/arrow-lambda.x` |
| Библиотечные функции: ввод-вывод файлов | Частично | в языке нет IO builtins; CLI читает `.x` файлы |
| Списки / последовательности | Да | `list`, `head`, `tail`, `cons`, `empty?` |
| Библиотечные функции для списков | Да | `map`, `filter`, `fold` |

Дополнительно реализованы:

- `Maybe`: `just`, `nothing`, `fmap`, `bind`;
- синтаксический сахар: `let x = ...`, `let*`, `cond`, `=>`, `and`, `or`;
- short-circuit логика для `and` / `or`;
- режимы CLI `--ast` и `--trace`;
- ошибки выполнения через `Result`, без исключений для обычных ошибок языка;
- CI на GitHub Actions.

## Quick Start

Требуется .NET SDK 10.x.

```bash
dotnet restore FuncProCoursework.sln
dotnet build FuncProCoursework.sln --configuration Release
dotnet test FuncProCoursework.sln --configuration Release
```

Проекты сейчас таргетируют `net8.0`, поэтому при запуске на более новом runtime
можно использовать roll-forward:

```bash
DOTNET_ROLL_FORWARD=Major dotnet test FuncProCoursework.sln --configuration Release
```

## Запуск программ

```bash
dotnet run --project src/Cli -- examples/factorial.x
```

Ожидаемый результат:

```text
Result: 120
```

Печать AST:

```bash
dotnet run --project src/Cli -- examples/factorial.x --ast
```

Трассировка вычислений:

```bash
dotnet run --project src/Cli -- examples/factorial.x --trace
```

Справка CLI:

```bash
dotnet run --project src/Cli -- --help
```

## Короткие примеры

Рекурсивный факториал:

```lisp
(letrec fact
  (lambda (n)
    (if (= n 0)
      1
      (* n (fact (- n 1)))))
  (fact 5))
```

Замыкание:

```lisp
(let make-adder
  (lambda (x)
    (lambda (y) (+ x y)))
  ((make-adder 10) 5))
```

Функции высшего порядка:

```lisp
(map (lambda (x) (* x x)) (list 1 2 3 4 5))
```

Явная ленивость:

```lisp
(let x (delay (+ 1 2))
  (force x))
```

Синтаксический сахар и short-circuit:

```lisp
(if (and (not false) (or false true)) 42 0)
```

## Документация

- [docs/index.md](docs/index.md): навигация по документации;
- [docs/syntax.md](docs/syntax.md): пользовательский синтаксис;
- [docs/language-spec.md](docs/language-spec.md): семантика и контракты;
- [docs/standard-library.md](docs/standard-library.md): стандартная библиотека;
- [docs/architecture.md](docs/architecture.md): архитектура реализации;
- [docs/trace.md](docs/trace.md): режим трассировки;
- [docs/examples.md](docs/examples.md): примеры программ и сценарий демонстрации;
- [docs/ai-usage](docs/ai-usage): раскрытие использования генеративного ИИ.

GitHub Pages в classroom repository недоступен. Markdown-документация в `docs/`
является canonical-версией; внешняя HTML-публикация будет указана отдельно.

## Структура проекта

```text
src/Language/       parser, AST, evaluator, values, builtins, trace
src/Cli/            запуск .x файлов, --ast, --trace
tests/Language.Tests/
                    parser/evaluator/builtin/example tests
examples/           runnable .x programs
docs/               документация проекта
.github/workflows/  CI
```

## Тесты

Основная команда:

```bash
dotnet test FuncProCoursework.sln --configuration Release
```

Локально при необходимости:

```bash
DOTNET_ROLL_FORWARD=Major dotnet test FuncProCoursework.sln --configuration Release
```

Тесты покрывают:

- парсер и ошибки синтаксиса;
- evaluator и ошибки выполнения;
- builtins;
- запуск всех основных examples через parser/evaluator pipeline.

## Команда

| Участник | Зона ответственности |
|---|---|
| Girday | semantic core: AST, values, environment, evaluator, closures, `letrec`, lazy core, quality fixes |
| SergioMartinov31 | parser, syntax, pretty-printer, CLI |
| GoJlub | builtins, lists, higher-order functions, examples, documentation |

Тесты писались всеми участниками в рамках своих зон: semantic core,
parser/CLI, builtins и runnable examples.

## Использование ИИ

ИИ использовался как помощник для планирования, ревью, тест-кейсов,
документации и отдельных реализационных задач. Все изменения проходили через
человеческую проверку, Pull Request и CI.

Политика и логи:

- [docs/ai-usage/00-policy.md](docs/ai-usage/00-policy.md);
- [docs/ai-usage/01-log-participant-1.md](docs/ai-usage/01-log-participant-1.md);
- [docs/ai-usage/01-log-participant-2.md](docs/ai-usage/01-log-participant-2.md);
- [docs/ai-usage/01-log-participant-3.md](docs/ai-usage/01-log-participant-3.md);
- [docs/ai-usage/02-team-decisions.md](docs/ai-usage/02-team-decisions.md).
