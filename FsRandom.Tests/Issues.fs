module FsRandom.Issues

open System
open FsRandom.Statistics
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

[<Test>]
[<Category("Issue #60")>]
let ``multinormal (mu, _) is affected by modification of mu`` () =
   let mu = [|0.0; 0.0|]
   let sigma = Array2D.init 2 2 (fun i j -> if i = j then 1.0 else 0.7)
   let m = multinormal (mu, sigma)
   let sample = Random.get m (getDefaultTester ())
   sample.[0] |> should be (lessThan 50.0)
   mu.[0] <- 100.0
   let sample = Random.get m (getDefaultTester ())
   sample.[0] |> should be (lessThan 50.0)
