module FsRandom.RandomTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates identity`` () =
   let tester = getDefaultTester ()
   let expected = getRandom ``[0, 1)`` tester
   let actual = getRandom (Random.identity ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates transformBy`` () =
   let tester = getDefaultTester ()
   let f = (+) 1.0
   let expected = getRandom ``[0, 1)`` tester |> f
   let actual = getRandom (Random.transformBy f ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates transformBy2`` () =
   let tester = getDefaultTester ()
   let f x y = 2.0 * x - y
   let expected =
      getRandom
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         return f u1 u2
      }
      <| tester
   let actual = getRandom (Random.transformBy2 f ``[0, 1)`` ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates zip`` () =
   let tester = getDefaultTester ()
   let expected =
      getRandom
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         return u1, u2
      }
      <| tester
   let actual = getRandom (Random.zip ``[0, 1)`` ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates zip3`` () =
   let tester = getDefaultTester ()
   let expected =
      getRandom
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         let! u3 = ``[0, 1)``
         return u1, u2, u3
      }
      <| tester
   let actual = getRandom (Random.zip3 ``[0, 1)`` ``[0, 1)`` ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates merge`` () =
   let tester = getDefaultTester ()
   let expected =
      getRandom
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         let! u3 = ``[0, 1)``
         return [u1; u2; u3]
      }
      <| tester
   let actual = getRandom (Random.merge <| List.init 3 (fun _ -> ``[0, 1)``)) tester
   actual |> should equal expected
