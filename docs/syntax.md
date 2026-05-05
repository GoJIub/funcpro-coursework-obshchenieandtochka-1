# Syntax

Язык использует S-expression синтаксис. Все составные формы записываются в
скобках. Файл `.x` должен содержать одно top-level выражение.

---

## Literals

### Numbers

```
1
42
-7
```

### Booleans

```
true
false
```

Строковых literals в текущей версии языка нет.

---

## Variables

```
x
my-var
```

---

## If

```
(if condition then-expr else-expr)
```

Пример:

```
(if true 1 2)
(if (= x 0) 100 200)
```

---

## Logical Forms

`not` вычисляет один boolean-аргумент и возвращает противоположное значение.

```
(not true)
```

`and` и `or` принимают два аргумента и вычисляются с short-circuit семантикой:
второй аргумент вычисляется только если он нужен для результата.

```
(and left right)
(or left right)
```

Примеры:

```
(not false)
(and false (/ 1 0))
(or true (/ 1 0))
```

---

## Cond

Многоветочное условие. Каждая ветка записывается как `(condition result)`.
Последняя ветка должна быть `(true result)`, она используется как fallback.
Парсер разворачивает `cond` в цепочку вложенных `if`.

```
(cond
  (condition1 result1)
  (condition2 result2)
  (true fallback-result))
```

Пример:

```
(cond
  ((= x 0) 0)
  ((< x 0) -1)
  (true 1))
```

---

## Let

Связывает имя со значением в теле выражения.

**Классический синтаксис:**

```
(let name value body)
```

**Sugar-синтаксис:**

```
(let name = value body)
```

Оба варианта эквивалентны. Пример:

```
(let x 10 (+ x 1))
(let x = 10 (+ x 1))
```

---

## Let*

Последовательные привязки. Каждая следующая binding expression вычисляется в
окружении, где уже доступны предыдущие привязки. Парсер разворачивает `let*` в
цепочку вложенных `let`.

```
(let* ((name1 value1) (name2 value2))
  body)
```

Пример:

```
(let* ((x 1) (y 2) (z (+ x y)))
  (+ (+ x y) z))
```

`let*` с пустым списком bindings возвращает body:

```
(let* () (+ 1 2))
```

---

## Letrec

Рекурсивное связывание — имя видно внутри самого `value`. Используется для определения рекурсивных функций.

```
(letrec name value body)
```

Пример:

```
(letrec fact
  (lambda (n) (if (= n 0) 1 (* n (fact (- n 1)))))
  (fact 5))
```

---

## Lambda

Анонимная функция.

**Классический синтаксис:**

```
(lambda (param1 param2 ...) body)
```

**Sugar-синтаксис — один параметр:**

```
(param => body)
```

**Sugar-синтаксис — несколько параметров:**

```
((param1 param2 ...) => body)
```

Примеры:

```
(lambda (x) (* x x))
(x => (* x x))
((x y) => (+ x y))
```

---

## Function Application

Вызов функции — первый элемент списка, остальные — аргументы.

```
(f arg1 arg2 ...)
```

Примеры:

```
(+ 1 2)
(factorial 10)
(map (x => (* x 2)) my-list)
```

---

## Lists

```
(list expr1 expr2 ...)
```

Пример:

```
(list 1 2 3)
(list true false true)
```

Пустой список создаётся как `(list)`. Синтаксис `()` не является выражением и
возвращает parse error.

---

## Maybe

`Maybe` создаётся builtins `just` и `nothing` и обрабатывается через `fmap` /
`bind`.

```
(just expr)
(nothing)
(fmap function maybe)
(bind maybe function)
```

Пример:

```
(fmap (x => (* x 2)) (just 5))
```

---

## Delay / Force

Ленивые вычисления. `delay` откладывает вычисление выражения, `force` запускает его.

```
(delay expr)
(force expr)
```

Пример:

```
(let t (delay (+ 1 2))
  (force t))
```

---

## Parse Errors

Парсер возвращает понятные ошибки при некорректном синтаксе:

| Ситуация | Ошибка |
|---|---|
| Пустой ввод | `Empty input` |
| Незакрытая скобка | `Missing ')'` |
| Лишняя закрывающая скобка | `Unexpected ')'` |
| Лишние токены после выражения | `Unexpected tokens after expression` |
| Неверное число аргументов у `if` | `Invalid if syntax` |
| Неверное число аргументов у `and` | `Invalid and syntax` |
| Неверное число аргументов у `or` | `Invalid or syntax` |
| Неверное число аргументов у `let` | `Invalid let syntax` |
| Неверный синтаксис `let*` | `Invalid let* syntax` / `Invalid let* binding syntax` |
| Неверное число аргументов у `letrec` | `Invalid letrec syntax` |
| Неверный синтаксис `cond` | `Invalid cond syntax` / `Invalid cond clause syntax` |
| Неверное число аргументов у `lambda` | `Invalid lambda syntax` |
| Неверный параметр lambda (число или bool) | `Invalid parameter name` |
| Неверный синтаксис `=>` | `Invalid lambda sugar syntax` |
| Неверное число аргументов у `delay` | `Invalid delay syntax` |
| Неверное число аргументов у `force` | `Invalid force syntax` |
| Пустой список `()` | `Empty list` |

## Ограничения синтаксиса

- Комментарии не поддерживаются.
- Строки не поддерживаются.
- Несколько top-level выражений в одном файле не поддерживаются.
