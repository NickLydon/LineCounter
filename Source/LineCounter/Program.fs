// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

//open System
open System.IO
open Microsoft.FSharp.Control.CommonExtensions
open Microsoft.FSharp.Collections

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv

    if(argv.Length = 0) then failwith "Directory must be provided as first argument"

    let timer = new System.Diagnostics.Stopwatch()
    timer.Start() 
    let directory = argv.[0]
    let enumerateFiles pattern = Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories)

    let searchPatterns = seq {
        yield "*"
        yield! Seq.ofArray argv 
            |> Seq.skip 1 
            |> Seq.takeWhile (fun _ -> true)
    }
   
    let allFileContent = 
        searchPatterns
        |> Seq.map enumerateFiles
        |> Seq.flatten
        |> Seq.map(fun file -> async {
            use fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
            let data = Array.create(int fs.Length) 0uy
            let! _ = fs.AsyncRead(data, 0, data.Length)
            return data         
        })
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> PSeq.map System.Text.Encoding.ASCII.GetString
                                
    let lineCounts =
        allFileContent
        |> Seq.map(fun i -> i.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries))
        |> Seq.flatten
        |> Seq.length
        
    printfn "Lines: %i" lineCounts

    timer.Stop()

    printfn "Took: %i milliseconds" timer.ElapsedMilliseconds

    0 // return an integer exit code

