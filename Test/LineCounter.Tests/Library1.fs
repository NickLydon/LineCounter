namespace LineCounter.Tests

open NUnit.Framework
open NUnit
open System.IO
open System.Linq

type Class1() = 
    
    let sut = Program.LineCounter.main

    [<NUnit.Framework.TheoryAttribute>]
    member x.``should throw when directory not specified``() = 
        Assert.Throws(fun () -> sut([| |]) |> ignore) |> ignore
    
    [<NUnit.Framework.TheoryAttribute>]
    member x.``should check all files when not given specific patterns``() = 
        let actual = 
            LineCounter.countLines 
                (Path.Combine "C:\\Users\\nickkell\\Documents\\GitHub\\LineCounter\\Test\\LineCounter.Tests\\Samples\\Nested")
                (Seq.singleton "*")
        Assert.AreEqual(3, actual)    