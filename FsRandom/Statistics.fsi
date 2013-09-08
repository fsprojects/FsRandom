module FsRandom.Statistics

/// <summary>
/// Returns a random number distributed uniformly in the range of [<paramref name="min" />, <paramref name="max" />].
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
val uniform : min:float * max:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed log-uniformly in the range of [<paramref name="min" />, <paramref name="max" />].
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
val loguniform : min:float * max:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed triangular.
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
/// <param name="mode">The mode.</param>
val triangular : min:float * max:float * mode:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed normally.
/// </summary>
/// <param name="mean">The mean.</param>
/// <param name="standardDeviation">The standard deviation.</param>
val normal : mean:float * standardDeviation:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed log-normally.
/// </summary>
/// <param name="mu">The mu parameter.</param>
/// <param name="sigma">The sigma parameter.</param>
val lognormal : mu:float * sigma:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed gamma.
/// </summary>
/// <param name="shape">The shape parameter.</param>
/// <param name="scale">The scale parameter.</param>
val gamma : shape:float * scale:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed beta.
/// </summary>
/// <param name="alpha">The first parameter.</param>
/// <param name="beta">The second parameter.</param>
val beta : alpha:float * beta:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed exponentially.
/// </summary>
/// <param name="rate">The rate parameter (equals to its mean^(-1)).</param>
val exponential : rate:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed Weibull.
/// </summary>
/// <param name="shape">The shape parameter.</param>
/// <param name="scale">The scale parameter.</param>
val weibull : shape:float * scale:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed Gumbel.
/// </summary>
/// <param name="location">The location parameter.</param>
/// <param name="scale">The scale parameter.</param>
val gumbel : location:float * scale:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed cauchy.
/// </summary>
/// <param name="location">The location parameter.</param>
/// <param name="scale">The scale parameter.</param>
val cauchy : location:float * scale:float -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed chi-square.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
val chisquare : degreeOfFreedom:int -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed Student-t.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
val t : degreeOfFreedom:int -> GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number distributed uniformly.
/// </summary>
/// <param name="min">The inclusive lower bound.</param>
/// <param name="max">The inclusive upper bound.</param>
val uniformDiscrete : min:int * max:int -> GeneratorFunction<'s, int>
/// <summary>
/// Returns a random number distributed Poisson.
/// </summary>
/// <param name="lambda">The lambda parameter (equals to its mean).</param>
val poisson : lambda:float -> GeneratorFunction<'s, int>
/// <summary>
/// Returns a random number distributed geometcally.
/// </summary>
/// <param name="probability">The probability to success a trial.</param>
val geometric : probability:float -> GeneratorFunction<'s, int>
/// <summary>
/// Returns a random number distributed Bernoulli.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="flipCoin" />
val bernoulli : probability:float -> GeneratorFunction<'s, int>
/// <summary>
/// Returns a random number distributed binomially.
/// </summary>
/// <param name="n">The number of trials.</param>
/// <param name="probability">The probability to success a trial.</param>
val binomial : n:int * probability:float -> GeneratorFunction<'s, int>
/// <summary>
/// Returns a random number distributed Dirichlet.
/// </summary>
/// <param name="alpha">The alpha parameter.</param>
val dirichlet : alpha:float list -> GeneratorFunction<'s, float list>
/// <summary>
/// Returns a random number distributed multinomially.
/// </summary>
/// <param name="n">The number of trials.</param>
/// <param name="weight">The list of probability.
/// Each item is normalized in the function so that the sum of values can be less or greater than 1.</param>
val multinomial : n:int * weight:float list -> GeneratorFunction<'s, int list>

[<RequireQualifiedAccess>]
module Seq =
   /// <summary>
   /// Makes infinite Markov chain.
   /// </summary>
   /// <param name="generator">A random function.</param>
   /// <param name="builder">A random builder.</param>
   /// <returns>
   /// A Markov chain.
   /// </returns>
   val markovChain : generator:('a -> GeneratorFunction<'s, 'a>) -> ('a -> PrngState<'s> -> seq<'a>)