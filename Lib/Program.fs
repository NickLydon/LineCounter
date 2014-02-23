// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

module LineCounter

open System.IO
open Microsoft.FSharp.Collections

let countLines directory searchPatterns = 
    
    searchPatterns
        |> Seq.map (fun pattern -> Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
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
        |> Seq.map(fun fileContents -> fileContents.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries))
        |> Seq.flatten
        |> Seq.length
    
