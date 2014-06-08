module FsRandom.RandomTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates singleton`` () =
   let tester = Utility.defaultState
   Random.get (Random.singleton 42) tester |> should equal 42
   Random.get (Random.singleton "foo") tester |> should equal "foo"

[<Test>]
let ``Validates identity`` () =
   let tester = Utility.defaultState
   let expected = Random.get ``[0, 1)`` tester
   let actual = Random.get (Random.identity ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates transformBy`` () =
   let tester = Utility.defaultState
   let f = (+) 1.0
   let expected = Random.get ``[0, 1)`` tester |> f
   let actual = Random.get (Random.map f ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates transformBy2`` () =
   let tester = Utility.defaultState
   let f x y = 2.0 * x - y
   let expected =
      Random.get
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         return f u1 u2
      }
      <| tester
   let actual = Random.get (Random.map2 f ``[0, 1)`` ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates zip`` () =
   let tester = Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         return u1, u2
      }
      <| tester
   let actual = Random.get (Random.zip ``[0, 1)`` ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates zip3`` () =
   let tester = Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         let! u3 = ``[0, 1)``
         return u1, u2, u3
      }
      <| tester
   let actual = Random.get (Random.zip3 ``[0, 1)`` ``[0, 1)`` ``[0, 1)``) tester
   actual |> should equal expected

[<Test>]
let ``Validates merge`` () =
   let tester = Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         let! u3 = ``[0, 1)``
         return [u1; u2; u3]
      }
      <| tester
   let actual = Random.get (Random.merge <| List.init 3 (fun _ -> ``[0, 1)``)) tester
   actual |> should equal expected

[<Test>]
let ``Validates mergeWith`` () =
   let tester = Utility.defaultState
   let f = List.reduce (+)
   let expected =
      Random.get
      <| random {
         let! u1 = ``[0, 1)``
         let! u2 = ``[0, 1)``
         let! u3 = ``[0, 1)``
         return f [u1; u2; u3]
      }
      <| tester
   let actual = Random.get (Random.mergeWith f <| List.init 3 (fun _ -> ``[0, 1)``)) tester
   actual |> should equal expected
