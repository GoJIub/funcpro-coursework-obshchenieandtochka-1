namespace Language

module Builtins =

    let private expectNumber (value: Value) : Result<int, EvalError> =
        match value with
        | VNumber n -> Ok n
        | other ->
            let typeName =
                match other with
                | VBool _ -> "Bool"
                | VList _ -> "List"
                | VClosure _ -> "Closure"
                | VBuiltin _ -> "Builtin"
                | VMaybe _ -> "Maybe"
                | VThunk _ -> "Thunk"
                | VNumber _ -> "Number"
            Error(TypeMismatch("Number", typeName))

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

    let builtins : Map<string, Value> =
        [ "+", VBuiltin("+", arithmeticOp (+))
          "-", VBuiltin("-", arithmeticOp (-))
          "*", VBuiltin("*", arithmeticOp (*))
          "/", VBuiltin("/", div)
          "=", VBuiltin("=", eq)
          "<", VBuiltin("<", compareOp (<))
          ">", VBuiltin(">", compareOp (>)) ]
        |> Map.ofList
