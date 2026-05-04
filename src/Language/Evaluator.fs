namespace Language

open PrettyPrinter
open Trace

module Evaluator =

    let rec private evalArguments env arguments =
        match arguments with
        | [] -> Ok []
        | argument :: rest ->
            match eval env argument with
            | Ok value ->
                match evalArguments env rest with
                | Ok values -> Ok (value :: values)
                | Error error -> Error error
            | Error error -> Error error

    and private apply callee arguments =
        let label =
            sprintf "apply %s [%s]"
                (printValue callee)
                (arguments |> List.map printValue |> String.concat "; ")

        enter label

        let result =
            match callee with
            | VClosure(parameters, body, closureEnv) ->
                if List.length parameters <> List.length arguments then
                    Error (WrongArgumentCount(List.length parameters, List.length arguments))
                else
                    let callEnv =
                        Environment.extendMany (List.zip parameters arguments) closureEnv
                    eval callEnv body

            | VBuiltin(_, implementation) ->
                implementation arguments

            | other ->
                Error (NotAFunction(ValueFormatting.valueTypeName other))

        match result with
        | Ok value ->
            exitWithResult value
            Ok value
        | Error e ->
            exitWithError (sprintf "%A" e)
            Error e

    and private forceThunk thunk =
        enter "force thunk"

        let result =
            match thunk.CachedValue with
            | Some value -> Ok value
            | None ->
                match eval thunk.Environment thunk.Expression with
                | Ok value ->
                    thunk.CachedValue <- Some value
                    Ok value
                | Error error -> Error error

        match result with
        | Ok value ->
            exitWithResult value
            Ok value
        | Error e ->
            exitWithError (sprintf "%A" e)
            Error e

    and eval (env: Env) (expr: Expr) : Result<Value, EvalError> =
        let label = sprintf "eval %s" (printExpr expr)
        enter label

        let result =
            match expr with
            | ENumber number ->
                Ok (VNumber number)

            | EBool value ->
                Ok (VBool value)

            | ESymbol name ->
                Environment.lookup name env

            | EIf(condition, thenBranch, elseBranch) ->
                match eval env condition with
                | Ok (VBool true) -> eval env thenBranch
                | Ok (VBool false) -> eval env elseBranch
                | Ok actual ->
                    Error (TypeMismatch("Bool", ValueFormatting.valueTypeName actual))
                | Error error -> Error error

            | ELet(name, valueExpr, body) ->
                match eval env valueExpr with
                | Ok value ->
                    let newEnv = Environment.extend name value env
                    eval newEnv body
                | Error error -> Error error

            | ELetRec(name, valueExpr, body) ->
                match valueExpr with
                | ELambda(parameters, lambdaBody) ->
                    let rec recursiveValue =
                        VClosure(parameters, lambdaBody, recursiveEnv)

                    and recursiveEnv =
                        Map.add name recursiveValue env

                    eval recursiveEnv body

                | _ ->
                    Error (OtherEvalError "letrec requires lambda value expression.")

            | ELambda(parameters, body) ->
                Ok (VClosure(parameters, body, env))

            | EDelay delayedExpr ->
                Ok (
                    VThunk
                        { Expression = delayedExpr
                          Environment = env
                          CachedValue = None }
                )

            | EForce thunkExpr ->
                match eval env thunkExpr with
                | Ok (VThunk thunk) -> forceThunk thunk
                | Ok other ->
                    Error (TypeMismatch("Thunk", ValueFormatting.valueTypeName other))
                | Error error -> Error error

            | EApply(calleeExpr, argumentExprs) ->
                match eval env calleeExpr with
                | Ok callee ->
                    match evalArguments env argumentExprs with
                    | Ok arguments -> apply callee arguments
                    | Error error -> Error error
                | Error error -> Error error

            | EList items ->
                match evalArguments env items with
                | Ok values -> Ok (VList values)
                | Error error -> Error error

        match result with
        | Ok value ->
            exitWithResult value
            Ok value
        | Error e ->
            exitWithError (sprintf "%A" e)
            Error e
