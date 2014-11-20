module FsRandom.StringTest

open System
open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates randomByString`` () =
   let tester = Utility.defaultState
   let actual = Random.get (String.randomByString "FsRandom" 1000) tester
   String.length actual |> should equal 1000
   let actual = List.ofSeq actual
   actual |> should contain 'F'
   actual |> should contain 's'
   actual |> should contain 'R'
   actual |> should contain 'a'
   actual |> should contain 'n'
   actual |> should contain 'd'
   actual |> should contain 'o'
   actual |> should contain 'm'
   actual |> should not' (contain 'r')
   actual |> should not' (contain '#')

[<Test>]
let ``randomByString _ 0 generates empty string`` () =
   let tester = Utility.defaultState
   let actual = Random.get (String.randomByString "" 0) tester
   actual |> should be EmptyString

[<Test>]
let ``Validates randomAscii`` () =
   let tester = Utility.defaultState
   let actual = Random.get (String.randomAscii 1000) tester
   String.length actual |> should equal 1000
   actual |> String.forall (fun c -> int c < 128) |> should be True
   actual |> String.exists (fun c -> Char.IsControl (c)) |> should be False
   actual |> String.exists (fun c -> Char.IsWhiteSpace (c)) |> should be False

[<Test>]
let ``Validates randomNumeric`` () =
   let tester = Utility.defaultState
   let actual = Random.get (String.randomNumeric 1000) tester
   String.length actual |> should equal 1000
   actual
   |> String.forall (fun c -> '0' <= c && c <= '9')
   |> should be True

[<Test>]
let ``Validates randomAlphabet`` () =
   let tester = Utility.defaultState
   let actual = Random.get (String.randomAlphabet 1000) tester
   String.length actual |> should equal 1000
   actual
   |> String.forall (fun c -> ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'))
   |> should be True

[<Test>]
let ``Validates randomAlphanumeric`` () =
   let tester = Utility.defaultState
   let actual = Random.get (String.randomAlphanumeric 1000) tester
   String.length actual |> should equal 1000
   actual
   |> String.forall (fun c -> '0' <= c && c <= '9' || ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'))
   |> should be True

[<Test>]
let ``Validates randomConcat`` () =
   let tester = Utility.defaultState
   let suffix = ".png"
   let generators = [String.randomByString "AB" 3; String.randomNumeric 5; Random.singleton suffix]
   let actual = Random.get (String.randomConcat "x" generators) tester
   String.length actual |> should equal (3 + 5 + String.length suffix + 2)
   actual.[3] |> should equal 'x'
   actual.[9] |> should equal 'x'
   actual
   |> fun s -> s.Substring (0, 3)
   |> String.forall (fun c -> c = 'A' || c = 'B')
   |> should be True
   actual
   |> fun s -> s.Substring (4, 5)
   |> String.forall (fun c -> '0' <= c && c <= '9')
   |> should be True
   actual
   |> fun s -> s.Substring (10)
   |> should equal ".png"
