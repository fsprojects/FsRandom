module FsRandom.SimdOrientedFastMersenneTwisterTest
()
open System
open System.IO
open FsRandom.SimdOrientedFastMersenneTwister
open FsRandom.Tests.Resources
open NUnit.Framework

let test parameter resource =
   let expected =
      use reader = new StringReader (resource)
      reader.ReadLine () |> ignore
      reader.ReadLine () |> ignore
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
      let seedByInt = StateVector.Initialize (parameter, 1234u)
      let randomByInt =
         Random.get
         <| random {
            let raws = Array.zeroCreate 1000
            for index = 0 to Array.length raws / 2 - 1 do
               let! u = rawBits
               raws.[2 * index] <- (sprintf "%10u" (uint32 (u &&& 0xFFFFFFFFFFFFFFFFuL))).Trim ()
               raws.[2 * index + 1] <- (sprintf "%10u" (uint32 ((u >>> 32) &&& 0xFFFFFFFFFFFFFFFFuL))).Trim ()
            return raws
         }
         <| createState sfmt seedByInt
      let seedByArray = StateVector.Initialize (parameter, [|0x1234u; 0x5678u; 0x9ABCu; 0xDEF0u|])
      let randomByArray =
         Random.get
         <| random {
            let raws = Array.zeroCreate 1000
            for index = 0 to Array.length raws / 2 - 1 do
               let! u = rawBits
               raws.[2 * index] <- (sprintf "%10u" (uint32 (u &&& 0xFFFFFFFFFFFFFFFFuL))).Trim ()
               raws.[2 * index + 1] <- (sprintf "%10u" (uint32 ((u >>> 32) &&& 0xFFFFFFFFFFFFFFFFuL))).Trim ()
            return raws
         }
         <| createState sfmt seedByArray
      randomByInt, randomByArray
   Assert.That (fst actual, Is.EquivalentTo(fst expected))
   Assert.That (snd actual, Is.EquivalentTo(snd expected))

[<Test>]
let ``Checks params 607 output`` () =
   test SfmtParams.Params607 KnownRandomSequence.SFMT_607_out

[<Test>]
let ``Checks params 1279 output`` () =
   test SfmtParams.Params1279 KnownRandomSequence.SFMT_1279_out

[<Test>]
let ``Checks params 2281 output`` () =
   test SfmtParams.Params2281 KnownRandomSequence.SFMT_2281_out

[<Test>]
let ``Checks params 4253 output`` () =
   test SfmtParams.Params4253 KnownRandomSequence.SFMT_4253_out

[<Test>]
let ``Checks params 11213 output`` () =
   test SfmtParams.Params11213 KnownRandomSequence.SFMT_11213_out

[<Test>]
let ``Checks params 19937 output`` () =
   test SfmtParams.Params19937 KnownRandomSequence.SFMT_19937_out

[<Test>]
let ``Checks params 44497 output`` () =
   test SfmtParams.Params44497 KnownRandomSequence.SFMT_44497_out

[<Test>]
let ``Checks params 86243 output`` () =
   test SfmtParams.Params86243 KnownRandomSequence.SFMT_86243_out

[<Test>]
let ``Checks params 132049 output`` () =
   test SfmtParams.Params132049 KnownRandomSequence.SFMT_132049_out

[<Test>]
let ``Checks params 216091 output`` () =
   test SfmtParams.Params216091 KnownRandomSequence.SFMT_216091_out
