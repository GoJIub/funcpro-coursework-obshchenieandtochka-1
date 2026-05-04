# Syntax

Язык использует S-expression синтаксис. Все составные формы записываются в скобках.

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
(if (= x 0) "zero" "nonzero")
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
| Неверное число аргументов у `let` | `Invalid let syntax` |
| Неверное число аргументов у `letrec` | `Invalid letrec syntax` |
| Неверное число аргументов у `lambda` | `Invalid lambda syntax` |
| Неверный параметр lambda (число или bool) | `Invalid parameter name` |
| Неверный синтаксис `=>` | `Invalid lambda sugar syntax` |
| Неверное число аргументов у `delay`/`force` | `Invalid delay/force syntax` |
| Пустой список `()` | `Empty list` |