# LispNT Documentation

Эта папка содержит canonical markdown-документацию курсового проекта:
**LispNT** (Lisp&Tochka), небольшого Lisp-like функционального языка на F#.

## Быстрая навигация

| Документ | Назначение |
|---|---|
| [syntax.md](syntax.md) | пользовательский синтаксис языка |
| [language-spec.md](language-spec.md) | семантика, AST, runtime values и ошибки |
| [standard-library.md](standard-library.md) | встроенные функции и формы стандартной библиотеки |
| [architecture.md](architecture.md) | архитектура реализации и основные модули |
| [trace.md](trace.md) | режим трассировки вычислений |
| [ai-usage/](ai-usage/) | политика, индивидуальные логи и командные решения по AI usage |

## Что реализовано

Язык поддерживает:

- числа и булевы значения;
- переменные и `let`;
- условные выражения `if`, `cond`;
- функции, `lambda` и `=>` sugar;
- лексические замыкания;
- рекурсию через `letrec`;
- списки и функции высшего порядка;
- `Maybe`, `fmap`, `bind`;
- явную ленивость через `delay` / `force`;
- short-circuit формы `and` / `or`;
- трассировку вычислений через CLI-флаг `--trace`.

## Запуск

Из корня репозитория:

Рекомендуемая среда: .NET SDK 10.x.

```bash
dotnet run --project src/Cli -- examples/factorial.x
```

Показать AST:

```bash
dotnet run --project src/Cli -- examples/factorial.x --ast
```

Показать trace:

```bash
dotnet run --project src/Cli -- examples/factorial.x --trace
```

Запустить тесты:

```bash
dotnet test FuncProCoursework.sln --configuration Release
```

Если локальный runtime новее целевого `net8.0`:

```bash
DOTNET_ROLL_FORWARD=Major dotnet test FuncProCoursework.sln --configuration Release
```

## Примеры

Основные runnable examples лежат в [../examples](../examples):

- `factorial.x`: рекурсия через `letrec`;
- `fibonacci.x`: рекурсивное ветвление;
- `closure.x`: лексические замыкания;
- `sum-list.x`: свёртка списка;
- `map.x`, `filter.x`, `fold.x`: функции высшего порядка;
- `maybe.x`: `Maybe` и `fmap`;
- `lazy.x`, `lazy-memo.x`, `lazy-scope.x`, `lazy-error.x`: явная ленивость;
- `arrow-lambda.x`, `let-sugar.x`, `let-star.x`, `cond.x`,
  `logical-forms.x`: синтаксический сахар.

Подробная страница с примерами будет добавлена в `docs/examples.md` в рамках
отдельной задачи.

## GitHub Pages

GitHub Pages недоступен в classroom repository. Markdown-документация в этой
папке остаётся canonical-версией. Если документация будет опубликована как
внешняя HTML-страница, ссылка будет добавлена в README и сюда.

## AI Usage

Проект использовал генеративный ИИ открыто и с логированием. Основные файлы:

- [ai-usage/00-policy.md](ai-usage/00-policy.md);
- [ai-usage/01-log-participant-1.md](ai-usage/01-log-participant-1.md);
- [ai-usage/01-log-participant-2.md](ai-usage/01-log-participant-2.md);
- [ai-usage/01-log-participant-3.md](ai-usage/01-log-participant-3.md);
- [ai-usage/02-team-decisions.md](ai-usage/02-team-decisions.md);
- [ai-usage/04-final-summary.md](ai-usage/04-final-summary.md).

Итоговое summary связывает индивидуальные логи, командные решения и процесс
human review AI-assisted изменений.
