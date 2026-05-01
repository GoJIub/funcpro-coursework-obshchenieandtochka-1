namespace Language

module Evaluator =
    let eval (_env: Env) (_expr: Expr) : Result<Value, EvalError> =
        Error(OtherEvalError "Evaluator is not implemented yet.")
