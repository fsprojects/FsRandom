module FsRandom.Tests.Issues

open FsRandom
open FsRandom.Utility
open NUnit.Framework

[<Test>]
let ``flipCoin can accept 0 (#17)`` () =
   let tester = getDefaultTester ()
   Random.get (flipCoin 0.0) tester |> ignore

[<Test>]
let ``flipCoin can accept 1 (#17)`` () =
   let tester = getDefaultTester ()
   Random.get (flipCoin 1.0) tester |> ignore
