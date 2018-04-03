[<AutoOpen>]
module internal FsRandom.RuntimeHelper

open System.IO
open MathNet.Numerics
open NUnit.Framework

let curry f x y = f (x, y)
let uncurry f (x, y) = f x y

// Kolmogorov-Smirnov test
let ksTest distribution samples =
   let samples = Array.sort samples
   let n = Array.length samples
   let f = distribution
   let Dn =
      samples
      |> Array.mapi (fun index x -> abs (f x - float (index + 1) / float n))
      |> Array.max
   let k = 1.628  // K-S distribution critical value (99%)
   Dn < k

// Chi-square goodness of fit test
let chisqTest distribution samples =
   let n = Array.length samples
   let f = distribution
   let samples = Seq.countBy id samples |> Seq.toArray |> Array.sortBy fst
   let chi2 =
      let sumP = samples |> Array.sumBy (fst >> f)
      let residual = 1.0 - sumP
      let sampled =
         samples
         |> Array.map (fun (k, obs) ->
            let npi = float n * f k
            pown (float obs - npi) 2 / npi
         )
         |> Array.sum
      if residual > 1.0e-6 then
         sampled + float n * residual
      else
         sampled
   chi2 < SpecialFunctions.GammaLowerRegularizedInv (float (n - 1) / 2.0, 0.99)

module KnownRandomSequence =
   let private getResourceString fileName =
      let path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", fileName)
      File.ReadAllText(path)

   let SFMT_11213_out = getResourceString "SFMT.11213.out.txt"
   let SFMT_1279_out = getResourceString "SFMT.1279.out.txt"
   let SFMT_132049_out = getResourceString "SFMT.132049.out.txt"
   let SFMT_19937_out = getResourceString "SFMT.19937.out.txt"
   let SFMT_216091_out = getResourceString "SFMT.216091.out.txt"
   let SFMT_2281_out = getResourceString "SFMT.2281.out.txt"
   let SFMT_4253_out = getResourceString "SFMT.4253.out.txt"
   let SFMT_44497_out = getResourceString "SFMT.44497.out.txt"
   let SFMT_607_out = getResourceString "SFMT.607.out.txt"
   let SFMT_86243_out = getResourceString "SFMT.86243.out.txt"
   let mt19937_64_out = getResourceString "mt19937-64.out.txt"
   let mt19937ar_out = getResourceString "mt19937ar.out.txt"
