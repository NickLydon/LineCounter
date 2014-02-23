module Seq

    let flatten x = x |> Seq.collect(fun i -> i)