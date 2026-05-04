(let x 10
  (let t (delay x)
    (let x 99
      (force t))))