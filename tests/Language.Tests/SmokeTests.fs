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
