namespace Language

module internal ValueFormatting =
    let valueTypeName value =
        match value with
        | VNumber _ -> "Number"
        | VBool _ -> "Bool"
        | VList _ -> "List"
        | VClosure _ -> "Closure"
        | VBuiltin _ -> "Builtin"
        | VMaybe _ -> "Maybe"
        | VThunk _ -> "Thunk"
