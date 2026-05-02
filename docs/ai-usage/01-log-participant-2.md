# AI Prompts Log

Лог всех обращений к ИИ в рамках проекта.

---

**Дата:** 02.05.2026  
**Задача:** parser: parse atoms — numbers, bools, symbols  

**Prompt / темы обращений:**
1. Реализация базового parser для атомов.
2. Обработка чисел, bool и символов.
3. Добавление unit-тестов.
4. Обновление smoke-теста после реализации parser.

**Ответ ИИ / краткое содержание:**
- Предложена реализация parseAtom.
- Добавлена обработка пустого ввода.
- Добавлены ParserTests.fs.
- Обнаружен падающий smoke test, связанный с заглушкой parser.
- Предложено обновить smoke test под текущее поведение parser.

**Что принято:**
- Parser реализован поэтапно: сначала atoms.
- Ошибки возвращаются через Result.
- Smoke test обновляется по мере развития parser.

**Что изменено человеком:**
- Исправлен SmokeTests.fs.
- Проверена локальная сборка и запуск тестов.
- Добавлены тесты ParserTests.fs.

**Связанные файлы:**
- src/Language/Parser.fs
- tests/Language.Tests/ParserTests.fs
- tests/Language.Tests/SmokeTests.fs

**Связанный PR:** pending

---
