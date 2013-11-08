module FsRandom.Issues

open System
open FsRandom.Utility
open NUnit.Framework
open FsUnit

[<Test>]
[<Category("Issue #17")>]
let ``flipCoin can accept 0`` () =
   let tester = getDefaultTester ()
   Random.get (flipCoin 0.0) tester |> ignore

[<Test>]
[<Category("Issue #17")>]
let ``flipCoin can accept 1`` () =
   let tester = getDefaultTester ()
   Random.get (flipCoin 1.0) tester |> ignore
   
[<Test>]
[<Category("Issue #45")>]
[<ExpectedException(typeof<ArgumentException>)>]
let ``sampleWithReplacement fails before run`` () =
   Array.sampleWithReplacement 0 Array.empty<int> |> should throw typeof<ArgumentException>
   Assert.Fail ()

[<Test>]
[<Category("Issue #45")>]
[<ExpectedException(typeof<ArgumentException>)>]
let ``weightedSampleWithReplacement fails before run`` () =
   Array.weightedSampleWithReplacement 0 Array.empty<float> Array.empty<int> |> should throw typeof<ArgumentException>
   Assert.Fail ()
