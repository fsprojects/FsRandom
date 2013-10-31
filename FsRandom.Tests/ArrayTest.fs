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

[<Test>]
let ``Validates Array.sample`` () =
   let array = Array.init 10 id
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.sample 8 array) tester
   Assert.That (next, Is.Not.EqualTo(tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.EqualTo(8))

[<Test>]
let ``Validates Array.weightedSample`` () =
   let array = Array.init 10 id
   let weight = Array.init (Array.length array) (id >> float >> ((+) 1.0))
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.weightedSample 8 weight array) tester
   Assert.That (next, Is.Not.EqualTo(tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.EqualTo(8))

[<Test>]
let ``Validates Array.sampleWithReplacement`` () =
   let array = Array.init 5 id
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.sampleWithReplacement 8 array) tester
   Assert.That (next, Is.Not.EqualTo(tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.Not.EqualTo(1))

[<Test>]
let ``Validates Array.weightedSampleWithReplacement`` () =
   let array = Array.init 5 id
   let weight = Array.init (Array.length array) (id >> float >> ((+) 1.0))
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.weightedSampleWithReplacement 8 weight array) tester
   Assert.That (next, Is.Not.EqualTo(tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.Not.EqualTo(1))

[<Test>]
let ``Validates Array.shuffle`` () =
   let tester = getDefaultTester ()
   let array = Array.init 8 id
   let result, next = Random.next (Array.shuffle array) tester
   Assert.That (next, Is.Not.EqualTo(tester))
   Assert.That (System.Object.ReferenceEquals (result, array), Is.False)
   Assert.That (Array.length result, Is.EqualTo(Array.length array))
   Assert.That (Array.zip array result |> Array.forall (fun (x, y) -> x = y), Is.False)
   Assert.That (Array.sort result, Is.EquivalentTo(array))

[<Test>]
let ``Validates Array.shuffleInPlace`` () =
   let tester = getDefaultTester ()
   let array = Array.init 8 id
   let copied = Array.copy array
   let _, next = Random.next (Array.shuffleInPlace array) tester
   Assert.That (next, Is.Not.EqualTo(seed))
   Assert.That (Array.zip copied array |> Array.forall (fun (x, y) -> x = y), Is.False)
   Assert.That (Array.sort array, Is.EquivalentTo(copied))
