/// <summary>
/// Provides generator functions related to statistical random distributions.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Statistics

/// <summary>
/// Returns a random number distributed uniformly in the range of [<paramref name="min" />, <paramref name="max" />].
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
[<CompiledName("Uniform")>]
val uniform : min:float * max:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed log-uniformly in the range of [<paramref name="min" />, <paramref name="max" />].
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
[<CompiledName("LogUniform")>]
val loguniform : min:float * max:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed triangular.
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
/// <param name="mode">The mode.</param>
[<CompiledName("Triangular")>]
val triangular : min:float * max:float * mode:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed normally.
/// </summary>
/// <param name="mean">The mean.</param>
/// <param name="standardDeviation">The standard deviation.</param>
[<CompiledName("Normal")>]
val normal : mean:float * standardDeviation:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed log-normally.
/// </summary>
/// <param name="mu">The mu parameter.</param>
/// <param name="sigma">The sigma parameter.</param>
[<CompiledName("LogNormal")>]
val lognormal : mu:float * sigma:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed gamma.
/// </summary>
/// <param name="shape">The shape parameter.</param>
/// <param name="scale">The scale parameter.</param>
[<CompiledName("Gamma")>]
val gamma : shape:float * scale:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed beta.
/// </summary>
/// <param name="alpha">The first parameter.</param>
/// <param name="beta">The second parameter.</param>
[<CompiledName("Beta")>]
val beta : alpha:float * beta:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed exponentially.
/// </summary>
/// <param name="rate">The rate parameter (equals to its mean^(-1)).</param>
[<CompiledName("Exponential")>]
val exponential : rate:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed Weibull.
/// </summary>
/// <param name="shape">The shape parameter.</param>
/// <param name="scale">The scale parameter.</param>
[<CompiledName("Weibull")>]
val weibull : shape:float * scale:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed Gumbel.
/// </summary>
/// <param name="location">The location parameter.</param>
/// <param name="scale">The scale parameter.</param>
[<CompiledName("Gumbel")>]
val gumbel : location:float * scale:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed Cauchy.
/// </summary>
/// <param name="location">The location parameter.</param>
/// <param name="scale">The scale parameter.</param>
[<CompiledName("Cauchy")>]
val cauchy : location:float * scale:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed chi-square.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
[<CompiledName("ChiSquare")>]
val chisquare : degreeOfFreedom:int -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed Student-t.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
[<CompiledName("StudentT")>]
val studentT : degreeOfFreedom:int -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed Student-t.
/// </summary>
/// <remarks>
/// This is a synonym for <see cref="studentT" />
/// </remarks>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
[<System.Obsolete("Will be removed. Use studentT instead.")>]
[<CompiledName("T")>]
val t : degreeOfFreedom:int -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed von Mises.
/// </summary>
/// <param name="direction">The direction parameter.</param>
/// <param name="concentration">The concentration parameter.</param>
[<CompiledName("VonMises")>]
val vonMises : direction:float * concentration:float -> GeneratorFunction<float>
/// <summary>
/// Returns a random number distributed uniformly.
/// </summary>
/// <param name="min">The inclusive lower bound.</param>
/// <param name="max">The inclusive upper bound.</param>
[<CompiledName("UniformDiscrete")>]
val uniformDiscrete : min:int * max:int -> GeneratorFunction<int>
/// <summary>
/// Returns a random number distributed Poisson.
/// </summary>
/// <param name="lambda">The lambda parameter (equals to its mean).</param>
[<CompiledName("Poisson")>]
val poisson : lambda:float -> GeneratorFunction<int>
/// <summary>
/// Returns a random number distributed geometcally on {0, 1, 2, ...}.
/// </summary>
/// <param name="probability">The probability to success a trial.</param>
/// <seealso cref="geometric1" />
[<CompiledName("GeometricZeroBased")>]
val geometric0 : probability:float -> GeneratorFunction<int>
/// <summary>
/// Returns a random number distributed geometcally on {1, 2, 3, ...}.
/// </summary>
/// <param name="probability">The probability to success a trial.</param>
/// <seealso cref="geometric0" />
[<CompiledName("GeometricOneBased")>]
val geometric1 : probability:float -> GeneratorFunction<int>
/// <summary>
/// Returns a random number distributed Bernoulli.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="flipCoin" />
[<CompiledName("Bernoulli")>]
val bernoulli : probability:float -> GeneratorFunction<int>
/// <summary>
/// Returns a random number distributed binomially.
/// </summary>
/// <param name="n">The number of trials.</param>
/// <param name="probability">The probability to success a trial.</param>
[<CompiledName("Binomial")>]
val binomial : n:int * probability:float -> GeneratorFunction<int>
/// <summary>
/// Returns a random number distributed Dirichlet.
/// </summary>
/// <param name="alpha">The alpha parameter.</param>
[<CompiledName("Dirichlet")>]
val dirichlet : alpha:float list -> GeneratorFunction<float list>
/// <summary>
/// Returns a random number distributed multinomially.
/// </summary>
/// <param name="n">The number of trials.</param>
/// <param name="weight">The list of probability.
/// Each item is normalized in the function so that the sum of values can be less or greater than 1.</param>
[<CompiledName("Multinomial")>]
val multinomial : n:int * weight:float list -> GeneratorFunction<int list>
/// <summary>
/// Returns a random vector distributed multinormally.
/// </summary>
/// <param name="mu">The mean vector.</param>
/// <param name="sigma">The covariance matrix.</param>
[<CompiledName("Normal")>]
val multinormal : mu:float [] * sigma:float [,] -> GeneratorFunction<float []>
/// <summary>
/// Returns a random matrix distributed Wishart.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
/// <param name="sigma">The covariance matrix.</param>
[<CompiledName("Wishart")>]
val wishart : degreeOfFreedom:int * sigma:float [,] -> GeneratorFunction<float [,]>
/// <summary>
/// Returns a mixted distribution.
/// </summary>
/// <param name="distributions">The mixed model.</param>
[<CompiledName("Mix")>]
val mix : distributions:(GeneratorFunction<'a> * float) list -> GeneratorFunction<'a>

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Standard =
   /// <summary>
   /// Returns a standard uniform random number.
   /// </summary>
   [<CompiledName("Uniform")>]
   val uniform : GeneratorFunction<float>
   /// <summary>
   /// Returns a standard normal random number.
   /// </summary>
   [<CompiledName("Normal")>]
   val normal : GeneratorFunction<float>
   /// <summary>
   /// Returns a standard gamma random number.
   /// </summary>
   /// <param name="shape">The shape parameter.</param>
   [<CompiledName("Gamma")>]
   val gamma : shape:float -> GeneratorFunction<float>
   /// <summary>
   /// Returns a standard exponential random number.
   /// </summary>
   [<CompiledName("Exponential")>]
   val exponential : GeneratorFunction<float>
   /// <summary>
   /// Returns a standard Weibull random number.
   /// </summary>
   /// <param name="shape">The shape parameter.</param>
   [<CompiledName("Weibull")>]
   val weibull : shape:float -> GeneratorFunction<float>
   /// <summary>
   /// Returns a standard Gumbel random number.
   /// </summary>
   [<CompiledName("Gumbel")>]
   val gumbel : GeneratorFunction<float>
   /// <summary>
   /// Returns a standard Cauchy random number.
   /// </summary>
   [<CompiledName("Cauchy")>]
   val cauchy : GeneratorFunction<float>

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Seq =
   /// <summary>
   /// Makes infinite Markov chain.
   /// </summary>
   /// <param name="generator">A random function.</param>
   /// <returns>
   /// A Markov chain.
   /// </returns>
   [<CompiledName("MarkovChain")>]
   val markovChain : generator:('a -> GeneratorFunction<'a>) -> ('a -> PrngState -> seq<'a>)
