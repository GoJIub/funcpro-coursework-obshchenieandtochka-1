(let make-adder (x => (y => (+ x y)))
  ((make-adder 10) 5))