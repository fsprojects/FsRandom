module FsRandom.Array2DTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates randomCreate`` () =
   let tester = getDefaultTester ()
   let expected =
      Random.get
      <| random {
         let r = Array2D.zeroCreate 3 2
         for i = 0 to 2 do
            for j = 0 to 1 do
               let! u = ``[0, 1)``
               r.[i, j] <- u
         return r
      }
      <| tester
   let actual = Random.get (Array2D.randomCreate 3 2 ``[0, 1)``) tester
   actual.GetLength (0) |> should equal 3
   actual.GetLength (1) |> should equal 2
   actual |> should equal expected

[<Test>]
let ``Validates randomCreateBased`` () =
   let tester = getDefaultTester ()
   let expected =
      Random.get
      <| random {
         let r = Array2D.zeroCreateBased 4 1 3 2
         for i = 4 to 6 do
            for j = 1 to 2 do
               let! u = ``[0, 1)``
               r.[i, j] <- u
         return r
      }
      <| tester
   let actual = Random.get (Array2D.randomCreateBased 4 1 3 2 ``[0, 1)``) tester
   actual.GetLength (0) |> should equal 3
   actual.GetLength (1) |> should equal 2
   actual |> should equal expected

[<Test>]
let ``Validates randomInit`` () =
   let tester = getDefaultTester ()
   let f i j u = float i + float j * u
   let expected =
      Random.get
      <| random {
         let r = Array2D.zeroCreate 3 2
         for i = 0 to 2 do
            for j = 0 to 1 do
               let! u = ``[0, 1)``
               r.[i, j] <- f i j u
         return r
      }
      <| tester
   let actual =
      Random.get
      <| Array2D.randomInit 3 2 (fun i j -> Random.transformBy (f i j) ``[0, 1)``)
      <| tester
   actual.GetLength (0) |> should equal 3
   actual.GetLength (1) |> should equal 2
   actual |> should equal expected

[<Test>]
let ``Validates randomInitBased`` () =
   let tester = getDefaultTester ()
   let f i j u = float i + float j * u
   let expected =
      Random.get
      <| random {
         let r = Array2D.zeroCreateBased 4 1 3 2
         for i = 4 to 6 do
            for j = 1 to 2 do
               let! u = ``[0, 1)``
               r.[i, j] <- f i j u
         return r
      }
      <| tester
   let actual =
      Random.get
      <| Array2D.randomInitBased 4 1 3 2 (fun i j -> Random.transformBy (f i j) ``[0, 1)``)
      <| tester
   actual.GetLength (0) |> should equal 3
   actual.GetLength (1) |> should equal 2
   actual |> should equal expected
