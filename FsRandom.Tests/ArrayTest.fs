module FsRandom.ArrayTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates randomCreate`` () =
   let tester = getDefaultTester ()
   let expected =
      Random.get
      <| random {
         let r = Array.zeroCreate 3
         for index = 0 to 2 do
            let! u = ``[0, 1)``
            r.[index] <- u
         return r
      }
      <| tester
   let actual = Random.get (Array.randomCreate 3 ``[0, 1)``) tester
   Array.length actual |> should equal 3
   actual |> should equal expected

[<Test>]
let ``Validates randomInit`` () =
   let tester = getDefaultTester ()
   let expected =
      Random.get
      <| random {
         let r = Array.zeroCreate 3
         for index = 0 to 2 do
            let! u = ``[0, 1)``
            r.[index] <- float index + u
         return r
      }
      <| tester
   let actual = Random.get (Array.randomInit 3 (fun i -> Random.transformBy (fun u -> float i + u) ``[0, 1)``)) tester
   Array.length actual |> should equal 3
   actual |> should equal expected

[<Test>]
let ``Validates randomFill`` () =
   let tester = getDefaultTester ()
   let expected =
      Random.get
      <| random {
         let r = Array.zeroCreate 5
         for index = 2 to 3 do
            let! u = ``[0, 1)``
            r.[index] <- u
         return r
      }
      <| tester
   let actual = Array.zeroCreate 5
   Random.get (Array.randomFill actual 2 2 ``[0, 1)``) tester
   actual |> should equal expected
