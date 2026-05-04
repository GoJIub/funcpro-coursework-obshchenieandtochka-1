open System
open System.IO
open Language
open Language.Parser
open Language.Evaluator
open Language.Environment
open Language.Builtins
open Language.Trace
open Language.PrettyPrinter

let printUsage () =
    printfn "Usage:"
    printfn "  dotnet run --project src/Cli -- <file.x> [--ast] [--trace] [--help]"
    printfn ""
    printfn "Options:"
    printfn "  --ast     Print AST"
    printfn "  --trace   Show evaluation trace"
    printfn "  --help    Show this message"
    printfn ""
    printfn "Examples:"
    printfn "  dotnet run --project src/Cli -- examples/test.x"
    printfn "  dotnet run --project src/Cli -- examples/test.x --ast"
    printfn "  dotnet run --project src/Cli -- examples/test.x --trace"

[<EntryPoint>]
let main args =
    let argsList = args |> Array.toList

    // --help или пустой запуск
    if argsList.IsEmpty || argsList |> List.contains "--help" then
        printUsage ()
        0
    else
        let filePath = args.[0]
        let flags = args |> Array.skip 1

        if not (File.Exists filePath) then
            printfn "Error: file not found: %s" filePath
            1
        else
            let source = File.ReadAllText filePath

            let showAst = flags |> Array.contains "--ast"
            let enableTrace = flags |> Array.contains "--trace"

            Trace.enabled <- enableTrace

            match parse source with
            | Error err ->
                printfn "Parse error: %A" err
                1

            | Ok expr ->

                if showAst then
                    printfn "AST:\n%A" expr

                let env =
                    Environment.empty
                    |> Map.fold (fun acc k v -> Map.add k v acc)
                        (Builtins.makeBuiltins eval)

                match eval env expr with
                | Ok value ->
                    printfn "Result: %s" (printValue value)
                    0
                | Error err ->
                    printfn "Runtime error: %A" err
                    1