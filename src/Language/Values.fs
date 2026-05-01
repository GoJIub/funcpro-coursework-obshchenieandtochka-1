namespace Language

type Env = Map<string, Value>

and Thunk =
    { Expression: Expr
      Environment: Env }

and BuiltinFunc = Value list -> Result<Value, EvalError>

and Value =
    | VNumber of int
    | VBool of bool
    | VList of Value list
    | VClosure of parameters: string list * body: Expr * env: Env
    | VBuiltin of name: string * implementation: BuiltinFunc
    | VMaybe of Value option
    | VThunk of Thunk
