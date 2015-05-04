module FsRandom.Issues

open System
open System.Globalization
open FsRandom.Statistics
open FsRandom.Utility
open NUnit.Framework
open FsUnit

[<Test>]
[<Category("Issue #17")>]
let ``flipCoin can accept 0`` () =
   let tester = Utility.defaultState
   Random.get (flipCoin 0.0) tester |> ignore

[<Test>]
[<Category("Issue #17")>]
let ``flipCoin can accept 1`` () =
   let tester = Utility.defaultState
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
   let sample = Random.get m Utility.defaultState
   sample.[0] |> should be (lessThan 50.0)
   mu.[0] <- 100.0
   let sample = Random.get m Utility.defaultState
   sample.[0] |> should be (lessThan 50.0)

[<Test>]
[<Category("Issue #65")>]
let ``String.randomByString breaks surrogate pairs`` () =
   let s = "𠮷野家"  // the first character is a surrogate pair character
   let tester = Utility.defaultState
   let actual =
      let r = Random.get (String.randomByString s 100) tester
      StringInfo (r)
   actual.LengthInTextElements |> should equal 100
   let actual = seq { 0 .. 99 } |> Seq.map (fun i -> actual.SubstringByTextElements (i, 1)) |> Seq.toArray
   actual |> should contain "𠮷"
   actual |> should contain "野"
   actual |> should contain "家"

[<Test>]
[<Category("Issue #77")>]
let ``List.randomCreate doesn't throw StackOverflowException`` () =
   List.randomCreate 10000 rawBits
   |> Random.get <| Utility.defaultState
   |> ignore

[<Test>]
[<Category("Issue #77")>]
let ``List.randomInit doesn't throw StackOverflowException`` () =
   List.randomInit 10000 (fun _ -> rawBits)
   |> Random.get <| Utility.defaultState
   |> ignore
