module RecycleBin.Random.MersenneTwisterTest

open System
open System.IO
open RecycleBin.Random.MersenneTwister
open RecycleBin.Random.Tests.Resources
open NUnit.Framework

[<Test>]
let ``Checks first 2,000 output`` () =
   let expected =
      use reader = new StringReader (KnownRandomSequence.mt19937ar_out)
      reader.ReadLine () |> ignore
      let raws = Array.zeroCreate 1000
      for i = 0 to 199 do
         let line = reader.ReadLine ()
         let values = line.Split ([|' '|], StringSplitOptions.RemoveEmptyEntries)
         for j = 0 to Array.length values - 1 do
            raws.[5 * i + j] <- values.[j]
      reader.ReadLine () |> ignore
      reader.ReadLine () |> ignore
      let standards = Array.zeroCreate 1000
      for i = 0 to 199 do
         let line = reader.ReadLine ()
         let values = line.Split ([|' '|], StringSplitOptions.RemoveEmptyEntries)
         for j = 0 to Array.length values - 1 do
            standards.[5 * i + j] <- values.[j]
      raws, standards
   let actual =
      let seed = StateVector.Initialize [|0x123u; 0x234u; 0x345u; 0x456u|]
      mersenne {
         let raws = Array.zeroCreate 1000
         for index = 0 to Array.length raws - 1 do
            let! u = raw
            raws.[index] <- (sprintf "%10u" u).Trim ()
         let standards = Array.zeroCreate 1000
         for index = 0 to Array.length standards - 1 do
            let! u = ``[0, 1)``
            standards.[index] <- (sprintf "%10.8f" u).Trim ()
         return raws, standards
      }
      <| seed
      |> fst
   Assert.That (fst actual, Is.EquivalentTo(fst expected))
   Assert.That (snd actual, Is.EquivalentTo(snd expected))
