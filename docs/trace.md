# Trace

Режим трассировки показывает пошаговое вычисление программы: каждый вход в `eval`, каждый вызов функции и каждый результат — с отступами по глубине вложенности.

---

## Использование

```bash
dotnet run --project src/Cli -- <file.x> --trace
```

Флаг `--trace` можно комбинировать с `--ast`:

```bash
dotnet run --project src/Cli -- <file.x> --ast --trace
```

---

## Формат вывода

Каждая строка начинается с отступа, соответствующего глубине вычисления.

```
→ <событие>       -- вход в вычисление
← result <value>  -- успешный выход с результатом
← ERROR: <msg>    -- выход с ошибкой
```

Три типа событий:

| Событие | Цвет | Когда |
|---|---|---|
| `eval <expr>` | синий | начало вычисления любого выражения |
| `apply <fn> [<args>]` | жёлтый | вызов функции или builtin |
| `force thunk` | синий | вычисление отложенного выражения |

Результаты всегда зелёного цвета.

---

## Пример: `(+ 1 2)`

```
→ eval (+ 1 2)
  → eval +
  ← result <builtin:+>
  → eval 1
  ← result 1
  → eval 2
  ← result 2
  → apply <builtin:+> [1; 2]
  ← result 3
← result 3
```

---

## Пример: факториал

Программа (`examples/factorial.x`):

```
(letrec fact
  (lambda (n) (if (= n 0) 1 (* n (fact (- n 1)))))
  (fact 3))
```

Трассировка:

```
→ eval (letrec fact (lambda (n) (if (= n 0) 1 (* n (fact (- n 1))))) (fact 3))
  → eval (fact 3)
    → eval fact
    ← result <closure>
    → eval 3
    ← result 3
    → apply <closure> [3]
      → eval (if (= n 0) 1 (* n (fact (- n 1))))
        → eval (= n 0)
          → apply <builtin:=> [3; 0]
          ← result false
        ← result false
        → eval (* n (fact (- n 1)))
          → eval n
          ← result 3
          → eval (fact (- n 1))
            → apply <closure> [2]
              ...
            ← result 2
          → apply <builtin:*> [3; 2]
          ← result 6
        ← result 6
      ← result 6
    ← result 6
  ← result 6
← result 6
```

---

## Пример: `delay` / `force`

```
→ eval (let t (delay (+ 1 2)) (force t))
  → eval (delay (+ 1 2))
  ← result <thunk>
  → eval (force t)
    → eval t
    ← result <thunk>
    → force thunk
      → eval (+ 1 2)
        → apply <builtin:+> [1; 2]
        ← result 3
      ← result 3
    ← result 3
  ← result 3
← result 3
```

Повторный `force` того же thunk возвращает кешированное значение без повторного вычисления:

```
→ force thunk
← result 3        -- из кеша, eval не вызывается
```

---

## Пример: ошибка

```
→ eval (/ 1 0)
  → apply <builtin:/> [1; 0]
  ← ERROR: DivisionByZero
← ERROR: DivisionByZero
```

---

## Цветовая схема

| Цвет | ANSI-код | Значение |
|---|---|---|
| Синий | `\u001b[34m` | `eval` — начало вычисления |
| Жёлтый | `\u001b[33m` | `apply` — вызов функции |
| Зелёный | `\u001b[32m` | `result` — успешный результат |

Цвета отображаются в терминалах с поддержкой ANSI (Linux, macOS, Windows Terminal).