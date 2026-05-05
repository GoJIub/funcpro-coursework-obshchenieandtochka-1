# Примеры программ

Все примеры находятся в папке [`examples/`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples) и запускаются через CLI.
Синтаксис языка описан в [`docs/syntax.md`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/docs/syntax.md),
встроенные функции — в [`docs/standard-library.md`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/docs/standard-library.md).

Базовая команда запуска:

```bash
dotnet run --project src/Cli -- examples/<файл>.x
```

Флаги:

- `--ast` — показать AST перед вычислением;
- `--trace` — пошаговая трассировка вычисления.

---

## factorial

**Файл:** [`examples/factorial.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/factorial.x)

Рекурсивное вычисление факториала через `letrec`. Имя `fact` видно внутри
своего тела, что позволяет функции вызывать саму себя.

```lisp
(letrec fact
  (lambda (n)
    (if (= n 0)
      1
      (* n (fact (- n 1)))))
  (fact 5))
```

**Результат:** `120`

**Демонстрирует:** `letrec`, рекурсия

```bash
dotnet run --project src/Cli -- examples/factorial.x
dotnet run --project src/Cli -- examples/factorial.x --trace
```

---

## fibonacci

**Файл:** [`examples/fibonacci.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/fibonacci.x)

Число Фибоначчи через двойную рекурсию. Каждый вызов ветвится на два
рекурсивных вызова с уменьшенными аргументами.

```lisp
(letrec fib
  (lambda (n)
    (if (= n 0)
      0
      (if (= n 1)
        1
        (+ (fib (- n 1)) (fib (- n 2))))))
  (fib 10))
```

**Результат:** `55`

**Демонстрирует:** `letrec`, двойная рекурсия

```bash
dotnet run --project src/Cli -- examples/fibonacci.x
```

---

## closure

**Файл:** [`examples/closure.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/closure.x)

Функция `make-adder` возвращает другую функцию, которая захватывает параметр
`x` из внешнего окружения. При вызове `(make-adder 10)` возвращается замыкание,
«помнящее» значение `x = 10`.

```lisp
(let make-adder
  (lambda (x)
    (lambda (y) (+ x y)))
  ((make-adder 10) 5))
```

**Результат:** `15`

**Демонстрирует:** замыкания, лексическая область видимости

```bash
dotnet run --project src/Cli -- examples/closure.x
dotnet run --project src/Cli -- examples/closure.x --ast
```

---

## sum-list

**Файл:** [`examples/sum-list.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/sum-list.x)

Сумма элементов списка через `fold` с начальным накопителем `0` и функцией `+`.

```lisp
(fold + 0 (list 1 2 3 4 5))
```

**Результат:** `15`

**Демонстрирует:** `fold`, списки, встроенная функция как аргумент

```bash
dotnet run --project src/Cli -- examples/sum-list.x
```

---

## map

**Файл:** [`examples/map.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/map.x)

Применение функции возведения в квадрат к каждому элементу списка. Lambda
передаётся как значение первого класса.

```lisp
(map (lambda (x) (* x x)) (list 1 2 3 4 5))
```

**Результат:** `(list 1 4 9 16 25)`

**Демонстрирует:** `map`, функции высшего порядка, lambda как аргумент

```bash
dotnet run --project src/Cli -- examples/map.x
```

---

## filter

**Файл:** [`examples/filter.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/filter.x)

Фильтрация списка по предикату: оставляются только элементы больше 2.

```lisp
(filter (lambda (x) (> x 2)) (list 1 2 3 4 5))
```

**Результат:** `(list 3 4 5)`

**Демонстрирует:** `filter`, предикат, функции высшего порядка

```bash
dotnet run --project src/Cli -- examples/filter.x
```

---

## fold

**Файл:** [`examples/fold.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/fold.x)

Произведение всех элементов списка через `fold` с начальным накопителем `1`
и функцией `*`.

```lisp
(fold * 1 (list 1 2 3 4 5))
```

**Результат:** `120`

**Демонстрирует:** `fold`, накопитель, встроенная функция как аргумент

```bash
dotnet run --project src/Cli -- examples/fold.x
```

---

## maybe

**Файл:** [`examples/maybe.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/maybe.x)

Применение функции к значению, обёрнутому в `Maybe`, через `fmap`.
Если значение `nothing`, функция не вызывается.

```lisp
(let double
  (lambda (x) (* x 2))
  (fmap double (just 5)))
```

**Результат:** `(just 10)`

**Демонстрирует:** `Maybe`, `fmap`, обработка отсутствующего значения

```bash
dotnet run --project src/Cli -- examples/maybe.x
```

---

## lazy

**Файл:** [`examples/lazy.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/lazy.x)

Базовый пример явной ленивости: выражение `(+ 1 2)` откладывается через
`delay` и вычисляется только при вызове `force`.

```lisp
(let x (delay (+ 1 2))
  (force x))
```

**Результат:** `3`

**Демонстрирует:** `delay`/`force`, явная ленивость

```bash
dotnet run --project src/Cli -- examples/lazy.x
```

---

## lazy-memo

**Файл:** [`examples/lazy-memo.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/lazy-memo.x)

Демонстрация мемоизации: `force` вызывается дважды на одном thunk, но
выражение `(+ 1 2)` вычисляется только один раз. Второй `force` возвращает
кэшированный результат.

```lisp
(let counter (delay (+ 1 2))
  (let a (force counter)
    (let b (force counter)
      b)))
```

**Результат:** `3`

**Демонстрирует:** мемоизация thunk

```bash
dotnet run --project src/Cli -- examples/lazy-memo.x
```

---

## lazy-scope

**Файл:** [`examples/lazy-scope.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/lazy-scope.x)

Thunk захватывает лексическое окружение в момент `delay`, а не в момент
`force`. Переменная `x` повторно связывается со значением `99`, но `force`
возвращает `10` — значение из окружения создания thunk.

```lisp
(let x 10
  (let t (delay x)
    (let x 99
      (force t))))
```

**Результат:** `10`

**Демонстрирует:** лексическое окружение в thunk, порядок захвата окружения

```bash
dotnet run --project src/Cli -- examples/lazy-scope.x
```

---

## lazy-error

**Файл:** [`examples/lazy-error.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/lazy-error.x)

`delay` не вычисляет выражение — даже если оно содержит ошибку. `(/ 1 0)` не
вычисляется, потому что нигде не вызывается `force`, и программа возвращает
`42` без ошибки.

```lisp
(let dangerous (delay (/ 1 0))
  42)
```

**Результат:** `42`

**Демонстрирует:** отложенное вычисление, `delay` не вызывает ошибку сразу

```bash
dotnet run --project src/Cli -- examples/lazy-error.x
```

---

## arrow-lambda

**Файл:** [`examples/arrow-lambda.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/arrow-lambda.x)

Синтаксический сахар `=>` для лямбды с одним параметром. Два вложенных `=>`
создают каррированную функцию — результат эквивалентен `closure.x`.

```lisp
(let make-adder (x => (y => (+ x y)))
  ((make-adder 10) 5))
```

**Результат:** 15

**Демонстрирует:** синтаксический сахар `=>`, каррирование

```bash
dotnet run --project src/Cli -- examples/arrow-lambda.x
```

---

## let-sugar

**Файл:** [`examples/let-sugar.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/let-sugar.x)

Синтаксический сахар `let x = value body` — знак `=` необязателен, оба
варианта записи `let` эквивалентны.

```lisp
(let x = 10
  (let y = 20
    (+ x y)))
```

**Результат:** `30`

**Демонстрирует:** синтаксический сахар `let x = ...`

```bash
dotnet run --project src/Cli -- examples/let-sugar.x
```

---

## let-star

**Файл:** [`examples/let-star.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/let-star.x)

`let*` позволяет использовать ранее связанные переменные в последующих
привязках того же блока. Здесь `z` вычисляется через уже доступные `x` и `y`.

```lisp
(let* ((x 1) (y 2) (z (+ x y)))
  (+ (+ x y) z))
```

**Результат:** `6`

**Демонстрирует:** `let*`, последовательные зависимые привязки

```bash
dotnet run --project src/Cli -- examples/let-star.x
```

---

## cond

**Файл:** [`examples/cond.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/cond.x)

Многоветочное условие через `cond`. Парсер разворачивает его в цепочку
вложенных `if`. Последняя ветка `(true ...)` служит fallback.

```lisp
(let x -3
  (cond
    ((= x 0) 0)
    ((< x 0) -1)
    (true 1)))
```

**Результат:** `-1`

**Демонстрирует:** `cond`, синтаксический сахар для ветвления

```bash
dotnet run --project src/Cli -- examples/cond.x
```

---

## logical-forms

**Файл:** [`examples/logical-forms.x`](https://github.com/GoJIub/funcpro-coursework-obshchenieandtochka-1/blob/main/examples/logical-forms.x)

Short-circuit вычисление логических форм: `(and false ...)` не вычисляет
правую часть, `(or true ...)` тоже. `not` инвертирует булево значение.

```lisp
(if (and (not false) (or false true)) 42 0)
```

**Результат:** `42`

**Демонстрирует:** `and`, `or`, `not`, short-circuit вычисления

```bash
dotnet run --project src/Cli -- examples/logical-forms.x
```

---

## Сценарий демонстрации

От простого к сложному, с акцентом на ключевые возможности языка.

```bash
# 1. AST простой программы — показать структуру языка изнутри
dotnet run --project src/Cli -- examples/closure.x --ast

# 2. Рекурсия
dotnet run --project src/Cli -- examples/factorial.x

# 3. Замыкания и лексическая область видимости
dotnet run --project src/Cli -- examples/closure.x

# 4. Функции высшего порядка
dotnet run --project src/Cli -- examples/map.x

# 5. Maybe
dotnet run --project src/Cli -- examples/maybe.x

# 6. Ленивость — отложенное вычисление и мемоизация
dotnet run --project src/Cli -- examples/lazy-memo.x

# 7. Трассировщик — собственная фишка проекта
dotnet run --project src/Cli -- examples/factorial.x --trace
```
