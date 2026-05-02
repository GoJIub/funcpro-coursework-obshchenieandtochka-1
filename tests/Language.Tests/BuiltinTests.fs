module Language.Tests.BuiltinTests

open Language
open Xunit

let private builtins = Builtins.makeBuiltins Evaluator.eval

// ─── operators ──────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin add returns sum`` () =
    let add = builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VNumber 7) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 7, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects first non-number`` () =
    let add = builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VBool true; VNumber 1 ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects second non-number`` () =
    let add = builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VBool true ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects single arg`` () =
    let add = builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1 ] with
        | Error(WrongArgumentCount(2, 1)) -> Assert.True(true)
        | result -> Assert.Fail($"Expected WrongArgumentCount, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin add rejects more than 2 args`` () =
    let add = builtins |> Map.find "+"
    match add with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4; VNumber 5 ] with
        | Error(WrongArgumentCount _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected WrongArgumentCount, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin sub returns differnece`` () =
    let sub = builtins |> Map.find "-"
    match sub with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VNumber -1) -> Assert.True(true)
        | result -> Assert.Fail($"Expected -1, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin mult returns composition`` () =
    let mult = builtins |> Map.find "*"
    match mult with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VNumber 12) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 12, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin div returns ratio`` () =
    let div = builtins |> Map.find "/"
    match div with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 10; VNumber 5 ] with
        | Ok(VNumber 2) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 2, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin div raises DivisionByZero`` () =
    let div = builtins |> Map.find "/"
    match div with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 10; VNumber 0 ] with
        | Error DivisionByZero -> Assert.True(true)
        | result -> Assert.Fail($"Expected DivisionByZero, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns true to numbers`` () =
    let eq = builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 3 ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns false to numbers`` () =
    let eq = builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns true to bool`` () =
    let eq = builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VBool true; VBool true ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq returns false to bool`` () =
    let eq = builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VBool true; VBool false ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin eq rejects mismatched types`` () =
    let eq = builtins |> Map.find "="
    match eq with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VBool true ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare more returns true`` () =
    let compare = builtins |> Map.find ">"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 4; VNumber 3 ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare more returns false`` () =
    let compare = builtins |> Map.find ">"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare less returns true`` () =
    let compare = builtins |> Map.find "<"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 3; VNumber 4 ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin compare less returns false`` () =
    let compare = builtins |> Map.find "<"
    match compare with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 4; VNumber 3 ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── list ───────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin list creates list from args`` () =
    let list = builtins |> Map.find "list"
    match list with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VNumber 2; VNumber 3 ] with
        | Ok(VList [ VNumber 1; VNumber 2; VNumber 3 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin list creates empty list`` () =
    let list = builtins |> Map.find "list"
    match list with
    | VBuiltin(_, impl) ->
        match impl [] with
        | Ok(VList []) -> Assert.True(true)
        | result -> Assert.Fail($"Expected empty list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── head ───────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin head returns first element`` () =
    let head = builtins |> Map.find "head"
    match head with
    | VBuiltin(_, impl) ->
        match impl [ VList [ VNumber 1; VNumber 2; VNumber 3 ] ] with
        | Ok(VNumber 1) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 1, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin head rejects empty list`` () =
    let head = builtins |> Map.find "head"
    match head with
    | VBuiltin(_, impl) ->
        match impl [ VList [] ] with
        | Error(OtherEvalError _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected error, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin head rejects non-list`` () =
    let head = builtins |> Map.find "head"
    match head with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1 ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── tail ────────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin tail returns rest of list`` () =
    let tail = builtins |> Map.find "tail"
    match tail with
    | VBuiltin(_, impl) ->
        match impl [ VList [ VNumber 1; VNumber 2; VNumber 3 ] ] with
        | Ok(VList [ VNumber 2; VNumber 3 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected tail, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin tail of single element returns empty list`` () =
    let tail = builtins |> Map.find "tail"
    match tail with
    | VBuiltin(_, impl) ->
        match impl [ VList [ VNumber 1 ] ] with
        | Ok(VList []) -> Assert.True(true)
        | result -> Assert.Fail($"Expected empty list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin tail rejects empty list`` () =
    let tail = builtins |> Map.find "tail"
    match tail with
    | VBuiltin(_, impl) ->
        match impl [ VList [] ] with
        | Error(OtherEvalError _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected error, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin tail rejects non-list`` () =
    let tail = builtins |> Map.find "tail"
    match tail with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1 ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── cons ───────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin cons prepends element to list`` () =
    let cons = builtins |> Map.find "cons"
    match cons with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VList [ VNumber 2; VNumber 3 ] ] with
        | Ok(VList [ VNumber 1; VNumber 2; VNumber 3 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin cons prepends to empty list`` () =
    let cons = builtins |> Map.find "cons"
    match cons with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VList [] ] with
        | Ok(VList [ VNumber 1 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected singleton list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin cons rejects non-list second arg`` () =
    let cons = builtins |> Map.find "cons"
    match cons with
    | VBuiltin(_, impl) ->
        match impl [ VNumber 1; VNumber 2 ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── empty? ─────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin empty? returns true for empty list`` () =
    let isEmpty = builtins |> Map.find "empty?"
    match isEmpty with
    | VBuiltin(_, impl) ->
        match impl [ VList [] ] with
        | Ok(VBool true) -> Assert.True(true)
        | result -> Assert.Fail($"Expected true, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin empty? returns false for non-empty list`` () =
    let isEmpty = builtins |> Map.find "empty?"
    match isEmpty with
    | VBuiltin(_, impl) ->
        match impl [ VList [ VNumber 1 ] ] with
        | Ok(VBool false) -> Assert.True(true)
        | result -> Assert.Fail($"Expected false, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── map ────────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin map applies builtin to list`` () =
    let map = builtins |> Map.find "map"
    let double = VBuiltin("double", fun args ->
        match args with
        | [ VNumber n ] -> Ok(VNumber(n * 2))
        | _ -> Error(WrongArgumentCount(1, List.length args)))
    match map with
    | VBuiltin(_, impl) ->
        match impl [ double; VList [ VNumber 1; VNumber 2; VNumber 3 ] ] with
        | Ok(VList [ VNumber 2; VNumber 4; VNumber 6 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected doubled list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin map applies closure to list`` () =
    let map = builtins |> Map.find "map"
    let double = VClosure(
        [ "x" ],
        EApply(ESymbol "*", [ ESymbol "x"; ENumber 2 ]),
        builtins)
    match map with
    | VBuiltin(_, impl) ->
        match impl [ double; VList [ VNumber 1; VNumber 2; VNumber 3 ] ] with
        | Ok(VList [ VNumber 2; VNumber 4; VNumber 6 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected doubled list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin map over empty list returns empty list`` () =
    let map = builtins |> Map.find "map"
    let double = VBuiltin("double", fun args ->
        match args with
        | [ VNumber n ] -> Ok(VNumber(n * 2))
        | _ -> Error(WrongArgumentCount(1, List.length args)))
    match map with
    | VBuiltin(_, impl) ->
        match impl [ double; VList [] ] with
        | Ok(VList []) -> Assert.True(true)
        | result -> Assert.Fail($"Expected empty list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── filter ─────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin filter keeps matching elements`` () =
    let filter = builtins |> Map.find "filter"
    let isEven = VBuiltin("isEven", fun args ->
        match args with
        | [ VNumber n ] -> Ok(VBool(n % 2 = 0))
        | _ -> Error(WrongArgumentCount(1, List.length args)))
    match filter with
    | VBuiltin(_, impl) ->
        match impl [ isEven; VList [ VNumber 1; VNumber 2; VNumber 3; VNumber 4 ] ] with
        | Ok(VList [ VNumber 2; VNumber 4 ]) -> Assert.True(true)
        | result -> Assert.Fail($"Expected filtered list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin filter over empty list returns empty list`` () =
    let filter = builtins |> Map.find "filter"
    let isEven = VBuiltin("isEven", fun args ->
        match args with
        | [ VNumber n ] -> Ok(VBool(n % 2 = 0))
        | _ -> Error(WrongArgumentCount(1, List.length args)))
    match filter with
    | VBuiltin(_, impl) ->
        match impl [ isEven; VList [] ] with
        | Ok(VList []) -> Assert.True(true)
        | result -> Assert.Fail($"Expected empty list, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin filter rejects non-bool predicate result`` () =
    let filter = builtins |> Map.find "filter"
    let returnsNumber = VBuiltin("bad", fun args ->
        match args with
        | [ _ ] -> Ok(VNumber 1)
        | _ -> Error(WrongArgumentCount(1, List.length args)))
    match filter with
    | VBuiltin(_, impl) ->
        match impl [ returnsNumber; VList [ VNumber 1 ] ] with
        | Error(TypeMismatch _) -> Assert.True(true)
        | result -> Assert.Fail($"Expected TypeMismatch, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

// ─── fold ───────────────────────────────────────────────────────────────────

[<Fact>]
let ``builtin fold accumulates result`` () =
    let fold = builtins |> Map.find "fold"
    let add = VBuiltin("add", fun args ->
        match args with
        | [ VNumber a; VNumber b ] -> Ok(VNumber(a + b))
        | _ -> Error(WrongArgumentCount(2, List.length args)))
    match fold with
    | VBuiltin(_, impl) ->
        match impl [ add; VNumber 0; VList [ VNumber 1; VNumber 2; VNumber 3 ] ] with
        | Ok(VNumber 6) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 6, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin fold over empty list returns init`` () =
    let fold = builtins |> Map.find "fold"
    let add = VBuiltin("add", fun args ->
        match args with
        | [ VNumber a; VNumber b ] -> Ok(VNumber(a + b))
        | _ -> Error(WrongArgumentCount(2, List.length args)))
    match fold with
    | VBuiltin(_, impl) ->
        match impl [ add; VNumber 0; VList [] ] with
        | Ok(VNumber 0) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 0, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")

[<Fact>]
let ``builtin fold applies closure`` () =
    let fold = builtins |> Map.find "fold"
    let add = VClosure(
        [ "acc"; "x" ],
        EApply(ESymbol "+", [ ESymbol "acc"; ESymbol "x" ]),
        builtins)
    match fold with
    | VBuiltin(_, impl) ->
        match impl [ add; VNumber 0; VList [ VNumber 1; VNumber 2; VNumber 3 ] ] with
        | Ok(VNumber 6) -> Assert.True(true)
        | result -> Assert.Fail($"Expected 6, got {result}")
    | _ -> Assert.Fail("Expected VBuiltin")
