namespace Language

module Environment =
    let empty : Env = Map.empty

    let lookup (name: string) (env: Env) : Result<Value, EvalError> =
        match Map.tryFind name env with
        | Some value -> Ok value
        | None -> Error(UnboundVariable name)

    let extend (name: string) (value: Value) (env: Env) : Env =
        Map.add name value env

    let extendMany (bindings: (string * Value) list) (env: Env) : Env =
        bindings
        |> List.fold (fun currentEnv (name, value) -> extend name value currentEnv) env
