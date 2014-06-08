module FsRandom.UtilityTest

open FsRandom.Utility
open FsUnit
open NUnit.Framework

[<Test>]
let ``Validates randomSign (int)`` () =
   let g = Random.map (fun sign -> sign * 1) (randomSign ())
   g.GetType () |> should equal typeof<GeneratorFunction<int>>

[<Test>]
let ``Validates randomSign (float)`` () =
   let g = Random.map (fun sign -> sign * 1.0) (randomSign ())
   g.GetType () |> should equal typeof<GeneratorFunction<float>>

[<Test>]
let ``Validates choose`` () =
   let n = 4
   let tester = Utility.defaultState
   let result = Random.get (Utility.choose 10 n) tester
   Assert.That (List.length result, Is.EqualTo(n))
   Assert.That (List.forall (fun x -> List.exists ((=) x) [0..9]) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.EqualTo(n))

[<Test>]
let ``Validates chooseOne`` () =
   let tester = Utility.defaultState
   let result = Random.get (Utility.chooseOne 10) tester
   Assert.That (0 <= result && result < 10, Is.True)
