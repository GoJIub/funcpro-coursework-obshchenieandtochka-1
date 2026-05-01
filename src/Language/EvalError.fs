namespace Language

type ParseError =
    | InvalidSyntax of message: string
    | UnexpectedToken of token: string
    | UnexpectedEndOfInput

type EvalError =
    | UnboundVariable of name: string
    | TypeMismatch of expected: string * actual: string
    | NotAFunction of actual: string
    | WrongArgumentCount of expected: int * actual: int
    | DivisionByZero
    | OtherEvalError of message: string
