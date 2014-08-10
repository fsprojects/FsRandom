module FsRandom.RandomBuilderTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Satisfies monad law 1 (left identity)`` () =
   let tester = Utility.defaultState
   let a = 1.0
   let f x = Random.map ((+) x) ``[0, 1)``
   let l = Random.bind f (random.Return (a)) |> Random.get <| tester
   let r = f a |> Random.get <| tester
   r |> should equal l

[<Test>]
let ``Satisfies monad law 2 (right identity)`` () =
   let tester = Utility.defaultState
   let m = ``[0, 1)``
   let l = Random.bind random.Return m |> Random.get <| tester
   let r = m |> Random.get <| tester
   r |> should equal l

[<Test>]
let ``Satisfies monad law 3 (associativity)`` () =
   let tester = Utility.defaultState
   let m = ``[0, 1)``
   let f x = Random.map ((+) x) ``[0, 1)``
   let g x = Random.map (fun t -> t - x) ``[0, 1)``
   let l = Random.bind g (Random.bind f m) |> Random.get <| tester
   let r = Random.bind (fun y -> Random.bind g (f y)) m |> Random.get <| tester
   r |> should equal l

[<Test>]
let ``Can use if-else expression in random computation expression (true)`` () =
   let actual =
      Random.get
      <| random {
         let u = ref 0uL
         let! x = rawBits
         if true then
            u := x
         else
            u := x >>> 1
         return !u
      }
      <| Utility.defaultState
   let expected = Random.get rawBits Utility.defaultState
   actual |> should equal expected

[<Test>]
let ``Can use if-else expression in random computation expression (false)`` () =
   let actual =
      Random.get
      <| random {
         let u = ref 0uL
         let! x = rawBits
         if false then
            u := x
         else
            u := x >>> 1
         return !u
      }
      <| Utility.defaultState
   let expected = Random.get <| Random.map (fun u -> u >>> 1) rawBits <| Utility.defaultState
   actual |> should equal expected

[<Test>]
let ``Can use if without else expression in random computation expression`` () =
   let actual =
      Random.get
      <| random {
         let u = ref -1.0
         if false then
            let! x = ``[0, 1)``
            u := x
         return !u
      }
      <| Utility.defaultState
   actual |> should equal -1.0

[<Test>]
let ``Can use while-do expression in random computation expression`` () =
   let actual =
      Random.get
      <| random {
         let r = ref []
         let i = ref 0
         while !i < 3 do
            let! x = rawBits
            r := x :: !r
            incr i
         return !r
      }
      <| Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! x = rawBits
         let! y = rawBits
         let! z = rawBits
         return [z; y; x]
      }
      <| Utility.defaultState
   actual |> should equal expected

[<Test>]
let ``Can use for-do expression in random computation expression`` () =
   let actual =
      Random.get
      <| random {
         let r = ref []
         for i = 0 to 2 do
            let! x = rawBits
            r := x :: !r
         return !r
      }
      <| Utility.defaultState
   let expected =
      Random.get
      <| random {
         let! x = rawBits
         let! y = rawBits
         let! z = rawBits
         return [z; y; x]
      }
      <| Utility.defaultState
   actual |> should equal expected

[<Test>]
let ``Can use try-with expression in random computation expression`` () =
   Random.get
   <| random {
      try
         invalidOp ""
         return false
      with
         | _ -> return true
   }
   <| Utility.defaultState
   |> should be True

[<Test>]
let ``Can use try-finally in random computation expression`` () =
   let isFinallyRun = ref false
   Random.get
   <| random {
      try
         return true
      finally
         isFinallyRun := true
   }
   <| Utility.defaultState
   |> should be True
   !isFinallyRun |> should be True

type Resource () =
   member val Closed = false with get, set
   interface System.IDisposable with
      member this.Dispose () = this.Closed <- true
[<Test>]
let ``Can use use binding in random computation expression`` () =
   let r = new Resource ()
   Random.get
   <| random {
      use r2 = r
      return r2.Closed
   }
   <| Utility.defaultState
   |> should be False
   r.Closed |> should be True
