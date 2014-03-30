namespace LineCounter.Tests

open NUnit.Framework
open NUnit
open System.IO
open System.Linq

type Class1() = 
    
    let sut = Program.LineCounter.main
    let currentDirectory =System.Environment.CurrentDirectory

    [<Theory>]
    member x.``should throw when directory not specified``() = 
        Assert.Throws(fun () -> sut([| |]) |> ignore) |> ignore
    
    [<Theory>]
    member x.``should check all files when not given specific patterns``() = 
        let actual = 
            LineCounter.countLines 
                (currentDirectory + @"\Samples\Nested")
                [ "*" ]
                Seq.empty
                Seq.empty

        Assert.AreEqual(3, actual)    

    [<Theory>]
    member x.``should exclude specified directories``() = 
        let actual = 
            LineCounter.countLines 
                (System.Environment.CurrentDirectory + @"\Samples\Ignore")
                [ "*" ]
                Seq.empty
                [ currentDirectory + @"\Samples\Ignore\Exc" ]
                    
        Assert.AreEqual(1, actual)    