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