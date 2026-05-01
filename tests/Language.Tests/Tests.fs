module Language.Tests.SmokeTests

open Language
open Xunit

[<Fact>]
let ``environment lookup returns bound value`` () =
    let env = Environment.empty |> Environment.extend "x" (VNumber 10)

    match Environment.lookup "x" env with
    | Ok(VNumber 10) -> Assert.True(true)
    | result -> Assert.Fail($"Expected bound number 10, got {result}")

[<Fact>]
let ``parser exposes initial contract`` () =
    match Parser.parse "(+ 1 2)" with
    | Error(InvalidSyntax _) -> Assert.True(true)
    | result -> Assert.Fail($"Expected parser stub error, got {result}")

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
