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

    let rec toExpr sexpr =
        match sexpr with
        // ===== ATOMS =====
        | Atom s ->
            match Int32.TryParse(s) with
            | true, n -> ENumber n
            | _ ->
                match s with
                | "true" -> EBool true
                | "false" -> EBool false
                | _ -> ESymbol s

        // ===== IF =====
        | List [Atom "if"; cond; thenExpr; elseExpr] ->
            EIf(toExpr cond, toExpr thenExpr, toExpr elseExpr)

        | List (Atom "if" :: _) ->
            failwith "Invalid if syntax"

        // ===== LET (старый синтаксис) =====
        | List [Atom "let"; Atom name; valueExpr; body] ->
            ELet(name, toExpr valueExpr, toExpr body)

        // ===== LET (с сахаром: let x = expr body) =====
        | List [Atom "let"; Atom name; Atom "="; valueExpr; body] ->
            ELet(name, toExpr valueExpr, toExpr body)

        | List (Atom "let" :: _) ->
            failwith "Invalid let syntax"

        // ===== LETREC =====
        | List [Atom "letrec"; Atom name; valueExpr; body] ->
            ELetRec(name, toExpr valueExpr, toExpr body)

        | List (Atom "letrec" :: _) ->
            failwith "Invalid letrec syntax"

        // ===== LAMBDA (старый синтаксис) =====
        | List [Atom "lambda"; List parameters; body] ->
            let paramNames =
                parameters
                |> List.map (function
                    | Atom s ->
                        match Int32.TryParse(s) with
                        | true, _ -> failwith "Invalid parameter"
                        | _ ->
                            match s with
                            | "true" | "false" -> failwith "Invalid parameter"
                            | _ -> s
                    | _ -> failwith "Invalid parameter")
            ELambda(paramNames, toExpr body)

        // ===== LAMBDA (сахар: (x => body)) =====
        | List [Atom param; Atom "=>"; body] ->
            ELambda([param], toExpr body)

        // ===== LAMBDA (сахар: ((x y) => body)) =====
        | List [List parameters; Atom "=>"; body] ->
            let paramNames =
                parameters
                |> List.map (function
                    | Atom s -> s
                    | _ -> failwith "Invalid parameter")
            ELambda(paramNames, toExpr body)

        | List (Atom "lambda" :: _) ->
            failwith "Invalid lambda syntax"

        // ===== FUNCTION APPLICATION =====
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