namespace Language

module Evaluator =
    let rec private evalArguments env arguments =
        match arguments with
        | [] -> Ok []
        | argument :: rest ->
            match eval env argument with
            | Ok value ->
                match evalArguments env rest with
                | Ok values -> Ok(value :: values)
                | Error error -> Error error
            | Error error -> Error error

    and private apply callee arguments =
        match callee with
        | VClosure(parameters, body, closureEnv) ->
            if List.length parameters <> List.length arguments then
                Error(WrongArgumentCount(List.length parameters, List.length arguments))
            else
                let callEnv = Environment.extendMany (List.zip parameters arguments) closureEnv
                eval callEnv body
        | VBuiltin(_, implementation) -> implementation arguments
        | other -> Error(NotAFunction(ValueFormatting.valueTypeName other))

    and eval (env: Env) (expr: Expr) : Result<Value, EvalError> =
        match expr with
        | ENumber number -> Ok(VNumber number)
        | EBool value -> Ok(VBool value)
        | ESymbol name -> Environment.lookup name env
        | EIf(condition, thenBranch, elseBranch) ->
            match eval env condition with
            | Ok(VBool true) -> eval env thenBranch
            | Ok(VBool false) -> eval env elseBranch
            | Ok actual -> Error(TypeMismatch("Bool", ValueFormatting.valueTypeName actual))
            | Error error -> Error error
        | ELet(name, valueExpr, body) ->
            match eval env valueExpr with
            | Ok value -> eval (Environment.extend name value env) body
            | Error error -> Error error
        | ELetRec _ -> Error(OtherEvalError "letrec evaluation is not implemented yet.")
        | ELambda(parameters, body) -> Ok(VClosure(parameters, body, env))
        | EApply(calleeExpr, argumentExprs) ->
            match eval env calleeExpr with
            | Ok callee ->
                match evalArguments env argumentExprs with
                | Ok arguments -> apply callee arguments
                | Error error -> Error error
            | Error error -> Error error
        | EList _ -> Error(OtherEvalError "list evaluation is not implemented yet.")
