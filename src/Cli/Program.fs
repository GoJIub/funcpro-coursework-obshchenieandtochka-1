open System
open System.IO
open Language

let printUsage () =
    printfn "Usage:"
    printfn "  dotnet run -- <file.x> [--ast]"
    printfn ""
    printfn "Options:"
    printfn "  --ast    Print AST"
    printfn ""
    printfn "Example:"
    printfn "  dotnet run -- examples/test.x --ast"

[<EntryPoint>]
let main args =
    match args with
    | [| |] ->
        printUsage ()
        1

    | _ ->
        try
            let showAst = args |> Array.exists (fun a -> a = "--ast")

            let filePath =
                args
                |> Array.tryFind (fun a -> not (a.StartsWith "--"))

            match filePath with
            | None ->
                printfn "Error: file path is required"
                printUsage ()
                1

            | Some path ->
                if not (File.Exists path) then
                    printfn $"Error: file not found: {path}"
                    1
                else
                    let source = File.ReadAllText path

                    match Parser.parse source with
                    | Ok expr ->
                        if showAst then
                            printfn "AST:"
                            printfn "%A" expr
                        else
                            printfn "Parse success:"
                            printfn "%A" expr
                        0

                    | Error err ->
                        printfn "Parse error:"
                        printfn "%A" err
                        1

        with ex ->
            printfn $"Unexpected error: {ex.Message}"
            1