module FsRandom.Tests.Issues

open FsRandom
open FsRandom.Statistics
open FsRandom.Utility
open NUnit.Framework

[<Test>]
let ``flipCoin can accept 0 (#17)`` () =
   let builder, seed = getDefaultTester ()
   builder { return! flipCoin 0.0 } <| seed |> ignore

[<Test>]
let ``flipCoin can accept 1 (#17)`` () =
   let builder, seed = getDefaultTester ()
   builder { return! flipCoin 1.0 } <| seed |> ignore
