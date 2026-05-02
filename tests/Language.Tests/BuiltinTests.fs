module Language.Tests.BuiltinTests

open Language
open Xunit

[<Fact>]
let ``builtin add returns sum`` () =
    let add = Builtins.builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VNumber 7) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 7, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects first non-number`` () =
    let add = Builtins.builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VBool true; VNumber 1 ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects second non-number`` () =
    let add = Builtins.builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VBool true ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects single arg`` () =
    let add = Builtins.builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1 ] with
        | Error(WrongArgumentCount(2, 1)) -> Assert.True(true)
        | result -> Assert.Fail($"Expected WrongArgumentCount, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects more than 2 args`` () =
    let add = Builtins.builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4; VNumber 5 ] with
        | Error(WrongArgumentCount _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected WrongArgumentCount, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin sub returns differnece`` () =
    let sub = Builtins.builtins |> Map.find "-"
    match sub with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VNumber -1) -> Assert.True(true)
        | result -> Assert.Fail($"Expected -1, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin mult returns composition`` () =
    let mult = Builtins.builtins |> Map.find "*"
    match mult with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VNumber 12) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 12, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin div returns ratio`` () =
    let div = Builtins.builtins |> Map.find "/"
    match div with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 10; VNumber 5 ] with
        | Ok(VNumber 2) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 2, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin div raises DivisionByZero`` () =
    let div = Builtins.builtins |> Map.find "/"
    match div with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 10; VNumber 0 ] with
        | Error DivisionByZero -> Assert.True(true)
        | result -> Assert.Fail($"Expected DivisionByZero, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns true to numbers`` () =
    let eq = Builtins.builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 3 ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns false to numbers`` () =
    let eq = Builtins.builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns true to bool`` () =
    let eq = Builtins.builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VBool true; VBool true ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns false to bool`` () =
    let eq = Builtins.builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VBool true; VBool false ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq rejects mismatched types`` () =
    let eq = Builtins.builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VBool true ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare more returns true`` () =
    let compare = Builtins.builtins |> Map.find ">"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 4; VNumber 3 ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare more returns false`` () =
    let compare = Builtins.builtins |> Map.find ">"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare less returns true`` () =
    let compare = Builtins.builtins |> Map.find "<"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare less returns false`` () =
    let compare = Builtins.builtins |> Map.find "<"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 4; VNumber 3 ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")
