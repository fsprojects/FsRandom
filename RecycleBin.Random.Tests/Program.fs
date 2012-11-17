module RecycleBin.Random.Tests.Program

open System
open RecycleBin.Random
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Statistics

let n = 500
let level = 0.01
let seed = 123456789u, 362436069u, 521288629u, 88675123u
let rec generate f seed =
   seq {
      let r, s = f seed
      yield r
      yield! generate f s
   }
let sample f = generate f seed |> Seq.take n |> Seq.toList
let totalTestCount = ref 0
let successTestCount = ref 0

type TestResult = Accept | Reject
let printTestResult result =
   let original = Console.ForegroundColor
   match result with
      | Accept ->
         Console.ForegroundColor <- ConsoleColor.Green
         printfn "OK"
         incr successTestCount
      | Reject ->
         Console.ForegroundColor <- ConsoleColor.Red
         printfn "NG"
   Console.ForegroundColor <- original
   incr totalTestCount

[<Literal>]
let iteration = 1000
let ks x = 1.0 - 2.0 * (Seq.init iteration (fun i -> let k = float (i + 1) in (if i % 2 = 0 then 1.0 else -1.0) * exp (-2.0 * k * k * x * x)) |> Seq.sum)
let testContinuous generator cdf =
   let observed =
      let f seed =
         xorshift seed {
            let! u = generator
            return u
         }
      in sample f
   assert (n > 35)  // for approximation of dmax. TODO: implement KS statistic function.
   let empirical x = List.sumBy (fun t -> if t <= x then 1.0 / float n else 0.0) observed
   let epsilon = List.sort observed |> Seq.pairwise |> Seq.map (fun (a, b) -> b - a) |> Seq.min
   let diff x = empirical x - cdf x |> abs
   let d = observed |> List.collect (fun x -> [diff x; diff (x - epsilon)]) |> List.max
   printTestResult (if ks (sqrt (float n) * d) < level then Reject else Accept)

let testDiscrete generator cdf parameterCount =
   let observed =
      let f seed =
         xorshift seed {
            let! u = generator
            return u
         }
      in sample f
   let binCount = int <| ceil (2.0 * (float n ** 0.4))
   let histogram = Histogram(List.map float observed, binCount)
   let p =
      let nonEmptyCell = ref 0
      let mutable sum = 0.0
      for index = 0 to binCount - 1 do
         let bin = histogram.[index]
         let o = float bin.Count
         let e = float n * (cdf bin.UpperBound - cdf bin.LowerBound)
         sum <- sum + (o - e) ** 2.0 / e
         if bin.Count <> 0.0 then incr nonEmptyCell
      let df = !nonEmptyCell - (parameterCount + 1)
      ChiSquare(float df).CumulativeDistribution (sum)
   printTestResult (if p < level then Reject else Accept)

let testBinary generator cdf probability =
   let observed =
      let f seed =
         xorshift seed {
            let! u = generator
            return u
         }
      in sample f
   let o0, o1 = observed |> List.partition ((=) 0) |> (fun (zero, one) -> float (List.length zero), float (List.length one))
   let e0, e1 = let one = float n * probability in (float n - one, one)
   let chisq = (o0 - e0) ** 2.0 / e0 + (o1 - e1) ** 2.0 / e1
   let p = ChiSquare(1.0).CumulativeDistribution (chisq)
   printTestResult (if p < 0.01 then Reject else Accept)

let cdfUniform (a, b) = ContinuousUniform(a, b).CumulativeDistribution
let testUniform parameter =
   printf "Uniform (%.1f, %.1f)\t" <|| parameter
   let generator = uniform parameter
   let cdf = cdfUniform parameter
   testContinuous generator cdf
   
let cdfNormal (mean, sd) = Normal(mean, sd).CumulativeDistribution
let testNormal parameter =
   printf "Normal (%.1f, %.1f)\t" <|| parameter
   let generator = normal parameter
   let cdf = cdfNormal parameter
   testContinuous generator cdf
   
let cdfGamma (shape, scale) =
   // Gamma.CumulativeDistribution (x) (x < 0) throws an exception.
   let distribution = Gamma (shape, 1.0 / scale)
   fun x -> if x < 0.0 then 0.0 else distribution.CumulativeDistribution (x)
let testGamma parameter =
   printf "Gamma (%.1f, %.1f)\t" <|| parameter
   let generator = gamma parameter
   let cdf = cdfGamma parameter
   testContinuous generator cdf
   
let cdfExponential rate = Exponential(rate).CumulativeDistribution
let testExponential rate =
   printf "Exponential (%.1f)\t" rate
   let generator = exponential rate
   let cdf = cdfExponential rate
   testContinuous generator cdf
   
let cdfBeta (a, b) = Beta(a, b).CumulativeDistribution
let testBeta parameter =
   printf "Beta (%.1f, %.1f)\t" <|| parameter
   let generator = beta parameter
   let cdf = cdfBeta parameter
   testContinuous generator cdf
   
let cdfCauchy (location, scale) = Cauchy(location, scale).CumulativeDistribution
let testCauchy parameter =
   printf "Cauchy (%.1f, %.1f)\t" <|| parameter
   let generator = cauchy parameter
   let cdf = cdfCauchy parameter
   testContinuous generator cdf
   
let cdfChisq df = ChiSquare(float df).CumulativeDistribution
let testChiSquare df =
   printf "χ^2 (%d)\t" df
   let generator = chisquare df
   let cdf = cdfChisq df
   testContinuous generator cdf
   
let cdfT df = StudentT(0.0, 1.0, float df).CumulativeDistribution
let testT df =
   printf "t (%d)\t" df
   let generator = t df
   let cdf = cdfT df
   testContinuous generator cdf
   
let cdfUniformDiscrete (a, b) = DiscreteUniform(a, b).CumulativeDistribution
let testUniformDiscrete parameter =
   printf "Uniform (discrete) (%d, %d)\t" <|| parameter
   let generator = uniformDiscrete parameter
   let cdf = cdfUniformDiscrete parameter
   testDiscrete generator cdf 2
   
let cdfPoisson lambda = Poisson(lambda).CumulativeDistribution
let testPoisson lambda =
   printf "Poisson (%.1f)\t" lambda
   let generator = poisson lambda
   let cdf = cdfPoisson lambda
   testDiscrete generator cdf 1
   
let cdfGeometric p = Geometric(p).CumulativeDistribution
let testGeometric p =
   printf "Geometric (%.1f)\t" p
   let generator = geometric p
   let cdf = cdfGeometric p
   testDiscrete generator cdf 1
   
let cdfBernoulli p = Bernoulli(p).CumulativeDistribution
let testBernoulli p =
   printf "Bernoulli (%.1f)\t" p
   let generator = bernoulli p
   let cdf = cdfBernoulli p
   testBinary generator cdf p
   
let cdfBinomial (n, p) = Binomial(p, n).CumulativeDistribution
let testBinomial parameter =
   printf "Binomial (%d, %.1f)\t" <|| parameter
   let generator = binomial parameter
   let cdf = cdfBinomial parameter
   testDiscrete generator cdf 2
   
let testDirichlet parameter =
   NotImplementedException () |> raise

[<EntryPoint>]
let main argv =
   printfn "Number of samples: %d" n
   printfn "Seed: %A" seed
   printfn "Significance level: %.2f" level
   printfn "Kolmogorov-Smirnov tests"
   testUniform (-10.0, 10.0)
   testNormal (-5.0, 3.0)
   testGamma (0.3, 2.0)
   testGamma (5.6, 0.4)
   testGamma (3.0, 7.9)
   testExponential (1.5)
   testBeta (1.5, 0.4)
   testCauchy (-1.5, 0.1)
   testChiSquare (10)
   testT (3)
   printfn "Chi-square goodness-of-fit test"
   testUniformDiscrete (-10, 10)
   testPoisson (5.2)
   testGeometric (0.2)
   testBernoulli (0.7)
   testBinomial (20, 0.3)
   // To be implemented.
   // testDirichlet [1.0; 2.0; 2.5; 0.5]

   if !totalTestCount > 0
   then
      let success = !successTestCount
      let total = !totalTestCount
      let rate = float success / float total * 100.0
      printfn "%d out of %d tests (%.0f%%) succeeded." success total rate
   0
