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
