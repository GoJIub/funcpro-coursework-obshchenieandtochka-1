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
- явную ленивость через `delay` и `force`;
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
(let* ((name1 value1) (name2 value2)) body)
(letrec name value-expr body)
(cond (condition1 result1) (true fallback-result))
(lambda (arg1 arg2) body)
(delay expr)
(force expr)
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
    | EDelay of Expr
    | EForce of Expr
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

Контракт thunk:

```fsharp
type Thunk =
    { Expression: Expr
      Environment: Env
      mutable CachedValue: Value option }
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

Parser sugar:

- `(let name = value-expr body)` эквивалентен `(let name value-expr body)`.
- `(let* ((name1 value1) (name2 value2)) body)` разворачивается в цепочку
  вложенных `ELet`.
- `(cond (condition1 result1) (true fallback-result))` разворачивается в
  цепочку вложенных `EIf`; последняя ветка должна иметь условие `true`.
- `(param => body)` и `((param1 param2) => body)` эквивалентны `lambda`.

## Контракт интерпретатора

```fsharp
val eval : Env -> Expr -> Result<Value, EvalError>
```

Первый каркас проекта содержит только заглушку интерпретатора. Полноценная
реализация интерпретатора относится к зоне ответственности Участника 1.

Семантика уже реализованных выражений:

- `ENumber` и `EBool` вычисляются в соответствующие runtime-значения.
- `ESymbol` ищется в текущем окружении.
- `EIf` сначала вычисляет условие; ветка выбирается только для `VBool`.
- `ELet` сначала вычисляет значение, затем тело в расширенном окружении.
- `ELambda` вычисляется в `VClosure` с текущим лексическим окружением.
- `EApply` сначала вычисляет вызываемое выражение и аргументы, затем применяет
  функцию. Для `VClosure` проверяется количество аргументов, параметры
  связываются с вычисленными аргументами в окружении замыкания, после чего
  вычисляется тело. Попытка вызвать не-функцию возвращает `NotAFunction`.
- Замыкания используют окружение создания, а не окружение вызова. Параметры
  функции и вложенные `let`-bindings могут предсказуемо затенять имена из
  сохранённого окружения.
- `ELetRec` поддерживает рекурсивное связывание имени с `ELambda`. Замыкание
  получает окружение, где его имя уже связано с самим замыканием, поэтому тело
  функции может вызывать саму себя. Некорректный `letrec` с не-lambda значением
  возвращает `OtherEvalError`.
- `EDelay` не вычисляет вложенное выражение сразу. Вместо этого создаётся
  `VThunk`, который хранит исходное выражение, окружение создания и пустой cache.
- `EForce` сначала вычисляет выражение до runtime-значения. Если получен
  `VThunk`, он вычисляет сохранённое выражение в окружении создания thunk-а,
  сохраняет результат в `CachedValue` и возвращает его. Повторный `EForce`
  возвращает cached value без повторного вычисления. Для не-thunk значения
  возвращается `TypeMismatch("Thunk", actual)`.
- `EList` вычисляет элементы списка слева направо в текущем окружении и
  возвращает `VList` с вычисленными значениями. Пустой `EList []` возвращает
  `VList []`. Ошибка вычисления любого элемента возвращается как ошибка всего
  списка.

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
