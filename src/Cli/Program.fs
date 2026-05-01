open Language

[<EntryPoint>]
let main _args =
    printfn "Functional language CLI skeleton"
    printfn "Parser contract: string -> Result<Expr, ParseError>"
    printfn "Evaluator contract: Env -> Expr -> Result<Value, EvalError>"
    0
