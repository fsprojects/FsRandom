module RecycleBin.Random.Tests.Program

open System
open RecycleBin.Random
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Statistics

let n = 500
let level = 0.01
let xorshiftTester = xorshift, (123456789u, 362436069u, 521288629u, 88675123u)
let systemrandomSeed = 12345
let systemrandomTester = systemrandom, Random (systemrandomSeed)
let mersenneSeed = [| 0x123u; 0x234u; 0x345u; 0x456u |]
let mersenneTester = mersenne, StateVector.Initialize mersenneSeed
let rec generate f seed =
   seq {
      let r, s = f seed
      yield r
      yield! generate f s
   }
let sample f seed = generate f seed |> Seq.take n |> Seq.toList
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
let testContinuous (prng : 's -> RandomBuilder<'s>, seed) generator cdf =
   let observed =
      let f seed = prng seed { return! generator }
      in sample f seed
   let empirical x = List.sumBy (fun t -> if t <= x then 1.0 / float n else 0.0) observed
   let epsilon = List.sort observed |> Seq.pairwise |> Seq.map (fun (a, b) -> b - a) |> Seq.min |> ((*) 0.1)
   let diff x = empirical x - cdf x |> abs
   let d = observed |> List.collect (fun x -> [diff x; diff (x - epsilon)]) |> List.max
   printTestResult (if ks (sqrt (float n) * d) < level then Reject else Accept)

let testDiscrete (prng : 's -> RandomBuilder<'s>, seed) generator cdf parameterCount =
   let observed =
      let f seed = prng seed { return! generator }
      in sample f seed
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

let testBinary (prng : 's -> RandomBuilder<'s>, seed) generator cdf probability =
   let observed =
      let f seed = prng seed { return! generator }
      in sample f seed
   let o0, o1 = observed |> List.partition ((=) 0) |> (fun (zero, one) -> float (List.length zero), float (List.length one))
   let e0, e1 = let one = float n * probability in (float n - one, one)
   let chisq = (o0 - e0) ** 2.0 / e0 + (o1 - e1) ** 2.0 / e1
   let p = ChiSquare(1.0).CumulativeDistribution (chisq)
   printTestResult (if p < 0.01 then Reject else Accept)

let cdfUniform (a, b) = ContinuousUniform(a, b).CumulativeDistribution
let testUniform tester parameter =
   printf "Uniform (%.1f, %.1f)\t" <|| parameter
   let generator = uniform parameter
   let cdf = cdfUniform parameter
   testContinuous tester generator cdf
   
let cdfNormal (mean, sd) = Normal(mean, sd).CumulativeDistribution
let testNormal tester parameter =
   printf "Normal (%.1f, %.1f)\t" <|| parameter
   let generator = normal parameter
   let cdf = cdfNormal parameter
   testContinuous tester generator cdf
   
let cdfGamma (shape, scale) =
   // Gamma.CumulativeDistribution (x) (x < 0) throws an exception.
   let distribution = Gamma (shape, 1.0 / scale)
   fun x -> if x < 0.0 then 0.0 else distribution.CumulativeDistribution (x)
let testGamma tester parameter =
   printf "Gamma (%.1f, %.1f)\t" <|| parameter
   let generator = gamma parameter
   let cdf = cdfGamma parameter
   testContinuous tester generator cdf
   
let cdfExponential rate = Exponential(rate).CumulativeDistribution
let testExponential tester rate =
   printf "Exponential (%.1f)\t" rate
   let generator = exponential rate
   let cdf = cdfExponential rate
   testContinuous tester generator cdf
   
let cdfBeta (a, b) = Beta(a, b).CumulativeDistribution
let testBeta tester parameter =
   printf "Beta (%.1f, %.1f)\t" <|| parameter
   let generator = beta parameter
   let cdf = cdfBeta parameter
   testContinuous tester generator cdf
   
let cdfCauchy (location, scale) = Cauchy(location, scale).CumulativeDistribution
let testCauchy tester parameter =
   printf "Cauchy (%.1f, %.1f)\t" <|| parameter
   let generator = cauchy parameter
   let cdf = cdfCauchy parameter
   testContinuous tester generator cdf
   
let cdfChisq df = ChiSquare(float df).CumulativeDistribution
let testChiSquare tester df =
   printf "χ^2 (%d)\t" df
   let generator = chisquare df
   let cdf = cdfChisq df
   testContinuous tester generator cdf
   
let cdfT df = StudentT(0.0, 1.0, float df).CumulativeDistribution
let testT tester df =
   printf "t (%d)\t" df
   let generator = t df
   let cdf = cdfT df
   testContinuous tester generator cdf
   
let cdfUniformDiscrete (a, b) = DiscreteUniform(a, b).CumulativeDistribution
let testUniformDiscrete tester parameter =
   printf "Uniform (discrete) (%d, %d)\t" <|| parameter
   let generator = uniformDiscrete parameter
   let cdf = cdfUniformDiscrete parameter
   testDiscrete tester generator cdf 2
   
let cdfPoisson lambda = Poisson(lambda).CumulativeDistribution
let testPoisson tester lambda =
   printf "Poisson (%.1f)\t" lambda
   let generator = poisson lambda
   let cdf = cdfPoisson lambda
   testDiscrete tester generator cdf 1
   
let cdfGeometric p = Geometric(p).CumulativeDistribution
let testGeometric tester p =
   printf "Geometric (%.1f)\t" p
   let generator = geometric p
   let cdf = cdfGeometric p
   testDiscrete tester generator cdf 1
   
let cdfBernoulli p = Bernoulli(p).CumulativeDistribution
let testBernoulli tester p =
   printf "Bernoulli (%.1f)\t" p
   let generator = bernoulli p
   let cdf = cdfBernoulli p
   testBinary tester generator cdf p
   
let cdfBinomial (n, p) = Binomial(p, n).CumulativeDistribution
let testBinomial tester parameter =
   printf "Binomial (%d, %.1f)\t" <|| parameter
   let generator = binomial parameter
   let cdf = cdfBinomial parameter
   testDiscrete tester generator cdf 2
   
let testDirichlet tester parameter =
   NotImplementedException () |> raise

let test tester name seed =
   printfn "========================================"
   printfn "Test %s" name
   printfn "Seed: %A" seed
   printfn "----------------------------------------"
   printfn "Kolmogorov-Smirnov tests"
   testUniform tester (-10.0, 10.0)
   testNormal tester (-5.0, 3.0)
   testGamma tester (0.3, 2.0)
   testGamma tester (5.6, 0.4)
   testGamma tester (3.0, 7.9)
   testExponential tester (1.5)
   testBeta tester (1.5, 0.4)
   testCauchy tester (-1.5, 0.1)
   testChiSquare tester (10)
   testT tester (3)
   printfn "Chi-square goodness-of-fit test"
   testUniformDiscrete tester (-10, 10)
   testPoisson tester (5.2)
   testGeometric tester (0.2)
   testBernoulli tester (0.7)
   testBinomial tester (20, 0.3)
   // To be implemented.
   // testDirichlet tester [1.0; 2.0; 2.5; 0.5]

[<EntryPoint>]
let main argv =
   printfn "Number of samples: %d" n
   printfn "Significance level: %.2f" level
   test xorshiftTester "xorshift" (snd xorshiftTester)
   test systemrandomTester "systemrandom (System.Random)" systemrandomSeed
   test mersenneTester "mersenne" mersenneSeed
   
   if !totalTestCount > 0
   then
      let success = !successTestCount
      let total = !totalTestCount
      let rate = float success / float total * 100.0
      printfn "========================================"
      printfn "%d out of %d tests (%.0f%%) succeeded." success total rate
   0
