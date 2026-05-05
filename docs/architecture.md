# Architecture

Этот документ описывает текущую архитектуру LispNT: интерпретатора небольшого
Lisp-like функционального языка на F#.

## Pipeline

```text
source .x
  -> Parser.parse
  -> Expr AST
  -> Evaluator.eval
  -> Value / EvalError
```

CLI (`src/Cli`) читает `.x` файл, передаёт текст в parser, при успешном parse
создаёт начальное окружение со стандартной библиотекой и запускает evaluator.
Опционально CLI может напечатать AST (`--ast`) и включить trace (`--trace`).

## Основные контракты

Ключевые типы живут в `src/Language`:

| Файл | Роль |
|---|---|
| `Ast.fs` | AST типа `Expr` |
| `Values.fs` | runtime-значения `Value`, окружение `Env`, thunk и builtin contract |
| `EvalError.fs` | ошибки parser/evaluator |
| `Environment.fs` | операции над immutable environment |
| `Parser.fs` | tokenizer, S-expression parser и перевод в `Expr` |
| `Evaluator.fs` | eval/apply interpreter |
| `Builtins.fs` | стандартная библиотека |
| `PrettyPrinter.fs` | печать AST и runtime values |
| `ValueFormatting.fs` | имена runtime-типов для ошибок |
| `Trace.fs` | trace-события вычисления |

Общие контракты `Expr`, `Value`, `Env`, `ParseError`, `EvalError`, `parse` и
`eval` являются границами между parser, evaluator, CLI, tests и документацией.

## Parser

`Parser.parse : string -> Result<Expr, ParseError>` работает в два шага:

1. `tokenize` разбивает входной текст на скобки и атомы.
2. `parseSExpr` строит промежуточное S-expression дерево.
3. `toExpr` переводит S-expression в AST.

Parser поддерживает специальные формы:

- `if`;
- `let` и sugar-форму `let name = value body`;
- `let*`;
- `letrec`;
- `cond`;
- `lambda`;
- lambda sugar `=>`;
- `delay`;
- `force`;
- binary `and` / `or`.

`let*`, `cond`, `=>`, `and` и `or` не добавляют новых AST nodes. Они
разворачиваются parser-ом в уже существующие `ELet`, `EIf` и `ELambda`.

Parser не поддерживает строки, комментарии и несколько top-level выражений в
одном файле. Обычная программа должна быть одним выражением.

## AST

AST намеренно небольшой:

- literals: `ENumber`, `EBool`;
- variables: `ESymbol`;
- control flow: `EIf`;
- bindings: `ELet`, `ELetRec`;
- functions: `ELambda`, `EApply`;
- explicit laziness: `EDelay`, `EForce`;
- direct list AST: `EList`.

Большая часть пользовательского синтаксиса является sugar поверх этих форм.

## Environment

Окружение имеет тип:

```fsharp
type Env = Map<string, Value>
```

`Environment.lookup` возвращает `UnboundVariable`, если имя не найдено.
`extend` и `extendMany` создают новое immutable окружение на базе старого.

Замыкания хранят окружение создания:

```fsharp
VClosure of parameters: string list * body: Expr * env: Env
```

Поэтому функции используют lexical scope, а не окружение места вызова.

## Evaluator

`Evaluator.eval : Env -> Expr -> Result<Value, EvalError>` реализует
eval/apply interpreter:

1. literals вычисляются в соответствующие `Value`;
2. symbol ищется в `Env`;
3. `if` сначала вычисляет condition и требует `Bool`;
4. `let` вычисляет value expression и расширяет окружение для body;
5. `letrec` поддерживает рекурсивную lambda-привязку;
6. `lambda` создаёт `VClosure`;
7. application вычисляет callee и аргументы, затем вызывает closure или builtin;
8. `delay` создаёт thunk без вычисления expression;
9. `force` вычисляет thunk и memoize-ит результат;
10. `EList` вычисляет элементы слева направо и возвращает `VList`.

Ошибки выполнения возвращаются как `Error EvalError`; обычные ошибки языка не
пробрасываются исключениями.

## Builtins

`Builtins.makeBuiltins` создаёт начальное окружение стандартной библиотеки.
Builtins имеют контракт:

```fsharp
type BuiltinFunc = Value list -> Result<Value, EvalError>
```

Стандартная библиотека включает:

- арифметику: `+`, `-`, `*`, `/`;
- сравнения: `=`, `<`, `>`;
- boolean builtin: `not`;
- списки: `list`, `head`, `tail`, `cons`, `empty?`;
- higher-order functions: `map`, `filter`, `fold`;
- `Maybe`: `just`, `nothing`, `fmap`, `bind`.

Higher-order builtins умеют применять как closures, так и builtins через общий
helper внутри `Builtins.fs`.

## CLI

CLI живёт в `src/Cli/Program.fs`.

```bash
dotnet run --project src/Cli -- <file.x> [--ast] [--trace] [--help]
```

Поведение:

- без аргументов или с `--help` печатает справку;
- если файл не найден, возвращает exit code `1`;
- parse error печатается как `Parse error: ...`;
- runtime error печатается как `Runtime error: ...`;
- успешный результат печатается как `Result: <value>`.

CLI является единственной IO-частью проекта. В самом языке нет `print`,
строковых literals и builtins для чтения/записи файлов.

## Trace

`Trace.fs` содержит глобальный флаг `Trace.enabled` и функции:

- `enter`;
- `exitWithResult`;
- `exitWithError`.

Evaluator вызывает trace hooks на входе в `eval`, на application и при
`force thunk`. Если trace выключен, hooks ничего не печатают.

Trace предназначен для демонстрации и debugging, а не для изменения семантики
языка.

## Tests

Тесты лежат в `tests/Language.Tests` и покрывают:

- parser и syntax sugar;
- evaluator semantics;
- builtins;
- CLI-related examples через parser/evaluator pipeline;
- runnable programs из `examples/`.

Основная команда:

```bash
DOTNET_ROLL_FORWARD=Major dotnet test FuncProCoursework.sln --configuration Release
```

## Ограничения текущей реализации

- Нет string literals.
- Нет комментариев в `.x` файлах.
- Нет нескольких top-level выражений в одном файле.
- Нет пользовательских algebraic data types.
- Нет IO builtins внутри языка.
- `and` и `or` бинарные.
- `letrec` принимает только lambda value expression.
