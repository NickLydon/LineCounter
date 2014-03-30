module Seq

    let flatten x = x |> Seq.collect id

    let contains y x = x |> Seq.exists(fun i -> i = y)