module FsRandom.StatisticsTest

open FsRandom.Statistics
open FsUnit
open MathNet.Numerics
open MathNet.Numerics.Distributions
open NUnit.Framework

let n = 5000
let getSamples g = Seq.ofRandom g Utility.defaultState |> Seq.take n |> Seq.toArray

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

[<TearDown>]
let tearDown () =
   ()

[<Test>]
let ``Validates uniform`` () =
   let distribution = ContinuousUniform (-10.0, 10.0)
   getSamples (uniform (-10.0, 10.0))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates loguniform`` () =
   let cdf (a, b) x = 1.0 / (log b - log a) * log x
   getSamples (loguniform (1.0, 100.0))
   |> ksTest (cdf (1.0, 100.0))
   |> should be True

[<Test>]
let ``Validates triangular`` () =
   let distribution = Triangular (-3.3, 10.7, 2.1)
   getSamples (triangular (-3.3, 10.7, 2.1))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates normal`` () =
   let distribution = Normal (-5.0, 3.0)
   getSamples (normal (-5.0, 3.0))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates lognormal`` () =
   let distribution = LogNormal (3.1, 7.2)
   getSamples (lognormal (3.1, 7.2))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates gamma (shape < 1)`` () =
   let distribution = Gamma (0.3, 1.0 / 2.0)
   getSamples (gamma (0.3, 2.0))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates gamma (shape > 1)`` () =
   let distribution = Gamma (5.6, 1.0 / 0.4)
   getSamples (gamma (5.6, 0.4))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates gamma (shape is integer)`` () =
   let distribution = Gamma (3.0, 1.0 / 7.9)
   getSamples (gamma (3.0, 7.9))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates exponential`` () =
   let distribution = Exponential (1.5)
   getSamples (exponential (1.5))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates weibull`` () =
   let distribution = Weibull (6.1, 1.4)
   getSamples (weibull (6.1, 1.4))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates gumbel`` () =
   let cdf (mu, beta) x = exp <| -exp (-(x - mu) / beta)
   getSamples (gumbel (6.1, 1.4))
   |> ksTest (cdf (6.1, 1.4))
   |> should be True

[<Test>]
let ``Validates beta`` () =
   let distribution = Beta (1.5, 0.4)
   getSamples (beta (1.5, 0.4))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates cauchy`` () =
   let distribution = Cauchy (-1.5, 0.1)
   getSamples (cauchy (-1.5, 0.1))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates chisquare`` () =
   let distribution = ChiSquared (10.0)
   getSamples (chisquare (10))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

[<Test>]
let ``Validates studentT`` () =
   let distribution = StudentT (0.0, 1.0, 3.0)
   getSamples (studentT (3))
   |> ksTest distribution.CumulativeDistribution
   |> should be True

// CDF is unknown
//[<Test>]
//let ``Validates vonMises`` () =
//   ()

[<Test>]
let ``Validates uniformDiscrete`` () =
   let distribution = DiscreteUniform (-10, 10)
   getSamples (uniformDiscrete (-10, 10))
   |> chisqTest distribution.Probability
   |> should be True

[<Test>]
let ``Validates poisson`` () =
   let distribution = Poisson (5.2)
   getSamples (poisson (5.2))
   |> chisqTest distribution.Probability
   |> should be True

[<Test>]
let ``Validates geometric0`` () =
   let distribution = Geometric (0.2)
   getSamples (geometric0 (0.2))
   |> chisqTest (fun x -> distribution.Probability (x + 1))  // Math.NET Numerics' Geometric supports [1, 2, ...]
   |> should be True

[<Test>]
let ``Validates geometric1`` () =
   let distribution = Geometric (0.7)
   getSamples (geometric1 (0.7))
   |> chisqTest distribution.Probability
   |> should be True

[<Test>]
let ``Validates bernoulli`` () =
   let distribution = Bernoulli (0.4)
   getSamples (bernoulli (0.4))
   |> chisqTest distribution.Probability
   |> should be True

[<Test>]
let ``Validates binomial`` () =
   let distribution = Binomial (0.3, 20)
   getSamples (binomial (20, 0.3))
   |> chisqTest distribution.Probability
   |> should be True

// TODO: implement
//[<Test>]
//let ``Validates dirichlet`` () =
//   testDirichlet Utility.defaultState [1.0; 2.0; 2.5; 0.5]
//
// TODO: implement
//[<Test>]
//let ``Validates multinomial`` () =
//   testMultinomial Utility.defaultState [1.0; 2.0; 2.5; 0.5]

[<Test>]
let ``wishart returns positive and positive semidefinite matrices`` () =
   let sigma = Array2D.init 3 3 (fun i j ->
      match i, j with
         | 0, 0 -> 1.0
         | 1, 1 -> 1.0
         | 2, 2 -> 4.0
         | 0, 1 | 1, 0 -> 0.7
         | 0, 2 | 2, 0 -> -1.0
         | 1, 2 | 2, 1 -> 0.0
         | _ -> failwith "never"
      )
   let samples =
      Seq.ofRandom (wishart (4, sigma)) Utility.defaultState
      |> Seq.take 1000
      |> Seq.toList
   samples
   |> List.forall (fun m -> Seq.forall2 (fun i j -> m.[i, j] > 0.0) [0..2] [0..2])
   |> should be True
   samples
   |> List.map (Matrix.jacobi >> fst)
   |> List.forall (Array.forall (fun x -> x >= 0.0))
   |> should be True

[<Test>]
let ``Validates mix (float)`` () =
   let cdf x =
      let gamma = Gamma (3.0, 1.0 / 2.0)
      let normal = Normal (-2.0, 1.0)
      if x <= 0.0 then
         0.75 * normal.CumulativeDistribution (x)
      else
         0.25 * gamma.CumulativeDistribution (x) + 0.75 * normal.CumulativeDistribution (x)
   getSamples (mix [gamma (3.0, 2.0), 1.0; normal (-2.0, 1.0), 3.0])
   |> ksTest cdf
   |> should be True

[<Test>]
let ``uniformDiscrete on full-range of int generates both positive and negative values`` () =
   let g = uniformDiscrete (System.Int32.MinValue, System.Int32.MaxValue)
   let values = Random.get <| List.randomCreate 10 g <| Utility.defaultState
   List.exists (fun x -> x > 0) values |> should be True
   List.exists (fun x -> x < 0) values |> should be True
