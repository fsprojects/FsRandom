[<AutoOpen>]
module internal FsRandom.RuntimeHelper

open MathNet.Numerics

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
