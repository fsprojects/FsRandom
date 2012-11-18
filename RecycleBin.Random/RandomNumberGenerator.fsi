[<AutoOpen>]
module RecycleBin.Random.RandomNumberGenerator

open System
open RecycleBin.Random.StateMonad

/// <summary>
/// Represents a pseudorandom number generator that supports 32-bit resolution.
/// </summary>
type Prng<'s> = 's -> uint32 * 's
type PrngState<'s> = Prng<'s> * 's

/// <summary>
/// Random number generator using user-specified random number generator.
/// </summary>
type RandomBuilder<'s> =
   inherit StateBuilder
   /// <summary>
   /// Initializes a new instance.
   /// </summary>
   new : initial:PrngState<'s> -> RandomBuilder<'s>
   /// <summary>
   /// Runs the random computation expression.
   /// </summary>
   /// <returns>
   /// The result comprises a tuple with two elements.
   /// The first element is the result of the random number computation.
   /// The second element is the state after the computation.
   /// </returns>
   member Run : m:State<PrngState<'s>, 'a> -> 'a * 's
   
/// <summary>
/// Random number generator using user-specified random number generator.
/// </summary>
/// <param name="prng">A random number generator.</param>
/// <param name="seed">A initial state to generate a random sequence.</param>
val random : prng:Prng<'s> -> seed:'s -> RandomBuilder<'s>
   
/// <summary>
/// Random number generator using <see cref="System.Random" />.
/// </summary>
/// <remarks>
/// You will get different result on each call because an instance of <see cref="System.Random" /> has state by itself.
/// </remarks>
/// <param name="random">A random number generator.</param>
val systemrandom : random:Random -> RandomBuilder<Random>

/// <summary>
/// Random number generator using Xorshift algorithm (Marsaglia 2003).
/// </summary>
/// <param name="seed">Random seed composed of four 32-bit unsigned integers.</param>
val xorshift : seed:(uint32 * uint32 * uint32 * uint32) -> RandomBuilder<uint32 * uint32 * uint32 * uint32>

/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns the value.
/// </summary>
/// <param name="generator">The random number generator.</param>
val getRandom : generator:State<PrngState<'s>, 'a> -> State<PrngState<'s>, 'a>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns a transformed value by <paramref name="transformation" /> function.
/// </summary>
/// <param name="transformation">The function to transform a random value.</param>
/// <param name="generator">The random number generator.</param>
val getRandomBy : transformation:('a -> 'b) -> generator:State<PrngState<'s>, 'a> -> State<PrngState<'s>, 'b>

/// <summary>
/// Returns a random number in the range of (0, 1).
/// </summary>
val ``(0, 1)`` : State<PrngState<'s>, float>
/// <summary>
/// Returns a random number in the range of [0, 1).
/// </summary>
val ``[0, 1)`` : State<PrngState<'s>, float>
/// <summary>
/// Returns a random number in the range of (0, 1].
/// </summary>
val ``(0, 1]`` : State<PrngState<'s>, float>
/// <summary>
/// Returns a random number in the range of [0, 1].
/// </summary>
val ``[0, 1]`` : State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed uniformly in the range of [<paramref name="min" />, <paramref name="max" />].
/// </summary>
/// <param name="min">The inclusive lower limit.</param>
/// <param name="max">The inclusive upper limit.</param>
/// <seealso cref="(0, 1)" />
/// <seealso cref="[0, 1)" />
/// <seealso cref="(0, 1]" />
/// <seealso cref="[0, 1]" />
val uniform : min:float * max:float -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed normally.
/// </summary>
/// <param name="mean">The mean.</param>
/// <param name="standardDeviation">The standard deviation.</param>
val normal : mean:float * standardDeviation:float -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed gamma.
/// </summary>
/// <param name="shape">The shape parameter.</param>
/// <param name="scale">The scale parameter.</param>
val gamma : shape:float * scale:float -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed beta.
/// </summary>
/// <param name="alpha">The first parameter.</param>
/// <param name="beta">The second parameter.</param>
val beta : alpha:float * beta:float -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed exponentially.
/// </summary>
/// <param name="rate">The rate parameter (equals to its mean^(-1)).</param>
val exponential : rate:float -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed cauchy.
/// </summary>
/// <param name="location">The location parameter.</param>
/// <param name="scale">The scale parameter.</param>
val cauchy : location:float * scale:float -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed chi-square.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
val chisquare : degreeOfFreedom:int -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed Student-t.
/// </summary>
/// <param name="degreeOfFreedom">The degree of freedom.</param>
val t : degreeOfFreedom:int -> State<PrngState<'s>, float>
/// <summary>
/// Returns a random number distributed uniformly.
/// </summary>
/// <param name="min">The inclusive lower bound.</param>
/// <param name="max">The inclusive upper bound.</param>
val uniformDiscrete : min:int * max:int -> State<PrngState<'s>, int>
/// <summary>
/// Returns a random number distributed Poisson.
/// </summary>
/// <param name="lambda">The lambda parameter (equals to its mean).</param>
val poisson : lambda:float -> State<PrngState<'s>, int>
/// <summary>
/// Returns a random number distributed geometcally.
/// </summary>
/// <param name="probability">The probability to success a trial.</param>
val geometric : probability:float -> State<PrngState<'s>, int>
/// <summary>
/// Returns a random number distributed Bernoulli.
/// </summary>
/// <param name="probability">The probability to success a trial.</param>
val bernoulli : probability:float -> State<PrngState<'s>, int>
/// <summary>
/// Returns a random number distributed binomially.
/// </summary>
/// <param name="n">The number of trials.</param>
/// <param name="probability">The probability to success a trial.</param>
val binomial : n:int * probability:float -> State<PrngState<'s>, int>
/// <summary>
/// Returns a random number distributed Dirichlet.
/// </summary>
/// <param name="alpha">The alpha parameter.</param>
val dirichlet : alpha:float list -> State<PrngState<'s>, float list>
