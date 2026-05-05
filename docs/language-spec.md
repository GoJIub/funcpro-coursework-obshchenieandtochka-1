# Language Specification

Этот документ фиксирует текущую семантику LispNT и контракты, от которых
зависят parser, evaluator, стандартная библиотека, examples и tests.

## Обзор

LispNT — небольшой строгий динамически типизированный функциональный язык с
Lisp-like S-expression синтаксисом.

Язык поддерживает:

- integer numbers;
- boolean values;
- variables;
- `if`;
- `let`, `let*`, `letrec`;
- `lambda` и lambda sugar `=>`;
- lexical closures;
- function application;
- lists;
- higher-order builtins;
- `Maybe`;
- explicit laziness через `delay` / `force`;
- binary short-circuit `and` / `or`;
- runtime errors через `Result<Value, EvalError>`.

## Ограничения

Текущая версия языка не поддерживает:

- string literals;
- comments в `.x` файлах;
- несколько top-level выражений в одном файле;
- пользовательские algebraic data types;
- pattern matching;
- IO builtins (`print`, чтение/запись файлов).

CLI умеет читать `.x` файл с диска, но это поведение CLI, а не встроенная
возможность языка.

## Concrete Syntax

Синтаксис основан на S-expressions:

```lisp
42
true
false
x
(if condition then-expr else-expr)
(let name value-expr body)
(let name = value-expr body)
(let* ((name1 value1) (name2 value2)) body)
(letrec name value-expr body)
(cond (condition1 result1) (true fallback-result))
(lambda (arg1 arg2) body)
(arg => body)
((arg1 arg2) => body)
(delay expr)
(force expr)
(not expr)
(and left right)
(or left right)
(function arg1 arg2)
(list item1 item2 item3)
```

Имена переменных и параметров являются atom-ами. Числа, `true` и `false` нельзя
использовать как parameter names.

## AST Contract

```fsharp
type Expr =
    | ENumber of int
    | EBool of bool
    | ESymbol of string
    | EIf of condition: Expr * thenBranch: Expr * elseBranch: Expr
    | ELet of name: string * valueExpr: Expr * body: Expr
    | ELetRec of name: string * valueExpr: Expr * body: Expr
    | ELambda of parameters: string list * body: Expr
    | EDelay of Expr
    | EForce of Expr
    | EApply of callee: Expr * arguments: Expr list
    | EList of Expr list
```

`EList` остаётся частью AST-контракта и поддерживается evaluator-ом, но обычная
пользовательская форма `(list ...)` парсится как application builtin-а `list`.

## Runtime Values

```fsharp
type Env = Map<string, Value>

type Value =
    | VNumber of int
    | VBool of bool
    | VList of Value list
    | VClosure of parameters: string list * body: Expr * env: Env
    | VBuiltin of name: string * implementation: BuiltinFunc
    | VMaybe of Value option
    | VThunk of Thunk
```

Thunk:

```fsharp
type Thunk =
    { Expression: Expr
      Environment: Env
      mutable CachedValue: Value option }
```

Builtin contract:

```fsharp
type BuiltinFunc = Value list -> Result<Value, EvalError>
```

## Parser Contract

```fsharp
val parse : string -> Result<Expr, ParseError>
```

Parser принимает ровно одно top-level выражение. Если после первого выражения
остались tokens, возвращается parse error.

### Parser Sugar

Следующие формы разворачиваются parser-ом в базовый AST:

| Syntax | Expansion |
|---|---|
| `(let name = value body)` | `ELet(name, value, body)` |
| `(let* ((x 1) (y 2)) body)` | nested `ELet` |
| `(cond (c1 r1) (true fallback))` | nested `EIf` |
| `(arg => body)` | `ELambda([arg], body)` |
| `((a b) => body)` | `ELambda([a; b], body)` |
| `(and left right)` | `EIf(left, right, false)` |
| `(or left right)` | `EIf(left, true, right)` |

`cond` должен иметь последнюю ветку `(true fallback)`. Пустой `cond` и
некорректные clauses возвращают parse error.

`and` и `or` бинарные. N-ary формы не поддерживаются.

## Evaluation Semantics

```fsharp
val eval : Env -> Expr -> Result<Value, EvalError>
```

### Literals and Variables

- `ENumber n` -> `VNumber n`.
- `EBool b` -> `VBool b`.
- `ESymbol name` ищется в текущем `Env`.
- Отсутствующее имя возвращает `UnboundVariable name`.

### If

`EIf(condition, thenBranch, elseBranch)` сначала вычисляет `condition`.

- `VBool true` выбирает `thenBranch`.
- `VBool false` выбирает `elseBranch`.
- Любой другой runtime type возвращает `TypeMismatch("Bool", actual)`.

Только выбранная ветка вычисляется.

### Let

`ELet(name, valueExpr, body)` сначала вычисляет `valueExpr` в текущем
окружении, затем вычисляет `body` в окружении с новой привязкой `name`.

Вложенные `let` могут затенять внешние имена.

### Letrec

`ELetRec(name, valueExpr, body)` поддерживает рекурсивную привязку только если
`valueExpr` является `ELambda`.

В этом случае создаётся `VClosure`, окружение которого уже содержит привязку
`name` к этому же closure. Поэтому тело функции может рекурсивно вызывать само
себя.

Если `valueExpr` не `ELambda`, evaluator возвращает:

```text
OtherEvalError "letrec requires lambda value expression."
```

### Lambda and Application

`ELambda(parameters, body)` создаёт `VClosure(parameters, body, env)`, где `env`
— окружение создания lambda.

`EApply(calleeExpr, argumentExprs)`:

1. вычисляет `calleeExpr`;
2. вычисляет аргументы слева направо;
3. если callee — `VClosure`, проверяет arity, связывает параметры с аргументами
   в окружении closure и вычисляет body;
4. если callee — `VBuiltin`, вызывает builtin implementation;
5. иначе возвращает `NotAFunction actual`.

Closures используют lexical scope: свободные переменные берутся из окружения
создания функции, а не из окружения вызова.

### Lists

`EList items` вычисляет элементы слева направо в текущем окружении и возвращает
`VList values`. Пустой `EList []` возвращает `VList []`.

Если вычисление любого элемента возвращает ошибку, ошибка становится ошибкой
всего списка.

Пользовательская форма `(list ...)` реализована как builtin и возвращает
`VList` из уже вычисленных аргументов.

### Maybe

`Maybe` представлен как:

```fsharp
VMaybe of Value option
```

- `(just value)` возвращает `VMaybe(Some value)`;
- `(nothing)` возвращает `VMaybe None`;
- `(fmap f maybe)` применяет `f` к значению внутри `just`, но оставляет
  `nothing` без изменений;
- `(bind maybe f)` применяет `f` к значению внутри `just` и требует, чтобы `f`
  вернула `Maybe`; `nothing` short-circuit-ит цепочку.

### Explicit Laziness

`EDelay expr` создаёт `VThunk` и не вычисляет `expr`.

Thunk хранит:

- исходное expression;
- environment создания;
- mutable cache для результата.

`EForce expr` сначала вычисляет `expr`.

- Если результат `VThunk`, thunk вычисляется в сохранённом environment.
- Успешный результат сохраняется в `CachedValue`.
- Повторный `force` возвращает cached value без повторного eval.
- Если результат не thunk, возвращается `TypeMismatch("Thunk", actual)`.

### Logical Forms

`not` — обычный builtin, который принимает один `Bool`.

`and` и `or` реализованы как parser sugar поверх `if`, поэтому сохраняют
short-circuit семантику:

- `(and left right)` вычисляет `right` только если `left` равен `true`;
- `(or left right)` вычисляет `right` только если `left` равен `false`.

## Errors

```fsharp
type ParseError =
    | InvalidSyntax of message: string
    | UnexpectedToken of token: string
    | UnexpectedEndOfInput

type EvalError =
    | UnboundVariable of name: string
    | TypeMismatch of expected: string * actual: string
    | NotAFunction of actual: string
    | WrongArgumentCount of expected: int * actual: int
    | DivisionByZero
    | OtherEvalError of message: string
```

Текущий parser в большинстве случаев возвращает `InvalidSyntax message`.
`UnexpectedToken` и `UnexpectedEndOfInput` остаются частью public contract.

Evaluator не выбрасывает исключения для обычных ошибок языка.

## Standard Environment

Начальное окружение CLI создаётся через `Builtins.makeBuiltins eval`.

В нём доступны:

- `+`, `-`, `*`, `/`;
- `=`, `<`, `>`;
- `not`;
- `list`, `head`, `tail`, `cons`, `empty?`;
- `map`, `filter`, `fold`;
- `just`, `nothing`, `fmap`, `bind`.
