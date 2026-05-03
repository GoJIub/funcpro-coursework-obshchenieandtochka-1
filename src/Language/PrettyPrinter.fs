namespace Language

module PrettyPrinter =

    let rec printExpr expr =
        match expr with
        | ENumber n -> string n
        | EBool b -> if b then "true" else "false"
        | ESymbol s -> s
        | EIf (c, t, e) ->
            $"(if {printExpr c} {printExpr t} {printExpr e})"
        | ELet (name, value, body) ->
            $"(let {name} {printExpr value} {printExpr body})"
        | ELetRec (name, value, body) ->
            $"(letrec {name} {printExpr value} {printExpr body})"
        | ELambda (args, body) ->
            let argsStr = String.concat " " args
            $"(lambda ({argsStr}) {printExpr body})"
        | EApply (f, args) ->
            let argsStr =
                args
                |> List.map printExpr
                |> String.concat " "
            $"({printExpr f} {argsStr})"
        | EDelay e ->
            $"(delay {printExpr e})"
        | EForce e ->
            $"(force {printExpr e})"
        | EList items ->
            let itemsStr =
                items
                |> List.map printExpr
                |> String.concat " "
            $"(list {itemsStr})"


    let rec printValue value =
        match value with
        | VNumber n -> string n
        | VBool b -> if b then "true" else "false"

        | VList items ->
            let itemsStr =
                items
                |> List.map printValue
                |> String.concat "; "
            $"[{itemsStr}]"

        | VClosure _ ->
            "<closure>"

        | VBuiltin (name, _) ->
            $"<builtin:{name}>"

        | VMaybe None ->
            "Nothing"

        | VMaybe (Some v) ->
            $"Just {printValue v}"

        | VThunk _ ->
            "<thunk>"