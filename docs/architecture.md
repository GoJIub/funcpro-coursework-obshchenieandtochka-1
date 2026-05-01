# Architecture

> ⚠️ Этот файл будет дополнен после реализации skeleton (Участник 1).

## Общая схема

```text
исходный код .x
    ↓
Parser (src/Language/Parser.fs)
    ↓
AST (src/Language/Ast.fs)
    ↓
Evaluator (src/Language/Evaluator.fs)
    ↓
Value (src/Language/Values.fs)
```

## Модули

| Файл | Ответственный | Описание |
|---|---|---|
| Ast.fs | Участник 1 | Тип Expr |
| Values.fs | Участник 1 | Тип Value |
| Environment.fs | Участник 1 | Окружение |
| EvalError.fs | Участник 1 | Ошибки вычисления |
| Evaluator.fs | Участник 1 | eval |
| Parser.fs | Участник 2 | parse |
| PrettyPrinter.fs | Участник 2 | Вывод AST |
| Builtins.fs | Участник 3 | Встроенные функции |
| Trace.fs | Участник 3 | Трассировщик |
| Program.fs | Участник 2 | CLI |