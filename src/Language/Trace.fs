namespace Language

module Trace =

    let mutable private indent = 0
    let mutable enabled = false

    let setEnabled value =
        enabled <- value


    let private reset = "\u001b[0m"
    let private blue = "\u001b[34m"
    let private green = "\u001b[32m"
    let private yellow = "\u001b[33m"

    let private color c s =
        c + s + reset


    let private prefix () =
        String.replicate indent "  "


    let enter (label: string) =
        if enabled then
            let colored =
                if label.StartsWith("eval") then color blue label
                elif label.StartsWith("apply") then color yellow label
                else label

            printfn "%s→ %s" (prefix ()) colored
            indent <- indent + 1

    let exitWithResult result =
        if enabled then
            indent <- indent - 1
            printfn "%s← %s" (prefix ()) (color green ("result " + result))