; lazy.x
; Демонстрация явной ленивости через delay/force
; Ожидаемый результат: 3

(let x (delay (+ 1 2))
  (force x))