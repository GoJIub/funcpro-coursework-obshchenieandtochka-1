namespace Language

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

    let validateParam (s: string) =
        match Int32.TryParse(s) with
        | true, _ -> failwith "Invalid parameter name"
        | _ ->
            match s with
            | "true" | "false" -> failwith "Invalid parameter name"
            | _ -> s

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

        | List [Atom "if"; cond; thenExpr; elseExpr] ->
            EIf(toExpr cond, toExpr thenExpr, toExpr elseExpr)

        | List (Atom "if" :: _) ->
            failwith "Invalid if syntax"

        | List [Atom "and"; left; right] ->
            EIf(toExpr left, toExpr right, EBool false)

        | List (Atom "and" :: _) ->
            failwith "Invalid and syntax"

        | List [Atom "or"; left; right] ->
            EIf(toExpr left, EBool true, toExpr right)

        | List (Atom "or" :: _) ->
            failwith "Invalid or syntax"

        | List [Atom "let"; Atom _; Atom "="; _] ->
            failwith "Invalid let syntax"

        | List [Atom "let"; Atom name; Atom "="; valueExpr; body] ->
            ELet(name, toExpr valueExpr, toExpr body)

        | List [Atom "let"; Atom name; valueExpr; body] ->
            ELet(name, toExpr valueExpr, toExpr body)

        | List (Atom "let" :: _) ->
            failwith "Invalid let syntax"

        | List [Atom "letrec"; Atom name; valueExpr; body] ->
            ELetRec(name, toExpr valueExpr, toExpr body)

        | List (Atom "letrec" :: _) ->
            failwith "Invalid letrec syntax"

        | List [Atom "lambda"; List parameters; body] ->
            let paramNames =
                parameters
                |> List.map (function
                    | Atom s -> validateParam s
                    | _ -> failwith "Invalid parameter")
            ELambda(paramNames, toExpr body)

        | List [Atom param; Atom "=>"; body] ->
            ELambda([validateParam param], toExpr body)

        | List [List parameters; Atom "=>"; body] ->
            let paramNames =
                parameters
                |> List.map (function
                    | Atom s -> validateParam s
                    | _ -> failwith "Invalid parameter")
            ELambda(paramNames, toExpr body)

        | List (Atom _ :: Atom "=>" :: _) ->
            failwith "Invalid lambda sugar syntax"

        | List (List _ :: Atom "=>" :: _) ->
            failwith "Invalid lambda sugar syntax"

        | List (Atom "lambda" :: _) ->
            failwith "Invalid lambda syntax"

        | List [Atom "delay"; expr] ->
            EDelay (toExpr expr)

        | List (Atom "delay" :: _) ->
            failwith "Invalid delay syntax"

        | List [Atom "force"; expr] ->
            EForce (toExpr expr)

        | List (Atom "force" :: _) ->
            failwith "Invalid force syntax"

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
