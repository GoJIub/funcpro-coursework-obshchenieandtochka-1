namespace Language

module PrettyPrinter =

    let rec printValue (value: Value) : string =
        match value with
        | VNumber n -> string n
        | VBool true -> "true"
        | VBool false -> "false"

        | VList items ->
            let inner =
                items
                |> List.map printValue
                |> String.concat " "
            $"(list {inner})"

        | VMaybe None -> "nothing"

        | VMaybe (Some v) ->
            $"(just {printValue v})"

        | VClosure _ -> "<closure>"

        | VBuiltin (name, _) ->
            $"<builtin:{name}>"

        | VThunk _ -> "<thunk>"