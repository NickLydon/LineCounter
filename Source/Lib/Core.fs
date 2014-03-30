module LineCounter

open System.IO
open Microsoft.FSharp.Collections

let countLines directory includeFiles excludeFiles excludeDirectories = 

    let filter transform containedIn sequence =
        sequence |> Seq.filter(fun file -> let d = transform file in not (containedIn |> Seq.contains d))
    
    includeFiles
        |> Seq.map (fun pattern -> Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
        |> Seq.flatten
        |> filter Path.GetDirectoryName excludeDirectories
        |> filter Path.GetExtension excludeFiles
        |> Seq.map(fun file -> async { 
            use fs = File.OpenRead file in return! fs.AsyncRead(int fs.Length) 
        })
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> Seq.map System.Text.Encoding.ASCII.GetString
        |> Seq.map(fun fileContents -> fileContents.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries))
        |> Seq.flatten
        |> Seq.length
    
