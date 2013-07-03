module RecycleBin.Random.Tests.Issues

open RecycleBin.Random
open RecycleBin.Random.Statistics
open RecycleBin.Random.Utility
open NUnit.Framework

[<Test>]
let ``flipCoin can accept 0 (#17)`` () =
   let builder, seed = getDefaultTester ()
   builder { return! flipCoin 0.0 } <| seed |> ignore

[<Test>]
let ``flipCoin can accept 1 (#17)`` () =
   let builder, seed = getDefaultTester ()
   builder { return! flipCoin 1.0 } <| seed |> ignore
