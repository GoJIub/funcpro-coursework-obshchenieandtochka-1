namespace Language

type Expr =
    | ENumber of int
    | EBool of bool
    | ESymbol of string
    | EIf of condition: Expr * thenBranch: Expr * elseBranch: Expr
    | ELet of name: string * valueExpr: Expr * body: Expr
    | ELetRec of name: string * valueExpr: Expr * body: Expr
    | ELambda of parameters: string list * body: Expr
    | EApply of callee: Expr * arguments: Expr list
    | EList of Expr list
