module FsRandom.ListTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates List.randomCreate`` () =
   let tester = Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! r1 = rawBits
         let! r2 = rawBits
         let! r3 = rawBits
         return [r1; r2; r3]
      }
      <| tester
   let actual = Random.get (List.randomCreate 3 rawBits) tester
   List.length actual |> should equal 3
   actual |> should equal expected

[<Test>]
let ``Validates List.randomInit`` () =
   let tester = Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! r1 = ``[0, 1)``
         let! r2 = ``[0, 1)``
         let! r3 = ``[0, 1)``
         return [r1 + 1.0; r2 + 2.0; r3 + 3.0]
      }
      <| tester
   let actual = Random.get (List.randomInit 3 (fun i -> Random.map (fun u -> u + float (i + 1)) ``[0, 1)``)) tester
   List.length actual |> should equal 3
   actual |> should equal expected
