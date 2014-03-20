module File1

    let (|Xyz|Abc|_|) input = 
        match input with
        | _ when input = "XYZ" -> Some(Xyz)
        | _ when input = "ABC" ->  Some(Abc)
        | _ -> None