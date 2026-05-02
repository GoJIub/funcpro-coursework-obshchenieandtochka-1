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
let ``parser parses simple application`` () =
    let result = Parser.parse "(+ 1 2)"

    match result with
    | Ok (EApply(ESymbol "+", [ENumber 1; ENumber 2])) -> Assert.True(true)
    | result -> Assert.Fail($"Unexpected result: {result}")