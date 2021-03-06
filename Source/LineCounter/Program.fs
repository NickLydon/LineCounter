﻿open System.IO
open System.Diagnostics
open LineCounter
open Microsoft.FSharp.Core

module LineCounter =
    
    type CollectionType =
        | Include
        | Exclude
        | ExcludeDirectory
        | Skip
    
    type FilePatterns = { Includes: string list; Excludes: string list; ExcludeDirectories: string list; }

    let [<Literal>] IncludeKey = "--ifile"
    let [<Literal>] ExcludeKey = "--efile"
    let [<Literal>] ExcludeDirectoryKey = "--edirectory"
    
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
                    | ExcludeDirectoryKey -> (acc, ExcludeDirectory)                                       
                    | next -> 
                        match collecting with
                        | Skip -> (acc, collecting)
                        | Include ->                             
                            ({ acc with Includes = next::acc.Includes }, Include)
                        | Exclude -> 
                            ({ acc with Excludes = next::acc.Excludes }, Exclude)
                        | ExcludeDirectory -> 
                            ({ acc with ExcludeDirectories = next::acc.ExcludeDirectories }, ExcludeDirectory))
                    ({ Includes = []; Excludes = []; ExcludeDirectories = []; }, Skip)
            |> fst
        
        let sw = Stopwatch()
        sw.Start()
                
        let lineCounts = 
            countLines 
                directory 
                (splitIncludesAndExcludes.Includes |> List.map(fun p -> "*" + p))
                splitIncludesAndExcludes.Excludes
                splitIncludesAndExcludes.ExcludeDirectories

        sw.Stop()

        printfn "Lines: %i; TIme: %i" lineCounts sw.ElapsedMilliseconds
        #if DEBUG
        System.Console.ReadKey() |> ignore
        #endif
        0 // return an integer exit code

