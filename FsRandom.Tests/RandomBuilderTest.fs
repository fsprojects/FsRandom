module FsRandom.RandomBuilderTest

open FsUnit
open NUnit.Framework

[<Test>]
let ``Satisfies monad law 1 (left identity)`` () =
   let tester = getDefaultTester ()
   let a = 1.0
   let f x = Random.transformBy ((+) x) ``[0, 1)``
   let l = random.Bind (random.Return (a), f) |> Random.get <| tester
   let r = f a |> Random.get <| tester
   r |> should equal l

[<Test>]
let ``Satisfies monad law 2 (right identity)`` () =
   let tester = getDefaultTester ()
   let m = ``[0, 1)``
   let l = random.Bind (m, random.Return) |> Random.get <| tester
   let r = m |> Random.get <| tester
   r |> should equal l

[<Test>]
let ``Satisfies monad law 3 (associativity)`` () =
   let tester = getDefaultTester ()
   let m = ``[0, 1)``
   let f x = Random.transformBy ((+) x) ``[0, 1)``
   let g x = Random.transformBy (fun t -> t - x) ``[0, 1)``
   let l = random.Bind (random.Bind (m, f), g) |> Random.get <| tester
   let r = random.Bind (m, fun y -> random.Bind (f y, g)) |> Random.get <| tester
   r |> should equal l

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
   <| getDefaultTester ()
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
   <| getDefaultTester ()
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
   <| getDefaultTester ()
   |> should be False
   r.Closed |> should be True
