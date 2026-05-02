namespace Language

open System

module Parser =

    open System

    type SExpr =
        | Atom of string
        | List of SExpr list

    let tokenize (input: string) =
        input
            .Replace("(", " ( ")
            .Replace(")", " ) ")
            .Split([|' '; '\n'; '\t'|], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

    let rec parseSExpr tokens =
        match tokens with
        | [] -> failwith "Unexpected end of input"

        | "(" :: rest ->
            let rec parseList acc tokens =
                match tokens with
                | [] -> failwith "Missing ')'"
                | ")" :: rest -> (List (List.rev acc), rest)
                | _ ->
                    let (expr, rest') = parseSExpr tokens
                    parseList (expr :: acc) rest'

            parseList [] rest

        | ")" :: _ ->
            failwith "Unexpected ')'" 

        | atom :: rest ->
            (Atom atom, rest)

    let rec toExpr sexpr =
        match sexpr with
        | Atom s ->
            match Int32.TryParse(s) with
            | true, n -> ENumber n
            | _ ->
                match s with
                | "true" -> EBool true
                | "false" -> EBool false
                | _ -> ESymbol s

        | List (head :: tail) ->
            let callee = toExpr head
            let args = tail |> List.map toExpr
            EApply(callee, args)

        | List [] ->
            failwith "Empty list"

    let parse (source: string) : Result<Expr, ParseError> =
        try
            let tokens = tokenize source

            match tokens with
            | [] -> Error (InvalidSyntax "Empty input")
            | _ ->
                let (sexpr, rest) = parseSExpr tokens

                if rest <> [] then
                    Error (InvalidSyntax "Unexpected tokens after expression")
                else
                    Ok (toExpr sexpr)

        with
        | ex -> Error (InvalidSyntax ex.Message)
