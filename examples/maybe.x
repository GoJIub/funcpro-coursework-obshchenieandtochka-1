(let double
  (lambda (x) (* x 2))
  (fmap double (just 5)))