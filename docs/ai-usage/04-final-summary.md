# Final AI Usage Summary

Этот документ кратко summarise-ит использование генеративного ИИ в проекте
LispNT и связывает индивидуальные логи участников.

Подробные артефакты:

- [00-policy.md](00-policy.md) — политика использования ИИ;
- [01-log-participant-1.md](01-log-participant-1.md) — лог Участника 1;
- [01-log-participant-2.md](01-log-participant-2.md) — лог Участника 2;
- [01-log-participant-3.md](01-log-participant-3.md) — лог Участника 3;
- [02-team-decisions.md](02-team-decisions.md) — командные решения.

## Использованные инструменты

В проекте использовались:

- OpenAI Codex;
- ChatGPT;
- Claude;
- GitHub Copilot, если он был доступен участнику.

ИИ использовался как инженерный помощник. Он не заменял авторство участников и
не был скрытым источником готового проекта.

## Для каких задач использовался ИИ

ИИ применялся в нескольких типах задач:

- планирование issue и pull request;
- декомпозиция этапов работы;
- объяснение функциональных концепций: lexical closures, `letrec`, explicit
  laziness, `Maybe`, parser sugar;
- генерация черновиков документации;
- подбор и уточнение тест-кейсов;
- review pull request и поиск несоответствий между кодом, примерами и docs;
- помощь с локальными git/GitHub workflow;
- финальная проверка документации на актуальность.

## Вклад по зонам

### Участник 1

Участник 1 использовал Codex для semantic core и quality/documentation work:

- проектный skeleton и language contract;
- evaluator для literals, variables, `if`, `let`;
- lambda application и lexical closures;
- `letrec` и recursion;
- core-поддержка `delay` / `force`;
- evaluator support для `EList`;
- временная реализация parser sugar `let*`, `cond`, `and`, `or`, когда это было
  нужно для финального этапа;
- review PR других участников;
- README, documentation index, architecture docs и language reference.

Подробности зафиксированы в
[01-log-participant-1.md](01-log-participant-1.md).

### Участник 2

Участник 2 использовал ИИ для parser, syntax и CLI задач:

- parser для atoms и S-expressions;
- special forms: `if`, `let`, `letrec`, `lambda`;
- syntax sugar;
- CLI запуск `.x` файлов;
- флаги `--ast` и `--trace`;
- pretty-printing values;
- UX CLI, syntax docs и parser tests.

Подробности зафиксированы в
[01-log-participant-2.md](01-log-participant-2.md).

### Участник 3

Участник 3 использовал ИИ для standard library, examples и части документации:

- builtins;
- списки и функции высшего порядка;
- `Maybe`, `fmap`, `bind`;
- документация `delay` / `force`;
- runnable examples и tests для examples;
- дополнительные examples для sugar и laziness.

Подробности зафиксированы в
[01-log-participant-3.md](01-log-participant-3.md).

## Командные решения

Отдельно логировались решения, которые влияли на архитектуру и организацию:

- вести отдельные AI usage logs по участникам;
- использовать Lisp-like syntax с небольшим sugar;
- писать parser вручную без FParsec;
- разделить tests по зонам ответственности;
- передавать `eval` в `makeBuiltins`, чтобы higher-order builtins могли
  применять closures;
- вынести runtime type formatting в общий helper;
- реализовать `delay` / `force` как special forms в AST/evaluator.

См. [02-team-decisions.md](02-team-decisions.md).

## Human Review

Все AI-generated code и documentation changes проходили человеческую проверку:

- изменения оформлялись через issues и pull requests;
- pull requests ревьюились участниками команды;
- спорные решения обсуждались вручную;
- код проверялся тестами и CI;
- документация сверялась с фактической реализацией.

ИИ помогал ускорить подготовку черновиков, тестовых сценариев и ревью, но
финальные решения принимались участниками проекта.

## Ограничения AI-помощи

Команда не принимала AI output как истину без проверки. Особенно проверялись:

- соответствие документации текущему parser/evaluator;
- корректность runnable examples;
- отсутствие несуществующих возможностей в docs;
- корректность ошибок и edge cases;
- отсутствие скрытого изменения публичных контрактов.

Например, в ходе review были исправлены случаи, когда документация или examples
описывали возможности, которых parser ещё не поддерживал.

## Итог

Использование ИИ было прозрачным, зафиксированным в markdown-логах и
ограниченным рамками инженерной помощи. Проект, код и документация остаются
результатом командной работы с ручной проверкой каждого принятого изменения.
