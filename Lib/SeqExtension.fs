module Seq

    let flatten x = x |> Seq.collect id