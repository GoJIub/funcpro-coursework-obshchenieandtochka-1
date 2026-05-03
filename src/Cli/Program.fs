open System
open System.IO
open Language

[<EntryPoint>]
let main args =
    let printUsage () =
        printfn "Usage:"
        printfn "  dotnet run --project src/Cli -- <file.x> [--ast]"
        printfn ""
        printfn "Options:"
        printfn "  --ast    Print AST"
        printfn ""
        printfn "Example:"
        printfn "  dotnet run --project src/Cli -- examples/test.x --ast"
        printfn "  dotnet run --project src/Cli -- examples/test.x"

    match args |> Array.toList with
    | [] ->
        printUsage ()
        1

    | filePath :: rest ->
        if not (File.Exists filePath) then
            printfn $"File not found: {filePath}"
            1
        else
            let source = File.ReadAllText filePath

            match Parser.parse source with
            | Error err ->
                printfn $"Parse error: {err}"
                1

            | Ok ast ->
                if rest |> List.contains "--ast" then
                    printfn "AST:"
                    printfn "%A" ast
                    0
                else
                    let env = Builtins.makeBuiltins Evaluator.eval

                    match Evaluator.eval env ast with
                    | Ok value ->
                        printfn "Result:"
                        printfn "%A" value
                        0

                    | Error err ->
                        printfn $"Runtime error: {err}"
                        1