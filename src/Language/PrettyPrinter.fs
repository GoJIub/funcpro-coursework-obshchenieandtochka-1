namespace Language

module PrettyPrinter =

    let rec printExpr expr =
        match expr with
        | ENumber n -> string n
        | EBool b -> if b then "true" else "false"
        | ESymbol s -> s
        | ELambda(args, body) ->
            sprintf "(lambda (%s) %s)" (String.concat " " args) (printExpr body)
        | EIf(c, t, e) ->
            sprintf "(if %s %s %s)" (printExpr c) (printExpr t) (printExpr e)
        | ELet(name, value, body) ->
            sprintf "(let %s %s %s)" name (printExpr value) (printExpr body)
        | EApply(f, args) ->
            let argsStr = args |> List.map printExpr |> String.concat " "
            sprintf "(%s %s)" (printExpr f) argsStr
        | _ -> "<expr>"

    let rec printValue value =
        match value with
        | VNumber n -> string n
        | VBool b -> if b then "true" else "false"
        | VList items ->
            items
            |> List.map printValue
            |> String.concat " "
            |> sprintf "(list %s)"
        | VClosure _ -> "<closure>"
        | VBuiltin(name, _) -> sprintf "<builtin:%s>" name
        | VMaybe None -> "nothing"
        | VMaybe(Some v) -> sprintf "(just %s)" (printValue v)
        | VThunk _ -> "<thunk>"