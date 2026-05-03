module Language.Tests.EvaluatorTests

open Language
open Xunit

[<Fact>]
let ``evaluator evaluates number literal`` () =
    match Evaluator.eval Environment.empty (ENumber 10) with
    | Ok(VNumber 10) -> Assert.True(true)
    | result -> Assert.Fail($"Expected number 10, got {result}")

[<Fact>]
let ``evaluator evaluates bool literal`` () =
    match Evaluator.eval Environment.empty (EBool true) with
    | Ok(VBool true) -> Assert.True(true)
    | result -> Assert.Fail($"Expected bool true, got {result}")

[<Fact>]
let ``evaluator looks up symbol in environment`` () =
    let env = Environment.empty |> Environment.extend "x" (VNumber 42)

    match Evaluator.eval env (ESymbol "x") with
    | Ok(VNumber 42) -> Assert.True(true)
    | result -> Assert.Fail($"Expected number 42, got {result}")

[<Fact>]
let ``evaluator returns unbound variable for missing symbol`` () =
    match Evaluator.eval Environment.empty (ESymbol "missing") with
    | Error(UnboundVariable "missing") -> Assert.True(true)
    | result -> Assert.Fail($"Expected unbound variable, got {result}")

[<Fact>]
let ``evaluator evaluates selected if branch`` () =
    let expr = EIf(EBool true, ENumber 1, ESymbol "missing")

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 1) -> Assert.True(true)
    | result -> Assert.Fail($"Expected selected branch result, got {result}")

[<Fact>]
let ``evaluator rejects non-bool if condition`` () =
    let expr = EIf(ENumber 1, ENumber 2, ENumber 3)

    match Evaluator.eval Environment.empty expr with
    | Error(TypeMismatch("Bool", _)) -> Assert.True(true)
    | result -> Assert.Fail($"Expected bool type mismatch, got {result}")

[<Fact>]
let ``evaluator evaluates let body in extended environment`` () =
    let expr = ELet("x", ENumber 10, ESymbol "x")

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 10) -> Assert.True(true)
    | result -> Assert.Fail($"Expected let-bound number 10, got {result}")

[<Fact>]
let ``evaluator propagates let value error`` () =
    let expr = ELet("x", ESymbol "missing", ESymbol "x")

    match Evaluator.eval Environment.empty expr with
    | Error(UnboundVariable "missing") -> Assert.True(true)
    | result -> Assert.Fail($"Expected let value error, got {result}")

[<Fact>]
let ``evaluator evaluates lambda to closure with lexical environment`` () =
    let env = Environment.empty |> Environment.extend "x" (VNumber 10)
    let expr = ELambda([ "y" ], ESymbol "x")

    match Evaluator.eval env expr with
    | Ok(VClosure([ "y" ], ESymbol "x", closureEnv)) ->
        match Environment.lookup "x" closureEnv with
        | Ok(VNumber 10) -> Assert.True(true)
        | result -> Assert.Fail($"Expected captured x = 10, got {result}")
    | result -> Assert.Fail($"Expected closure, got {result}")

[<Fact>]
let ``evaluator applies closure to evaluated argument`` () =
    let expr = EApply(ELambda([ "x" ], ESymbol "x"), [ ENumber 5 ])

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 5) -> Assert.True(true)
    | result -> Assert.Fail($"Expected applied argument value, got {result}")

[<Fact>]
let ``evaluator applies closure with lexical scoping`` () =
    let expr =
        ELet(
            "x",
            ENumber 10,
            ELet(
                "f",
                ELambda([ "ignored" ], ESymbol "x"),
                ELet("x", ENumber 20, EApply(ESymbol "f", [ ENumber 0 ]))
            )
        )

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 10) -> Assert.True(true)
    | result -> Assert.Fail($"Expected captured x = 10, got {result}")

[<Fact>]
let ``evaluator evaluates arguments before applying closure`` () =
    let expr = EApply(ELambda([ "x" ], ENumber 1), [ ESymbol "missing" ])

    match Evaluator.eval Environment.empty expr with
    | Error(UnboundVariable "missing") -> Assert.True(true)
    | result -> Assert.Fail($"Expected argument evaluation error, got {result}")

[<Fact>]
let ``evaluator rejects wrong closure argument count`` () =
    let expr = EApply(ELambda([ "x"; "y" ], ESymbol "x"), [ ENumber 1 ])

    match Evaluator.eval Environment.empty expr with
    | Error(WrongArgumentCount(2, 1)) -> Assert.True(true)
    | result -> Assert.Fail($"Expected wrong argument count, got {result}")

[<Fact>]
let ``evaluator rejects applying non-function`` () =
    let expr = EApply(ENumber 1, [])

    match Evaluator.eval Environment.empty expr with
    | Error(NotAFunction "Number") -> Assert.True(true)
    | result -> Assert.Fail($"Expected NotAFunction, got {result}")

[<Fact>]
let ``evaluator closure parameter shadows captured variable`` () =
    let expr =
        ELet(
            "x",
            ENumber 10,
            EApply(ELambda([ "x" ], ESymbol "x"), [ ENumber 20 ])
        )

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 20) -> Assert.True(true)
    | result -> Assert.Fail($"Expected parameter x = 20, got {result}")

[<Fact>]
let ``evaluator closure local let shadows captured variable`` () =
    let expr =
        ELet(
            "x",
            ENumber 10,
            ELet(
                "f",
                ELambda([ "ignored" ], ELet("x", ENumber 30, ESymbol "x")),
                ELet("x", ENumber 20, EApply(ESymbol "f", [ ENumber 0 ]))
            )
        )

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 30) -> Assert.True(true)
    | result -> Assert.Fail($"Expected local let x = 30, got {result}")

[<Fact>]
let ``evaluator nested closure captures call environment`` () =
    let expr =
        ELet(
            "make",
            ELambda([ "x" ], ELambda([ "ignored" ], ESymbol "x")),
            ELet(
                "f",
                EApply(ESymbol "make", [ ENumber 7 ]),
                ELet("x", ENumber 9, EApply(ESymbol "f", [ ENumber 0 ]))
            )
        )

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 7) -> Assert.True(true)
    | result -> Assert.Fail($"Expected nested closure captured x = 7, got {result}")

[<Fact>]
let ``evaluator letrec binds function to its own name`` () =
    let expr =
        ELetRec(
            "self",
            ELambda([ "x" ], ESymbol "self"),
            EApply(ESymbol "self", [ ENumber 1 ])
        )

    match Evaluator.eval Environment.empty expr with
    | Ok(VClosure([ "x" ], ESymbol "self", closureEnv)) ->
        match Environment.lookup "self" closureEnv with
        | Ok(VClosure _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected recursive self binding, got {result}")
    | _ -> Assert.Fail("Expected recursive closure")

[<Fact>]
let ``evaluator evaluates recursive factorial through letrec`` () =
    let env = Builtins.makeBuiltins Evaluator.eval

    let expr =
        ELetRec(
            "factorial",
            ELambda(
                [ "n" ],
                EIf(
                    EApply(ESymbol "=", [ ESymbol "n"; ENumber 0 ]),
                    ENumber 1,
                    EApply(
                        ESymbol "*",
                        [ ESymbol "n"
                          EApply(
                              ESymbol "factorial",
                              [ EApply(ESymbol "-", [ ESymbol "n"; ENumber 1 ]) ]
                          ) ]
                    )
                )
            ),
            EApply(ESymbol "factorial", [ ENumber 5 ])
        )

    match Evaluator.eval env expr with
    | Ok(VNumber 120) -> Assert.True(true)
    | result -> Assert.Fail($"Expected factorial 5 = 120, got {result}")

[<Fact>]
let ``evaluator rejects non-lambda letrec value`` () =
    let expr = ELetRec("x", ENumber 1, ESymbol "x")

    match Evaluator.eval Environment.empty expr with
    | Error(OtherEvalError _) -> Assert.True(true)
    | result -> Assert.Fail($"Expected letrec error, got {result}")

[<Fact>]
let ``evaluator rejects letrec function call with too many arguments`` () =
    let expr =
        ELetRec(
            "f",
            ELambda([ "x" ], ESymbol "x"),
            EApply(ESymbol "f", [ ENumber 1; ENumber 2 ])
        )

    match Evaluator.eval Environment.empty expr with
    | Error(WrongArgumentCount(1, 2)) -> Assert.True(true)
    | result -> Assert.Fail($"Expected letrec wrong argument count, got {result}")

[<Fact>]
let ``evaluator rejects letrec function call with too few arguments`` () =
    let expr =
        ELetRec(
            "f",
            ELambda([ "x" ], ESymbol "x"),
            EApply(ESymbol "f", [])
        )

    match Evaluator.eval Environment.empty expr with
    | Error(WrongArgumentCount(1, 0)) -> Assert.True(true)
    | result -> Assert.Fail($"Expected letrec wrong argument count, got {result}")

[<Fact>]
let ``evaluator propagates recursive function argument count error`` () =
    let expr =
        ELetRec(
            "f",
            ELambda([ "x" ], EApply(ESymbol "f", [ ESymbol "x"; ENumber 1 ])),
            EApply(ESymbol "f", [ ENumber 0 ])
        )

    match Evaluator.eval Environment.empty expr with
    | Error(WrongArgumentCount(1, 2)) -> Assert.True(true)
    | result -> Assert.Fail($"Expected recursive wrong argument count, got {result}")

[<Fact>]
let ``evaluator delay returns thunk without evaluating expression`` () =
    match Evaluator.eval Environment.empty (EDelay(ESymbol "missing")) with
    | Ok(VThunk thunk) ->
        Assert.Equal(ESymbol "missing", thunk.Expression)
        Assert.Equal(None, thunk.CachedValue)
    | result -> Assert.Fail($"Expected thunk, got {result}")

[<Fact>]
let ``evaluator force evaluates thunk expression`` () =
    let expr = EForce(EDelay(ENumber 42))

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 42) -> Assert.True(true)
    | result -> Assert.Fail($"Expected forced value 42, got {result}")

[<Fact>]
let ``evaluator force uses thunk creation environment`` () =
    let expr =
        ELet(
            "x",
            ENumber 10,
            ELet(
                "delayed",
                EDelay(ESymbol "x"),
                ELet("x", ENumber 20, EForce(ESymbol "delayed"))
            )
        )

    match Evaluator.eval Environment.empty expr with
    | Ok(VNumber 10) -> Assert.True(true)
    | result -> Assert.Fail($"Expected forced captured x = 10, got {result}")

[<Fact>]
let ``evaluator force stores evaluated value in thunk cache`` () =
    let thunk =
        { Expression = ESymbol "x"
          Environment = Environment.empty |> Environment.extend "x" (VNumber 1)
          CachedValue = None }

    let env =
        Environment.empty
        |> Environment.extend "t" (VThunk thunk)

    match Evaluator.eval env (EForce(ESymbol "t")) with
    | Ok(VNumber 1) -> Assert.True(true)
    | result -> Assert.Fail($"Expected first force value 1, got {result}")

    match thunk.CachedValue with
    | Some(VNumber 1) -> Assert.True(true)
    | cached -> Assert.Fail($"Expected cached value 1, got {cached}")

[<Fact>]
let ``evaluator force returns cached value without evaluating expression`` () =
    let thunk =
        { Expression = ESymbol "missing"
          Environment = Environment.empty
          CachedValue = Some(VNumber 99) }

    let env =
        Environment.empty
        |> Environment.extend "t" (VThunk thunk)

    match Evaluator.eval env (EForce(ESymbol "t")) with
    | Ok(VNumber 99) -> Assert.True(true)
    | result -> Assert.Fail($"Expected cached force value 99, got {result}")

[<Fact>]
let ``evaluator force rejects non-thunk value`` () =
    match Evaluator.eval Environment.empty (EForce(ENumber 1)) with
    | Error(TypeMismatch("Thunk", "Number")) -> Assert.True(true)
    | result -> Assert.Fail($"Expected thunk type mismatch, got {result}")
