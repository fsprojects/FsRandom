[<AutoOpen>]
module FsRandom.RandomNumberGenerator

open System
open FsRandom.StateMonad

/// <summary>
/// Represents a pseudorandom number generator that supports 32-bit resolution.
/// </summary>
type Prng<'s> = 's -> uint64 * 's
type PrngState<'s> = Prng<'s> * 's
/// <summary>
/// Generates random numbers.
/// </summary>
type GeneratorFunction<'s, 'a> = State<PrngState<'s>, 'a>

/// <summary>
/// Constructs a random number function.
/// </summary>
val random : StateBuilder

/// <summary>
/// Random number generator using user-specified random number generator.
/// </summary>
type RandomBuilder<'s> =
   inherit StateBuilder
   /// <summary>
   /// Initializes a new instance.
   /// </summary>
   new : initial:Prng<'s> -> RandomBuilder<'s>
   /// <summary>
   /// Runs the random computation expression.
   /// </summary>
   /// <returns>
   /// A random number function.
   /// It takes a seed and returns a tuple of a random number and a new seed.
   /// </returns>
   member Run : m:GeneratorFunction<'s, 'a> -> State<'s, 'a>
   
/// <summary>
/// Random number generator using user-specified random number generator.
/// </summary>
/// <param name="prng">A random number generator.</param>
/// <param name="seed">A initial state to generate a random sequence.</param>
val createRandomBuilder : prng:Prng<'s> -> RandomBuilder<'s>
   
/// <summary>
/// Random number generator using <see cref="System.Random" />.
/// </summary>
/// <remarks>
/// You will get different result on each call because an instance of <see cref="System.Random" /> has state by itself.
/// </remarks>
val systemrandom : RandomBuilder<Random>

/// <summary>
/// Random number generator using Xorshift algorithm (Marsaglia 2003).
/// </summary>
val xorshift : RandomBuilder<uint32 * uint32 * uint32 * uint32>

/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns the value.
/// </summary>
/// <param name="generator">The random number generator.</param>
val getRandom : generator:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, 'a>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns a transformed value by <paramref name="transformation" /> function.
/// </summary>
/// <param name="transformation">The function to transform a random value.</param>
/// <param name="generator">The random number generator.</param>
val getRandomBy : transformation:('a -> 'b) -> generator:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, 'b>

/// <summary>
/// Returns a random 64-bit number.
/// </summary>
val rawBits : GeneratorFunction<'s, uint64>
/// <summary>
/// Returns a random number in the range of (0, 1).
/// </summary>
val ``(0, 1)`` : GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number in the range of [0, 1).
/// </summary>
val ``[0, 1)`` : GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number in the range of (0, 1].
/// </summary>
val ``(0, 1]`` : GeneratorFunction<'s, float>
/// <summary>
/// Returns a random number in the range of [0, 1].
/// </summary>
val ``[0, 1]`` : GeneratorFunction<'s, float>
