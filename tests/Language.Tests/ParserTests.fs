namespace Language.Tests

open Xunit
open Language

module ParserTests =

    // ── Atoms ────────────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse number`` () =
        match Parser.parse "42" with
        | Ok (ENumber 42) -> ()
        | _ -> failwith "Expected ENumber 42"

    [<Fact>]
    let ``parse negative number`` () =
        match Parser.parse "-7" with
        | Ok (ENumber -7) -> ()
        | _ -> failwith "Expected ENumber -7"

    [<Fact>]
    let ``parse zero`` () =
        match Parser.parse "0" with
        | Ok (ENumber 0) -> ()
        | _ -> failwith "Expected ENumber 0"

    [<Fact>]
    let ``parse true`` () =
        match Parser.parse "true" with
        | Ok (EBool true) -> ()
        | _ -> failwith "Expected EBool true"

    [<Fact>]
    let ``parse false`` () =
        match Parser.parse "false" with
        | Ok (EBool false) -> ()
        | _ -> failwith "Expected EBool false"

    [<Fact>]
    let ``parse symbol`` () =
        match Parser.parse "x" with
        | Ok (ESymbol "x") -> ()
        | _ -> failwith "Expected ESymbol x"

    [<Fact>]
    let ``parse symbol with hyphen`` () =
        match Parser.parse "my-var" with
        | Ok (ESymbol "my-var") -> ()
        | _ -> failwith "Expected ESymbol my-var"

    // ── Errors: bad input ────────────────────────────────────────────────────

    [<Fact>]
    let ``empty input returns error`` () =
        match Parser.parse "" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``whitespace only returns error`` () =
        match Parser.parse "   \n\t  " with
        | Error _ -> ()
        | _ -> failwith "Expected error for whitespace-only input"

    [<Fact>]
    let ``extra tokens after expression returns error`` () =
        match Parser.parse "1 2" with
        | Error _ -> ()
        | _ -> failwith "Expected error for extra tokens"

    [<Fact>]
    let ``unmatched closing paren returns error`` () =
        match Parser.parse ")" with
        | Error _ -> ()
        | _ -> failwith "Expected error for unmatched ')'"

    [<Fact>]
    let ``unclosed paren returns error`` () =
        match Parser.parse "(+ 1 2" with
        | Error _ -> ()
        | _ -> failwith "Expected error for unclosed '('"

    [<Fact>]
    let ``empty list returns error`` () =
        match Parser.parse "()" with
        | Error _ -> ()
        | _ -> failwith "Expected error for empty list"

    // ── Application ──────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse simple application`` () =
        match Parser.parse "(+ 1 2)" with
        | Ok (EApply(ESymbol "+", [ENumber 1; ENumber 2])) -> ()
        | _ -> failwith "Unexpected result"

    [<Fact>]
    let ``parse single-argument application`` () =
        match Parser.parse "(f x)" with
        | Ok (EApply(ESymbol "f", [ESymbol "x"])) -> ()
        | _ -> failwith "Expected single-arg application"

    [<Fact>]
    let ``parse nested application`` () =
        match Parser.parse "(+ (* 2 3) 1)" with
        | Ok (EApply(ESymbol "+", [EApply(ESymbol "*", [ENumber 2; ENumber 3]); ENumber 1])) -> ()
        | _ -> failwith "Expected nested application"

    [<Fact>]
    let ``parse application with boolean arg`` () =
        match Parser.parse "(f true)" with
        | Ok (EApply(ESymbol "f", [EBool true])) -> ()
        | _ -> failwith "Expected application with bool arg"

    // ── If ───────────────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse if`` () =
        match Parser.parse "(if true 1 0)" with
        | Ok (EIf(EBool true, ENumber 1, ENumber 0)) -> ()
        | _ -> failwith "if parse failed"

    [<Fact>]
    let ``parse if with nested condition`` () =
        match Parser.parse "(if (= x 0) 1 2)" with
        | Ok (EIf(EApply(ESymbol "=", [ESymbol "x"; ENumber 0]), ENumber 1, ENumber 2)) -> ()
        | _ -> failwith "Expected if with nested condition"

    [<Fact>]
    let ``parse if with nested branches`` () =
        match Parser.parse "(if true (+ 1 2) (+ 3 4))" with
        | Ok (EIf(EBool true, EApply(ESymbol "+", [ENumber 1; ENumber 2]),
                              EApply(ESymbol "+", [ENumber 3; ENumber 4]))) -> ()
        | _ -> failwith "Expected if with nested branches"

    [<Fact>]
    let ``invalid if missing else returns error`` () =
        match Parser.parse "(if true 1)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for invalid if"

    [<Fact>]
    let ``invalid if too many args returns error`` () =
        match Parser.parse "(if true 1 2 3)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for if with 4 args"

    [<Fact>]
    let ``invalid if no args returns error`` () =
        match Parser.parse "(if)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for (if)"

    // ── Let ──────────────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse let`` () =
        match Parser.parse "(let x 10 x)" with
        | Ok (ELet("x", ENumber 10, ESymbol "x")) -> ()
        | _ -> failwith "let parse failed"

    [<Fact>]
    let ``parse let with expression value`` () =
        match Parser.parse "(let x (+ 1 2) x)" with
        | Ok (ELet("x", EApply(ESymbol "+", [ENumber 1; ENumber 2]), ESymbol "x")) -> ()
        | _ -> failwith "Expected let with expression value"

    [<Fact>]
    let ``parse let with nested body`` () =
        match Parser.parse "(let x 5 (+ x 1))" with
        | Ok (ELet("x", ENumber 5, EApply(ESymbol "+", [ESymbol "x"; ENumber 1]))) -> ()
        | _ -> failwith "Expected let with nested body"

    [<Fact>]
    let ``parse let sugar`` () =
        match Parser.parse "(let x = 10 x)" with
        | Ok (ELet("x", ENumber 10, ESymbol "x")) -> ()
        | _ -> failwith "let sugar failed"

    [<Fact>]
    let ``let classic and sugar produce same AST`` () =
        let classic = Parser.parse "(let x 10 x)"
        let sugar   = Parser.parse "(let x = 10 x)"
        match classic, sugar with
        | Ok a, Ok b when a = b -> ()
        | _ -> failwith "let classic and sugar ASTs differ"

    [<Fact>]
    let ``invalid let missing body returns error`` () =
        match Parser.parse "(let x 10)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for invalid let"

    [<Fact>]
    let ``invalid let sugar missing body returns error`` () =
        match Parser.parse "(let x = 10)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``invalid let too few args returns error`` () =
        match Parser.parse "(let x)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for (let x)"

    // ── Letrec ───────────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse letrec`` () =
        match Parser.parse "(letrec f 10 f)" with
        | Ok (ELetRec("f", ENumber 10, ESymbol "f")) -> ()
        | _ -> failwith "letrec parse failed"

    [<Fact>]
    let ``parse letrec with lambda`` () =
        match Parser.parse "(letrec fact (lambda (n) n) (fact 5))" with
        | Ok (ELetRec("fact", ELambda(["n"], ESymbol "n"),
                      EApply(ESymbol "fact", [ENumber 5]))) -> ()
        | _ -> failwith "Expected letrec with lambda"

    [<Fact>]
    let ``invalid letrec missing body returns error`` () =
        match Parser.parse "(letrec f 10)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for invalid letrec"

    [<Fact>]
    let ``invalid letrec no args returns error`` () =
        match Parser.parse "(letrec)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for (letrec)"

    // ── Lambda ───────────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse lambda single param`` () =
        match Parser.parse "(lambda (x) x)" with
        | Ok (ELambda(["x"], ESymbol "x")) -> ()
        | _ -> failwith "lambda parse failed"

    [<Fact>]
    let ``parse lambda multiple params`` () =
        match Parser.parse "(lambda (x y) (+ x y))" with
        | Ok (ELambda(["x"; "y"], EApply(ESymbol "+", [ESymbol "x"; ESymbol "y"]))) -> ()
        | _ -> failwith "Expected lambda with multiple params"

    [<Fact>]
    let ``parse lambda no params`` () =
        match Parser.parse "(lambda () 42)" with
        | Ok (ELambda([], ENumber 42)) -> ()
        | _ -> failwith "Expected lambda with no params"

    [<Fact>]
    let ``parse lambda with nested body`` () =
        match Parser.parse "(lambda (x) (if (= x 0) 0 x))" with
        | Ok (ELambda(["x"], EIf _)) -> ()
        | _ -> failwith "Expected lambda with nested body"

    [<Fact>]
    let ``invalid lambda number param returns error`` () =
        match Parser.parse "(lambda (x 1) x)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for number as param"

    [<Fact>]
    let ``invalid lambda bool param returns error`` () =
        match Parser.parse "(lambda (true) x)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for bool as param"

    [<Fact>]
    let ``invalid lambda missing body returns error`` () =
        match Parser.parse "(lambda (x))" with
        | Error _ -> ()
        | _ -> failwith "Expected error for lambda without body"

    // ── Lambda sugar ─────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse lambda sugar single param`` () =
        match Parser.parse "(x => x)" with
        | Ok (ELambda(["x"], ESymbol "x")) -> ()
        | _ -> failwith "lambda sugar failed"

    [<Fact>]
    let ``parse lambda sugar multiple params`` () =
        match Parser.parse "((x y) => (+ x y))" with
        | Ok (ELambda(["x"; "y"], EApply(ESymbol "+", [ESymbol "x"; ESymbol "y"]))) -> ()
        | _ -> failwith "lambda multi param failed"

    [<Fact>]
    let ``lambda classic and sugar produce same AST`` () =
        let classic = Parser.parse "(lambda (x) x)"
        let sugar   = Parser.parse "(x => x)"
        match classic, sugar with
        | Ok a, Ok b when a = b -> ()
        | _ -> failwith "lambda classic and sugar ASTs differ"

    [<Fact>]
    let ``invalid lambda sugar number param returns error`` () =
        match Parser.parse "(1 => 1)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``invalid lambda sugar bool param returns error`` () =
        match Parser.parse "(true => 1)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for bool param in sugar"

    [<Fact>]
    let ``invalid lambda sugar list with number param returns error`` () =
        match Parser.parse "((x 1) => x)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``invalid lambda sugar missing body returns error`` () =
        match Parser.parse "(x =>)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    // ── Delay / Force ────────────────────────────────────────────────────────

    [<Fact>]
    let ``parse delay`` () =
        match Parser.parse "(delay 42)" with
        | Ok (EDelay (ENumber 42)) -> ()
        | _ -> failwith "delay parse failed"

    [<Fact>]
    let ``parse delay with expression`` () =
        match Parser.parse "(delay (+ 1 2))" with
        | Ok (EDelay (EApply(ESymbol "+", [ENumber 1; ENumber 2]))) -> ()
        | _ -> failwith "Expected delay with expression"

    [<Fact>]
    let ``parse force`` () =
        match Parser.parse "(force x)" with
        | Ok (EForce (ESymbol "x")) -> ()
        | _ -> failwith "force parse failed"

    [<Fact>]
    let ``parse delay force roundtrip`` () =
        match Parser.parse "(force (delay 99))" with
        | Ok (EForce (EDelay (ENumber 99))) -> ()
        | _ -> failwith "Expected force(delay(99))"

    [<Fact>]
    let ``invalid delay no args returns error`` () =
        match Parser.parse "(delay)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``invalid delay too many args returns error`` () =
        match Parser.parse "(delay 1 2)" with
        | Error _ -> ()
        | _ -> failwith "Expected error for delay with 2 args"

    [<Fact>]
    let ``invalid force no args returns error`` () =
        match Parser.parse "(force)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    [<Fact>]
    let ``invalid force too many args returns error`` () =
        match Parser.parse "(force 1 2)" with
        | Error _ -> ()
        | _ -> failwith "Expected error"

    // ── Whitespace и форматирование ──────────────────────────────────────────

    [<Fact>]
    let ``parse with extra spaces`` () =
        match Parser.parse "  (  +   1   2  )  " with
        | Ok (EApply(ESymbol "+", [ENumber 1; ENumber 2])) -> ()
        | _ -> failwith "Expected application with extra spaces"

    [<Fact>]
    let ``parse with newlines`` () =
        match Parser.parse "(+\n  1\n  2\n)" with
        | Ok (EApply(ESymbol "+", [ENumber 1; ENumber 2])) -> ()
        | _ -> failwith "Expected application with newlines"

    [<Fact>]
    let ``parse with tabs`` () =
        match Parser.parse "(+\t1\t2)" with
        | Ok (EApply(ESymbol "+", [ENumber 1; ENumber 2])) -> ()
        | _ -> failwith "Expected application with tabs"

    // ── Вложенные конструкции ────────────────────────────────────────────────

    [<Fact>]
    let ``parse let with lambda value`` () =
        match Parser.parse "(let f (lambda (x) x) (f 42))" with
        | Ok (ELet("f", ELambda(["x"], ESymbol "x"),
                   EApply(ESymbol "f", [ENumber 42]))) -> ()
        | _ -> failwith "Expected let with lambda"

    [<Fact>]
    let ``parse deeply nested if`` () =
        match Parser.parse "(if true (if false 1 2) 3)" with
        | Ok (EIf(EBool true, EIf(EBool false, ENumber 1, ENumber 2), ENumber 3)) -> ()
        | _ -> failwith "Expected nested if"

    [<Fact>]
    let ``parse immediately applied lambda`` () =
        match Parser.parse "((lambda (x) x) 42)" with
        | Ok (EApply(ELambda(["x"], ESymbol "x"), [ENumber 42])) -> ()
        | _ -> failwith "Expected immediately applied lambda"