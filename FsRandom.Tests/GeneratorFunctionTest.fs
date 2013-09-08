module FsRandom.Tests.GeneratorFunctionTest

open System
open FsRandom
open FsRandom.Statistics
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Statistics
open NUnit.Framework

let n = 1000
let level = 0.01
let rec generate f seed =
   seq {
      let (_, s) as r = f seed
      yield r
      yield! generate f s
   }
let sample f seed = generate f seed |> Seq.take n |> Seq.toList |> (fun l -> List.map fst l, Seq.last l |> snd)

[<Literal>]
let iteration = 1000
let ks x = 1.0 - 2.0 * (Seq.init iteration (fun i -> let k = float (i + 1) in (if i % 2 = 0 then 1.0 else -1.0) * exp (-2.0 * k * k * x * x)) |> Seq.sum)
let testContinuous tester generator cdf =
   let observed = Seq.ofRandom generator tester |> Seq.take n |> Seq.toList
   let empirical x = List.sumBy (fun t -> if t <= x then 1.0 / float n else 0.0) observed
   let epsilon = List.sort observed |> Seq.pairwise |> Seq.map (fun (a, b) -> b - a) |> Seq.min |> ((*) 0.1)
   let diff x = empirical x - cdf x |> abs
   let d = observed |> List.collect (fun x -> [diff x; diff (x - epsilon)]) |> List.max
   Assert.That (ks (sqrt (float n) * d), Is.GreaterThanOrEqualTo(level))

let testDiscrete tester generator cdf parameterCount =
   let observed = Seq.ofRandom generator tester |> Seq.take n |> Seq.toList
   let binCount = int <| ceil (2.0 * (float n ** 0.4))
   let histogram = Histogram(List.map float observed, binCount)
   let p =
      let nonEmptyCell = ref 0
      let mutable sum = 0.0
      for index = 0 to binCount - 1 do
         let bin = histogram.[index]
         if floor bin.UpperBound <> floor bin.LowerBound  // ensures at least one integer exists in the bin
         then
            let o = float bin.Count
            let e = float n * (cdf bin.UpperBound - cdf bin.LowerBound)
            sum <- sum + (o - e) ** 2.0 / e
            if bin.Count <> 0.0 then incr nonEmptyCell
      let df = !nonEmptyCell - (parameterCount + 1)
      ChiSquare(float df).CumulativeDistribution (sum)
   Assert.That (p, Is.GreaterThanOrEqualTo(level))

let testBinary tester generator cdf probability =
   let observed = Seq.ofRandom generator tester |> Seq.take n |> Seq.toList
   let o0, o1 = observed |> List.partition ((=) 0) |> (fun (zero, one) -> float (List.length zero), float (List.length one))
   let e0, e1 = let one = float n * probability in (float n - one, one)
   let chisq = (o0 - e0) ** 2.0 / e0 + (o1 - e1) ** 2.0 / e1
   let p = ChiSquare(1.0).CumulativeDistribution (chisq)
   Assert.That (p, Is.GreaterThanOrEqualTo(level))
   
let cdfUniform (a, b) = ContinuousUniform(a, b).CumulativeDistribution
let testUniform tester parameter =
   let generator = uniform parameter
   let cdf = cdfUniform parameter
   testContinuous tester generator cdf
   
let cdfLoguniform (a, b) x =
   if x < a
   then
      0.0
   elif a <= x && x <= b
   then
      1.0 / (x * (log b - log a))
   else
      1.0
let testLoguniform tester parameter =
   let generator = loguniform parameter
   let cdf = cdfLoguniform parameter
   testContinuous tester generator cdf
   
let cdfTriangular (a, b, c) x =
   if x < a
   then
      0.0
   elif a <= x && x <= b
   then
      if x < c
      then
         (x - a) * (x - a) / ((b - a) * (c - a))
      else
         1.0 - (b - x) * (b - x) / ((b - a) * (b - c))
   else
      1.0
let testTriangular tester parameter =
   let generator = triangular parameter
   let cdf = cdfTriangular parameter
   testContinuous tester generator cdf
   
let cdfNormal (mean, sd) = Normal(mean, sd).CumulativeDistribution
let testNormal tester parameter =
   let generator = normal parameter
   let cdf = cdfNormal parameter
   testContinuous tester generator cdf
   
let cdfLognormal (mu, sigma) = LogNormal(mu, sigma).CumulativeDistribution
let testLognormal tester parameter =
   let generator = lognormal parameter
   let cdf = cdfLognormal parameter
   testContinuous tester generator cdf
   
let cdfGamma (shape, scale) =
   // Gamma.CumulativeDistribution (x) (x < 0) throws an exception.
   let distribution = Gamma (shape, 1.0 / scale)
   fun x -> if x < 0.0 then 0.0 else distribution.CumulativeDistribution (x)
let testGamma tester parameter =
   let generator = gamma parameter
   let cdf = cdfGamma parameter
   testContinuous tester generator cdf
   
let cdfExponential rate = Exponential(rate).CumulativeDistribution
let testExponential tester rate =
   let generator = exponential rate
   let cdf = cdfExponential rate
   testContinuous tester generator cdf
   
let cdfWeibull (shape, scale) = Weibull(shape, scale).CumulativeDistribution
let testWeibull tester parameter =
   let generator = weibull parameter
   let cdf = cdfWeibull parameter
   testContinuous tester generator cdf
   
let cdfGumbel (location, scale) = fun x -> exp (-exp (-(x - location) / scale))
let testGumbel tester parameter =
   let generator = gumbel parameter
   let cdf = cdfGumbel parameter
   testContinuous tester generator cdf
   
let cdfBeta (a, b) = Beta(a, b).CumulativeDistribution
let testBeta tester parameter =
   let generator = beta parameter
   let cdf = cdfBeta parameter
   testContinuous tester generator cdf
   
let cdfCauchy (location, scale) = Cauchy(location, scale).CumulativeDistribution
let testCauchy tester parameter =
   let generator = cauchy parameter
   let cdf = cdfCauchy parameter
   testContinuous tester generator cdf
   
let cdfChisq df = ChiSquare(float df).CumulativeDistribution
let testChiSquare tester df =
   let generator = chisquare df
   let cdf = cdfChisq df
   testContinuous tester generator cdf
   
let cdfT df = StudentT(0.0, 1.0, float df).CumulativeDistribution
let testT tester df =
   let generator = t df
   let cdf = cdfT df
   testContinuous tester generator cdf
   
let cdfUniformDiscrete (a, b) = DiscreteUniform(a, b).CumulativeDistribution
let testUniformDiscrete tester parameter =
   let generator = uniformDiscrete parameter
   let cdf = cdfUniformDiscrete parameter
   testDiscrete tester generator cdf 2
   
let cdfPoisson lambda = Poisson(lambda).CumulativeDistribution
let testPoisson tester lambda =
   let generator = poisson lambda
   let cdf = cdfPoisson lambda
   testDiscrete tester generator cdf 1
   
let cdfGeometric p = Geometric(p).CumulativeDistribution
let testGeometric tester p =
   let generator = geometric p
   let cdf = cdfGeometric p
   testDiscrete tester generator cdf 1
   
let cdfBernoulli p = Bernoulli(p).CumulativeDistribution
let testBernoulli tester p =
   let generator = bernoulli p
   let cdf = cdfBernoulli p
   testBinary tester generator cdf p
   
let cdfBinomial (n, p) = Binomial(p, n).CumulativeDistribution
let testBinomial tester parameter =
   let generator = binomial parameter
   let cdf = cdfBinomial parameter
   testDiscrete tester generator cdf 2
   
let testDirichlet tester parameter =
   Assert.Inconclusive ("Not implemented.")
   
let testMultinomial tester parameter =
   Assert.Inconclusive ("Not implemented.")

let testFlipCoin tester p =
   let generator = Random.transformBy (fun b -> if b then 1 else 0) <| Utility.flipCoin p
   let cdf = cdfBernoulli p
   testBinary tester generator cdf p

[<Test>]
let ``Validates uniform`` () =
   testUniform (getDefaultTester ()) (-10.0, 10.0)

[<Test>]
let ``Validates loguniform`` () =
   testUniform (getDefaultTester ()) (1.0, 100.0)

[<Test>]
let ``Validates triangular`` () =
   testTriangular (getDefaultTester ()) (-3.3, 10.7, 2.1)

[<Test>]
let ``Validates normal (-5.0, 3.0)`` () =
   testNormal (getDefaultTester ()) (-5.0, 3.0)

[<Test>]
let ``Validates lognormal`` () =
   testLognormal (getDefaultTester ()) (3.1, 7.2)

[<Test>]
let ``Validates gamma (shape < 1)`` () =
   testGamma (getDefaultTester ()) (0.3, 2.0)

[<Test>]
let ``Validates gamma (shape > 1)`` () =
   testGamma (getDefaultTester ()) (5.6, 0.4)

[<Test>]
let ``Validates gamma (shape is integer)`` () =
   testGamma (getDefaultTester ()) (3.0, 7.9)

[<Test>]
let ``Validates exponential`` () =
   testExponential (getDefaultTester ()) (1.5)

[<Test>]
let ``Validates weibull`` () =
   testWeibull (getDefaultTester ()) (6.1, 1.4)

[<Test>]
let ``Validates gumbel`` () =
   testGumbel (getDefaultTester ()) (-3.0, 2.3)

[<Test>]
let ``Validates beta`` () =
   testBeta (getDefaultTester ()) (1.5, 0.4)

[<Test>]
let ``Validates cauchy`` () =
   testCauchy (getDefaultTester ()) (-1.5, 0.1)

[<Test>]
let ``Validates chisquare`` () =
   testChiSquare (getDefaultTester ()) (10)

[<Test>]
let ``Validates t`` () =
   testT (getDefaultTester ()) (3)

[<Test>]
let ``Validates uniformDiscrete`` () =
   testUniformDiscrete (getDefaultTester ()) (-10, 10)

[<Test>]
let ``Validates poisson`` () =
   testPoisson (getDefaultTester ()) (5.2)

[<Test>]
let ``Validates geometric`` () =
   testGeometric (getDefaultTester ()) (0.2)

[<Test>]
let ``Validates bernoulli`` () =
   testBernoulli (getDefaultTester ()) (0.7)

[<Test>]
let ``Validates binomial`` () =
   testBinomial (getDefaultTester ()) (20, 0.3)

[<Test>]
let ``Validates dirichlet`` () =
    testDirichlet (getDefaultTester ()) [1.0; 2.0; 2.5; 0.5]

[<Test>]
let ``Validates multinomial`` () =
    testMultinomial (getDefaultTester ()) [1.0; 2.0; 2.5; 0.5]

[<Test>]
let ``Validates flipCoin`` () =
   testFlipCoin (getDefaultTester ()) (0.2)

[<Test>]
let ``Validates choose`` () =
   let n = 4
   let tester = getDefaultTester ()
   let result, next = Random.next (Utility.choose 10 n) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (List.length result, Is.EqualTo(n))
   Assert.That (List.forall (fun x -> List.exists ((=) x) [0..9]) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.EqualTo(n))

[<Test>]
let ``Validates Array.sample`` () =
   let array = Array.init 10 id
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.sample 8 array) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.EqualTo(8))

[<Test>]
let ``Validates Array.weightedSample`` () =
   let array = Array.init 10 id
   let weight = Array.init (Array.length array) (id >> float >> ((+) 1.0))
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.weightedSample 8 weight array) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.EqualTo(8))

[<Test>]
let ``Validates Array.sampleWithReplacement`` () =
   let array = Array.init 5 id
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.sampleWithReplacement 8 array) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.Not.EqualTo(1))

[<Test>]
let ``Validates Array.weightedSampleWithReplacement`` () =
   let array = Array.init 5 id
   let weight = Array.init (Array.length array) (id >> float >> ((+) 1.0))
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.weightedSampleWithReplacement 8 weight array) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   Assert.That (Array.forall (fun x -> Array.exists ((=) x) array) result, Is.True)
   Assert.That (Seq.length (Seq.distinct result), Is.Not.EqualTo(1))

[<Test>]
let ``Validates Array.randomCreate`` () =
   let tester = getDefaultTester ()
   let result, next = Random.next (Array.randomCreate 8 ``[0, 1)``) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (Array.length result, Is.EqualTo(8))
   let head = result.[0]
   Assert.That (Array.forall ((=) head) result, Is.False)

[<Test>]
let ``Validates Array.randomInit`` () =
   let tester = getDefaultTester ()
   let expected =
      Random.get (Array.randomCreate 8 (normal (0.0, 1.0))) tester
      |> Array.mapi (fun index z -> z + float index)
   let actual = Random.get (Array.randomInit 8 (fun index -> normal (float index, 1.0))) tester
   Assert.That (actual, Is.EqualTo(expected))

[<Test>]
let ``Validates Array.shuffle`` () =
   let tester = getDefaultTester ()
   let array = Array.init 8 id
   let result, next = Random.next (Array.shuffle array) tester
   Assert.That (snd next, Is.Not.EqualTo(snd tester))
   Assert.That (Object.ReferenceEquals (result, array), Is.False)
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
