namespace Language

open System

module Parser =

    let private tryParseInt (s: string) =
        match Int32.TryParse(s) with
        | true, v -> Some v
        | _ -> None

    let private parseAtom (token: string) : Result<Expr, ParseError> =
        match token with
        | "true" -> Ok (EBool true)
        | "false" -> Ok (EBool false)
        | _ ->
            match tryParseInt token with
            | Some n -> Ok (ENumber n)
            | None ->
                if String.IsNullOrWhiteSpace token then
                    Error (InvalidSyntax "Empty input")
                else
                    Ok (ESymbol token)

    let parse (source: string) : Result<Expr, ParseError> =
        let trimmed = source.Trim()

        if String.IsNullOrWhiteSpace trimmed then
            Error (InvalidSyntax "Input is empty")
        else
            parseAtom trimmed