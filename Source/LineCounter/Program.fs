open System.IO
open System.Diagnostics
open LineCounter
open Microsoft.FSharp.Core

module LineCounter =
    
    type CollectionType =
        | Include
        | Exclude
        | Skip
    
    let [<Literal>] IncludeKey = "-include"
    let [<Literal>] ExcludeKey  = "-exclude"
    
    [<EntryPoint>]
    let main argv = 

        let directory, searchPatterns = 
            match argv with 
            | [| |] -> failwith "Directory must be provided as first argument"
            | [| directory |] -> directory, [| IncludeKey; "*" |]
            | _ -> argv.[0], argv.[ 1.. ]

        let splitIncludesAndExcludes =
            searchPatterns |> 
                Seq.fold(fun (acc,collecting) next -> 
                    match next with
                    | IncludeKey -> (acc, Include)
                    | ExcludeKey -> (acc, Exclude)                                       
                    | next -> 
                        match collecting with
                        | Skip -> (acc, collecting)
                        | Include -> 
                            let inc, exc = acc
                            ((next::inc, exc), Include)
                        | Exclude -> 
                            let inc, exc = acc
                            ((inc, next::exc), Exclude)) 
                    (([], []), Skip)
            |> fst
                
        let lineCounts = countLines directory ((fst splitIncludesAndExcludes) |> List.map(fun p -> "*" + p)) (snd splitIncludesAndExcludes)

        printfn "Lines: %i" lineCounts
        #if DEBUG
        System.Console.ReadKey() |> ignore
        #endif
        0 // return an integer exit code

