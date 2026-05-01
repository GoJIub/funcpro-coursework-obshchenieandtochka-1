namespace Language

module Evaluator =
    let private valueTypeName value =
        match value with
        | VNumber _ -> "Number"
        | VBool _ -> "Bool"
        | VList _ -> "List"
        | VClosure _ -> "Closure"
        | VBuiltin _ -> "Builtin"
        | VMaybe _ -> "Maybe"
        | VThunk _ -> "Thunk"

    let rec eval (env: Env) (expr: Expr) : Result<Value, EvalError> =
        match expr with
        | ENumber number -> Ok(VNumber number)
        | EBool value -> Ok(VBool value)
        | ESymbol name -> Environment.lookup name env
        | EIf(condition, thenBranch, elseBranch) ->
            match eval env condition with
            | Ok(VBool true) -> eval env thenBranch
            | Ok(VBool false) -> eval env elseBranch
            | Ok actual -> Error(TypeMismatch("Bool", valueTypeName actual))
            | Error error -> Error error
        | ELet(name, valueExpr, body) ->
            match eval env valueExpr with
            | Ok value -> eval (Environment.extend name value env) body
            | Error error -> Error error
        | ELetRec _ -> Error(OtherEvalError "letrec evaluation is not implemented yet.")
        | ELambda _ -> Error(OtherEvalError "lambda evaluation is not implemented yet.")
        | EApply _ -> Error(OtherEvalError "application evaluation is not implemented yet.")
        | EList _ -> Error(OtherEvalError "list evaluation is not implemented yet.")
