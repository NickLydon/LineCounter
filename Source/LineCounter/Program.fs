// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System.IO
open System.Diagnostics
open LineCounter

[<EntryPoint>]
let main argv = 

    if(argv.Length = 0) then failwith "Directory must be provided as first argument"

    let timer = Stopwatch()
    timer.Start() 
    let directory = argv.[0]

    let searchPatterns = seq {
        if (argv.Length = 1) then yield "*"
        yield! Seq.ofArray argv 
            |> Seq.skip 1 
            |> Seq.takeWhile (fun _ -> true)
    }
                         
    let lineCounts = countLines directory searchPatterns

    printfn "Lines: %i" lineCounts

    timer.Stop()

    printfn "Took: %i milliseconds" timer.ElapsedMilliseconds

    0 // return an integer exit code

