namespace Language

module Builtins =

    let private expectNumber (value: Value) : Result<int, EvalError> =
        match value with
        | VNumber n -> Ok n
        | other -> Error(TypeMismatch("Number", ValueFormatting.valueTypeName other))

    let private arithmeticOp
        (op: int -> int -> int)
        (args: Value list)
        : Result<Value, EvalError> =
        match args with
        | [ a; b ] ->
            match expectNumber a, expectNumber b with
            | Ok x, Ok y -> Ok(VNumber(op x y))
            | Error e, _ -> Error e
            | _, Error e -> Error e
        | _ -> Error(WrongArgumentCount(2, List.length args))

    let private div (args: Value list) : Result<Value, EvalError> =
        match args with
        | [ a; b ] ->
            match expectNumber a, expectNumber b with
            | Ok _, Ok 0 -> Error DivisionByZero
            | Ok x, Ok y -> Ok(VNumber(x / y))
            | Error e, _ -> Error e
            | _, Error e -> Error e
        | _ -> Error(WrongArgumentCount(2, List.length args))

    let private compareOp
        (op: int -> int -> bool)
        (args: Value list)
        : Result<Value, EvalError> =
        match args with
        | [ a; b ] ->
            match expectNumber a, expectNumber b with
            | Ok x, Ok y -> Ok(VBool(op x y))
            | Error e, _ -> Error e
            | _, Error e -> Error e
        | _ -> Error(WrongArgumentCount(2, List.length args))

    let private eq (args: Value list) : Result<Value, EvalError> =
        match args with
        | [ VNumber a; VNumber b ] -> Ok(VBool(a = b))
        | [ VBool a; VBool b ] -> Ok(VBool(a = b))
        | [ _; _ ] -> Error(TypeMismatch("matching types", "mismatched types"))
        | _ -> Error(WrongArgumentCount(2, List.length args))

    let makeBuiltins (eval: Env -> Expr -> Result<Value, EvalError>) : Map<string, Value> =

        let applyFunc (f: Value) (args: Value list) : Result<Value, EvalError> =
            match f with
            | VBuiltin(_, impl) -> impl args
            | VClosure(parameters, body, closureEnv) ->
                if List.length parameters <> List.length args then
                    Error(WrongArgumentCount(List.length parameters, List.length args))
                else
                    let callEnv = Environment.extendMany (List.zip parameters args) closureEnv
                    eval callEnv body
            | other -> Error(NotAFunction(ValueFormatting.valueTypeName other))

        let expectList (value: Value) : Result<Value list, EvalError> =
            match value with
            | VList items -> Ok items
            | other -> Error(TypeMismatch("List", ValueFormatting.valueTypeName other))

        let makeList (args: Value list) : Result<Value, EvalError> =
            Ok(VList args)

        let head (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ arg ] ->
                match expectList arg with
                | Ok [] -> Error(OtherEvalError "head: empty list")
                | Ok(first :: _) -> Ok first
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(1, List.length args))

        let tail (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ arg ] ->
                match expectList arg with
                | Ok [] -> Error(OtherEvalError "tail: empty list")
                | Ok(_ :: rest) -> Ok(VList rest)
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(1, List.length args))

        let cons (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ item; list ] ->
                match expectList list with
                | Ok items -> Ok(VList(item :: items))
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(2, List.length args))

        let isEmpty (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ arg ] ->
                match expectList arg with
                | Ok [] -> Ok(VBool true)
                | Ok _ -> Ok(VBool false)
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(1, List.length args))

        let mapList (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ f; list ] ->
                match expectList list with
                | Ok items ->
                    items
                    |> List.map (fun item -> applyFunc f [ item ])
                    |> List.fold (fun acc result ->
                        match acc, result with
                        | Ok xs, Ok x -> Ok(xs @ [ x ])
                        | Error e, _ -> Error e
                        | _, Error e -> Error e) (Ok [])
                    |> Result.map VList
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(2, List.length args))

        let filterList (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ f; list ] ->
                match expectList list with
                | Ok items ->
                    items
                    |> List.fold (fun acc item ->
                        match acc with
                        | Error e -> Error e
                        | Ok xs ->
                            match applyFunc f [ item ] with
                            | Ok(VBool true) -> Ok(xs @ [ item ])
                            | Ok(VBool false) -> Ok xs
                            | Ok other -> Error(TypeMismatch("Bool", ValueFormatting.valueTypeName other))
                            | Error e -> Error e) (Ok [])
                    |> Result.map VList
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(2, List.length args))

        let foldList (args: Value list) : Result<Value, EvalError> =
            match args with
            | [ f; init; list ] ->
                match expectList list with
                | Ok items ->
                    items
                    |> List.fold (fun acc item ->
                        match acc with
                        | Ok accVal -> applyFunc f [ accVal; item ]
                        | Error e -> Error e) (Ok init)
                | Error e -> Error e
            | _ -> Error(WrongArgumentCount(3, List.length args))

        [ "+",      VBuiltin("+", arithmeticOp (+))
          "-",      VBuiltin("-", arithmeticOp (-))
          "*",      VBuiltin("*", arithmeticOp (*))
          "/",      VBuiltin("/", div)
          "=",      VBuiltin("=", eq)
          "<",      VBuiltin("<", compareOp (<))
          ">",      VBuiltin(">", compareOp (>))
          "list",   VBuiltin("list", makeList)
          "head",   VBuiltin("head", head)
          "tail",   VBuiltin("tail", tail)
          "cons",   VBuiltin("cons", cons)
          "empty?", VBuiltin("empty?", isEmpty)
          "map",    VBuiltin("map", mapList)
          "filter", VBuiltin("filter", filterList)
          "fold",   VBuiltin("fold", foldList) ]
        |> Map.ofList
