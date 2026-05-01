namespace Language

module Parser =
    let parse (_source: string) : Result<Expr, ParseError> =
        Error(InvalidSyntax "Parser is not implemented yet.")
