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
val systemrandom : (Random -> RandomBuilder<Random>)

/// <summary>
/// Random number generator using Xorshift algorithm (Marsaglia 2003).
/// </summary>
val xorshift : (uint32 * uint32 * uint32 * uint32 -> RandomBuilder<uint32 * uint32 * uint32 * uint32>)

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
