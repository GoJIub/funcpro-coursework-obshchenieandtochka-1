module Language.Tests.ExampleTests

open Language
open System.IO
open Xunit

let private builtins = Builtins.makeBuiltins Evaluator.eval

let private runFile (filename: string) =
    let path =
        Path.Combine(
            __SOURCE_DIRECTORY__,
            "..", "..", "examples", filename)
    let source = File.ReadAllText(path)
    match Parser.parse source with
    | Error e -> Error(sprintf "Parse error: %A" e)
    | Ok expr ->
        match Evaluator.eval builtins expr with
        | Ok value -> Ok value
        | Error e -> Error(sprintf "Eval error: %A" e)

[<Fact>]
let ``factorial returns 120`` () =
    match runFile "factorial.x" with
    | Ok(VNumber 120) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected 120, got {message}")
    | result -> Assert.Fail(sprintf "Expected 120, got %A" result)

[<Fact>]
let ``fibonacci returns 55`` () =
    match runFile "fibonacci.x" with
    | Ok(VNumber 55) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected 55, got {message}")
    | result -> Assert.Fail(sprintf "Expected 55, got %A" result)

[<Fact>]
let ``closure returns 15`` () =
    match runFile "closure.x" with
    | Ok(VNumber 15) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected 15, got {message}")
    | result -> Assert.Fail(sprintf "Expected 15, got %A" result)

[<Fact>]
let ``sum-list returns 15`` () =
    match runFile "sum-list.x" with
    | Ok(VNumber 15) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected 15, got {message}")
    | result -> Assert.Fail(sprintf "Expected 15, got %A" result)

[<Fact>]
let ``map returns squared list`` () =
    match runFile "map.x" with
    | Ok(VList [ VNumber 1; VNumber 4; VNumber 9; VNumber 16; VNumber 25 ]) ->
        Assert.True(true)
    | Error message -> Assert.Fail($"Expected squared list, got {message}")
    | result -> Assert.Fail(sprintf "Expected squared list, got %A" result)

[<Fact>]
let ``filter returns elements greater than 2`` () =
    match runFile "filter.x" with
    | Ok(VList [ VNumber 3; VNumber 4; VNumber 5 ]) ->
        Assert.True(true)
    | Error message -> Assert.Fail($"Expected (list 3 4 5), got {message}")
    | result -> Assert.Fail(sprintf "Expected (list 3 4 5), got %A" result)

[<Fact>]
let ``fold returns product 120`` () =
    match runFile "fold.x" with
    | Ok(VNumber 120) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected 120, got {message}")
    | result -> Assert.Fail(sprintf "Expected 120, got %A" result)

[<Fact>]
let ``maybe returns just 10`` () =
    match runFile "maybe.x" with
    | Ok(VMaybe(Some(VNumber 10))) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected just 10, got {message}")
    | result -> Assert.Fail(sprintf "Expected just 10, got %A" result)

[<Fact>]
let ``lazy returns 3`` () =
    match runFile "lazy.x" with
    | Ok(VNumber 3) -> Assert.True(true)
    | Error message -> Assert.Fail($"Expected 3, got {message}")
    | result -> Assert.Fail(sprintf "Expected 3, got %A" result)
