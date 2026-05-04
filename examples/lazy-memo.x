(let counter (delay (+ 1 2))
  (let a (force counter)
    (let b (force counter)
      b)))