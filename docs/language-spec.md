# Спецификация языка

Этот документ фиксирует начальный контракт языка, от которого зависят парсер,
интерпретатор, стандартная библиотека, примеры и документация.

## Цели

Проект реализует небольшой строгий динамически типизированный функциональный
язык с Lisp-like синтаксисом на F#.

Минимальная версия языка должна поддерживать:

- числа и булевы значения;
- переменные;
- `if`;
- `let`;
- `lambda`;
- применение функций;
- замыкания и лексическую область видимости;
- `letrec` и рекурсию;
- списки;
- встроенные функции;
- ошибки выполнения через `Result`.

## Черновик синтаксиса

Синтаксис основан на S-выражениях.

```lisp
42
true
false
x
(if condition then-expr else-expr)
(let name value-expr body)
(letrec name value-expr body)
(lambda (arg1 arg2) body)
(function arg1 arg2)
(list item1 item2 item3)
```

## Контракт AST

```fsharp
type Expr =
    | ENumber of int
    | EBool of bool
    | ESymbol of string
    | EIf of condition: Expr * thenBranch: Expr * elseBranch: Expr
    | ELet of name: string * valueExpr: Expr * body: Expr
    | ELetRec of name: string * valueExpr: Expr * body: Expr
    | ELambda of parameters: string list * body: Expr
    | EApply of callee: Expr * arguments: Expr list
    | EList of Expr list
```

## Контракт runtime-значений

```fsharp
type Value =
    | VNumber of int
    | VBool of bool
    | VList of Value list
    | VClosure of parameters: string list * body: Expr * env: Env
    | VBuiltin of name: string * implementation: BuiltinFunc
    | VMaybe of Value option
    | VThunk of Thunk
```

Контракт окружения:

```fsharp
type Env = Map<string, Value>
```

## Контракт парсера

```fsharp
val parse : string -> Result<Expr, ParseError>
```

Первый каркас проекта содержит только заглушку парсера. Полноценная реализация
парсера относится к зоне ответственности Участника 2.

## Контракт интерпретатора

```fsharp
val eval : Env -> Expr -> Result<Value, EvalError>
```

Первый каркас проекта содержит только заглушку интерпретатора. Полноценная
реализация интерпретатора относится к зоне ответственности Участника 1.

## Контракт ошибок

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

Обычные ошибки языка должны быть представлены как данные, а не выбрасываться как
исключения.
