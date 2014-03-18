module FsRandom.OptimizationTest

open System
open System.Diagnostics
open FsUnit
open NUnit.Framework

[<ReflectedDefinition>]
let generator = random {
   let! x = ``[0, 1)``
   use e = (seq { 1 .. 3 }).GetEnumerator ()
   let sum = ref 0.0
   while e.MoveNext () do
      let! u = ``[0, 1)``
      sum := !sum + u
   try
      return !sum
   with
      | _ -> return x
}

[<Test>]
let ``optimize doesn't change results`` () =
   let tester = getDefaultTester ()
   let optimized = Optimization.optimize <@ generator @>
   let expected = Random.get generator tester
   let actual = Random.get optimized tester
   actual |> should equal expected
   ()

[<Test>]
let ``optimized generator is faster than original generator`` () =
   let tester = getDefaultTester ()
   let optimized = Optimization.optimize <@ generator @>
   let time g n =
      let stopwatch = Stopwatch ()
      seq {
         for i = 1 to n do
            stopwatch.Restart ()
            Random.get g tester |> ignore
            stopwatch.Stop ()
            yield stopwatch.ElapsedTicks
      }
      |> Seq.toList
      |> List.sort
      |> Seq.skip (n / 4)
      |> Seq.take (n / 2)
      |> Seq.averageBy float
   let n = 10000
   time optimized n
   |> should lessThan (time generator n)
   ()
