module FsRandom.StatisticsTest

open FsRandom.Statistics
open FsUnit
open NUnit.Framework
open RDotNet

let p = 0.01
let n = 5000
let getSamples g = Seq.ofRandom g Utility.defaultState |> Seq.take n |> Seq.toArray

[<TearDown>]
let tearDown () =
   let engine = getREngine ()
   engine.Evaluate ("""rm(list=ls())""") |> ignore

[<Test>]
let ``Validates uniform`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (uniform (-10.0, 10.0))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "punif", min=-10, max=10)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates loguniform`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (loguniform (1.0, 100.0))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   use plogunif = engine.Evaluate ("""plogunif <- function(x, min, max) {
      if (x < min) {
         0
      } else if (min <= x && x <= max) {
         log(x) / (log(max) - log(min))
      } else {
         1
      }
   }""")
   engine.Evaluate ("""ks.test(x, "plogunif", min=1, max=100)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates triangular`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (triangular (-3.3, 10.7, 2.1))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   loadPackage "triangle"
   engine.Evaluate ("""ks.test(x, "ptriangle", a=-3.3, b=10.7, c=2.1)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates normal`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (normal (-5.0, 3.0))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pnorm", mean=-5, sd=3)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates lognormal`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (lognormal (3.1, 7.2))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "plnorm", meanlog=3.1, sdlog=7.2)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates gamma (shape < 1)`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (gamma (0.3, 2.0))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pgamma", shape=0.3, scale=2)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates gamma (shape > 1)`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (gamma (5.6, 0.4))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pgamma", shape=5.6, scale=0.4)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates gamma (shape is integer)`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (gamma (3.0, 7.9))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pgamma", shape=3, scale=7.9)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates exponential`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (exponential (1.5))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pexp", rate=1.5)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates weibull`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (weibull (6.1, 1.4))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pweibull", shape=6.1, scale=1.4)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates gumbel`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (gumbel (-3.0, 2.3))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   use pgumbel = engine.Evaluate ("""pgumbel <- function(x, mu, eta) exp(-exp(-(x - mu) / eta))""")
   engine.Evaluate ("""ks.test(x, "pgumbel", mu=-3, eta=2.3)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates beta`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (beta (1.5, 0.4))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pbeta", shape1=1.5, shape2=0.4)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates cauchy`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (cauchy (-1.5, 0.1))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pcauchy", location=-1.5, scale=0.1)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates chisquare`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (chisquare (10))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pchisq", df=10)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates studentT`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (studentT (3))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""ks.test(x, "pt", df=3)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates vonMises`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (vonMises (1.0, 2.0))
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   dvonMises <- function(x, mu, kappa) {
      if (x < -pi || pi < x) {
         0
      } else {
         i <- integrate(function(t) exp(kappa * cos(t)), 0, pi)$value
         exp(kappa * cos(x - mu)) / (2 * i)
      }
   }
   pvonMises <- function(x, mu, kappa) {
      integrate(dvonMises, -pi, x, mu=mu, kappa=kappa)$value
   }
   pvonMises <- Vectorize(pvonMises, "x")
   ks.test(x, "pvonMises", mu=1, kappa=2)""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates uniformDiscrete`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (uniformDiscrete (-10, 10))
      engine.CreateIntegerVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   x <- table(x)
   p <- rep(1/21, 21L)
   if (length(x) == 21L) {
      chisq.test(x, p=p)
   } else {
      chisq.test(c(x, rep(0L, 21L - length(x))), p=p)
   }""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates poisson`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (poisson (5.2))
      engine.CreateIntegerVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   x <- table(x)
   p <- dpois(as.integer(names(x)), 5.2)
   chisq.test(c(x, 0L), p=c(p, 1 - sum(p)))""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates geometric0`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (geometric0 (0.2))
      engine.CreateIntegerVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   x <- table(x)
   p <- dgeom(as.integer(names(x)), 0.2)
   if (abs(1 - sum(p)) > .Machine$double.eps) {
      chisq.test(c(x, 0L), p=c(p, 1 - sum(p)))
   } else {
      chisq.test(x, p=p)
   }""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates geometric1`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (geometric1 (0.7))
      engine.CreateIntegerVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   x <- table(x)
   p <- dgeom(as.integer(names(x)) - 1L, 0.7)
   if (abs(1 - sum(p)) > .Machine$double.eps) {
      chisq.test(c(x, 0L), p=c(p, 1 - sum(p)))
   } else {
      chisq.test(x, p=p)
   }""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates bernoulli`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (bernoulli (0.7))
      engine.CreateIntegerVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   x <- table(x)
   p <- dbinom(as.integer(names(x)), 1L, 0.7)
   if (length(x) == 2L) {
      chisq.test(x, p=p)
   } else {
      chisq.test(c(x, 0L), p=c(p, 1 - sum(p)))
   }""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``Validates binomial`` () =
   let engine = getREngine ()
   use samples =
      let samples = getSamples (binomial (20, 0.3))
      engine.CreateIntegerVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   x <- table(x)
   p <- dbinom(as.integer(names(x)), 20L, 0.3)
   if (length(x) == 21L) {
      chisq.test(x, p=p)
   } else {
      chisq.test(c(x, 0L), p=c(p, 1 - sum(p)))
   }""")
   |> getP
   |> should be (greaterThan p)

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
   let engine = getREngine ()
   use samples =
      let samples = getSamples (mix [gamma (3.0, 2.0), 1.0; normal (-2.0, 1.0), 3.0])
      engine.CreateNumericVector (samples)
   engine.SetSymbol ("x", samples)
   engine.Evaluate ("""
   pmix <- function(x) 0.25 * pgamma(x, shape=3, scale=2) + 0.75 * pnorm(x, mean=-2, sd=1)
   ks.test(x, "pmix")""")
   |> getP
   |> should be (greaterThan p)

[<Test>]
let ``uniformDiscrete on full-range of int generates both positive and negative values`` () =
   let g = uniformDiscrete (System.Int32.MinValue, System.Int32.MaxValue)
   let values = Random.get <| List.randomCreate 10 g <| Utility.defaultState
   List.exists (fun x -> x > 0) values |> should be True
   List.exists (fun x -> x < 0) values |> should be True
