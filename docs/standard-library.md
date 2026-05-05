# Standard Library

Стандартная библиотека LispNT добавляется в начальное окружение через
`Builtins.makeBuiltins eval`.

Все builtins принимают уже вычисленные runtime values и возвращают
`Result<Value, EvalError>`.

## Арифметика и сравнения

| Функция | Аргументы | Результат | Пример |
|---|---:|---|---|
| `+` | 2 numbers | number | `(+ 1 2)` -> `3` |
| `-` | 2 numbers | number | `(- 5 3)` -> `2` |
| `*` | 2 numbers | number | `(* 3 4)` -> `12` |
| `/` | 2 numbers | number | `(/ 10 2)` -> `5` |
| `=` | 2 numbers или 2 bools | bool | `(= 1 1)` -> `true` |
| `<` | 2 numbers | bool | `(< 1 2)` -> `true` |
| `>` | 2 numbers | bool | `(> 2 1)` -> `true` |

`/` выполняет целочисленное деление. Деление на ноль возвращает
`DivisionByZero`.

`=` поддерживает только пары numbers и пары booleans. Сравнение значений разных
типов возвращает `TypeMismatch`.

## Логические формы

| Форма | Тип | Описание |
|---|---|---|
| `not` | builtin | принимает один `Bool` и возвращает отрицание |
| `and` | parser sugar | binary short-circuit conjunction |
| `or` | parser sugar | binary short-circuit disjunction |

Примеры:

```lisp
(not true)
(and false (/ 1 0))
(or true (/ 1 0))
```

`and` и `or` не являются builtins: parser разворачивает их в `if`, чтобы второй
аргумент вычислялся только при необходимости.

## Списки

| Функция | Аргументы | Результат | Описание |
|---|---:|---|---|
| `list` | any number | `List` | создать список из аргументов |
| `head` | 1 list | value | первый элемент |
| `tail` | 1 list | `List` | список без первого элемента |
| `cons` | value, list | `List` | добавить элемент в начало |
| `empty?` | 1 list | bool | проверить пустоту |

Примеры:

```lisp
(list 1 2 3)
(head (list 1 2 3))
(tail (list 1 2 3))
(cons 0 (list 1 2))
(empty? (list))
```

Ожидаемые результаты:

```text
[1; 2; 3]
1
[2; 3]
[0; 1; 2]
true
```

Ошибки:

- не-list аргумент возвращает `TypeMismatch("List", actual)`;
- `head` пустого списка возвращает `OtherEvalError "head: empty list"`;
- `tail` пустого списка возвращает `OtherEvalError "tail: empty list"`;
- неправильное число аргументов возвращает `WrongArgumentCount`.

## Функции высшего порядка

| Функция | Аргументы | Результат | Описание |
|---|---:|---|---|
| `map` | function, list | `List` | применить function к каждому элементу |
| `filter` | function, list | `List` | оставить элементы, для которых predicate возвращает `true` |
| `fold` | function, initial value, list | value | левая свёртка списка |

Примеры:

```lisp
(map (lambda (x) (* x 2)) (list 1 2 3))
(filter (lambda (x) (> x 2)) (list 1 2 3 4))
(fold + 0 (list 1 2 3))
```

Ожидаемые результаты:

```text
[2; 4; 6]
[3; 4]
6
```

`map`, `filter` и `fold` могут применять как пользовательские closures, так и
builtins. Если function возвращает ошибку, вся операция возвращает эту ошибку.

`filter` требует, чтобы predicate возвращал `Bool`.

## Maybe

`Maybe` представляет значение, которое может отсутствовать:

```fsharp
VMaybe of Value option
```

| Функция | Аргументы | Результат | Описание |
|---|---:|---|---|
| `just` | 1 value | `Maybe` | обернуть значение |
| `nothing` | 0 | `Maybe` | пустое значение |
| `fmap` | function, maybe | `Maybe` | применить function внутри `Just` |
| `bind` | maybe, function | `Maybe` | монадическая цепочка |

Примеры:

```lisp
(just 5)
(nothing)
(fmap (lambda (x) (* x 2)) (just 5))
(fmap (lambda (x) (* x 2)) (nothing))
(bind (just 4) (lambda (x) (just (* x 2))))
```

Ожидаемые результаты:

```text
Just 5
Nothing
Just 10
Nothing
Just 8
```

Семантика:

- `fmap` применяет function только для `Just value`;
- `fmap` над `Nothing` возвращает `Nothing`;
- `bind` над `Just value` требует, чтобы function вернула `Maybe`;
- `bind` над `Nothing` возвращает `Nothing`.

## Ленивость

Язык строгий по умолчанию. Ленивость реализуется специальными формами
`delay` и `force`.

| Форма | Описание |
|---|---|
| `delay` | создать thunk без вычисления expression |
| `force` | вычислить thunk и вернуть результат |

Пример:

```lisp
(let t (delay (+ 1 2))
  (force t))
```

Ожидаемый результат:

```text
3
```

Повторный `force` того же thunk возвращает cached result без повторного eval.
Thunk захватывает lexical environment в момент `delay`.

## Что не входит в стандартную библиотеку

В текущей версии нет:

- `print`;
- string operations;
- file IO;
- mutation;
- n-ary `and` / `or`.
