module LineCounter

open System.IO
open Microsoft.FSharp.Collections
open System.Text

let countLines directory includeFiles excludeFiles excludeDirectories = 

    let filter transform containedIn sequence =
        sequence |> Seq.filter(fun file -> let d = transform file in not (containedIn |> Seq.contains d))
    
    includeFiles
        |> Seq.map (fun pattern -> Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
        |> Seq.flatten
        |> filter Path.GetDirectoryName excludeDirectories
        |> filter Path.GetExtension excludeFiles
        |> Seq.map(fun file -> async {             
            use fs = File.OpenRead file 
            let! bytes = fs.AsyncRead(int fs.Length) 
            let fileContents = Encoding.ASCII.GetString bytes
            return fileContents.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries).Length
        })
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> Seq.sum