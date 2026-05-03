(let x (delay (+ 1 2))
  (force x))