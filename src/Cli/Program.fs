open System
open System.IO
open Language
open Language.Parser
open Language.Evaluator
open Language.Environment
open Language.Builtins
open Language.Trace

let printUsage () =
    printfn "Usage:"
    printfn "  dotnet run --project src/Cli -- <file.x> [--ast] [--trace]"
    printfn ""
    printfn "Options:"
    printfn "  --ast    Print AST"
    printfn "  --trace  Enable evaluation trace"
    printfn ""
    printfn "Example:"
    printfn "  dotnet run --project src/Cli -- examples/test.x --trace"

[<EntryPoint>]
let main args =
    if args.Length = 0 then
        printUsage ()
        1
    else
        let filePath = args.[0]
        let flags = args |> Array.skip 1

        if not (File.Exists filePath) then
            printfn "File not found: %s" filePath
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
                    printfn "Result: %s" (PrettyPrinter.printValue value)
                    0
                | Error err ->
                    printfn "Runtime error: %A" err
                    1