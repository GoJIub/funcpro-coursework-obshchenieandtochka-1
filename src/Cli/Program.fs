open System
open System.IO
open Language

let printUsage () =
    printfn "Usage:"
    printfn "  dotnet run -- <file.x>"
    printfn ""
    printfn "Example:"
    printfn "  dotnet run -- examples/test.x"

[<EntryPoint>]
let main args =
    match args with
    | [| |] ->
        printUsage ()
        1

    | [| filePath |] ->
        try
            if not (File.Exists filePath) then
                printfn $"Error: file not found: {filePath}"
                1
            else
                let source = File.ReadAllText filePath

                match Parser.parse source with
                | Ok expr ->
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

    | _ ->
        printfn "Error: too many arguments"
        printUsage ()
        1