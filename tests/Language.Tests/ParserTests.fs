namespace Language.Tests

open Xunit
open Language

module ParserTests =

    [<Fact>]
    let ``parse number`` () =
        let result = Parser.parse "42"
        match result with
        | Ok (ENumber 42) -> ()
        | _ -> failwith "Expected ENumber 42"

    [<Fact>]
    let ``parse true`` () =
        let result = Parser.parse "true"
        match result with
        | Ok (EBool true) -> ()
        | _ -> failwith "Expected EBool true"

    [<Fact>]
    let ``parse false`` () =
        let result = Parser.parse "false"
        match result with
        | Ok (EBool false) -> ()
        | _ -> failwith "Expected EBool false"

    [<Fact>]
    let ``parse symbol`` () =
        let result = Parser.parse "x"
        match result with
        | Ok (ESymbol "x") -> ()
        | _ -> failwith "Expected ESymbol x"

    [<Fact>]
    let ``empty input returns error`` () =
        let result = Parser.parse ""
        match result with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``parse simple application`` () =
        let result = Parser.parse "(+ 1 2)"

        match result with
        | Ok (EApply(ESymbol "+", [ENumber 1; ENumber 2])) -> ()
        | _ -> failwith $"Unexpected result: {result}"

    [<Fact>]
    let ``parse if`` () =
        match Parser.parse "(if true 1 0)" with
        | Ok (EIf(EBool true, ENumber 1, ENumber 0)) -> ()
        | _ -> failwith "if parse failed"

    [<Fact>]
    let ``parse let`` () =
        match Parser.parse "(let x 10 x)" with
        | Ok (ELet("x", ENumber 10, ESymbol "x")) -> ()
        | _ -> failwith "let parse failed"

    [<Fact>]
    let ``parse lambda`` () =
        match Parser.parse "(lambda (x) x)" with
        | Ok (ELambda(["x"], ESymbol "x")) -> ()
        | _ -> failwith "lambda parse failed"
