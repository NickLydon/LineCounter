module LineCounter

open System.IO
open Microsoft.FSharp.Collections

let countLines directory includeFiles excludeFiles = 
    
    includeFiles
        |> Seq.map (fun pattern -> Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
        |> Seq.flatten
        |> Seq.filter (fun file -> not (excludeFiles |> Seq.exists(fun extension -> Path.GetExtension(file) = extension)))
        |> Seq.map(fun file -> async { 
            use fs = File.OpenRead file in return! fs.AsyncRead(fs.Length |> int) 
        })
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> Seq.map System.Text.Encoding.ASCII.GetString
        |> Seq.map(fun fileContents -> fileContents.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries))
        |> Seq.flatten
        |> Seq.length
    
