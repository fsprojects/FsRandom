[<AutoOpen>]
module FsRandom.RandomNumberGenerator

open System
open FsRandom.StateMonad

/// <summary>
/// Represents a pseudorandom number generator that supports 64-bit resolution.
/// </summary>
type Prng<'s> = 's -> uint64 * 's
/// <summary>
/// Represents a random state.
/// </summary>
type PrngState =
   abstract Next64Bits : unit -> uint64 * PrngState
/// <summary>
/// Generates random numbers.
/// </summary>
type GeneratorFunction<'a> = State<PrngState, 'a>

/// <summary>
/// Constructs a random state.
/// </summary>
/// <param name="prng">The PRNG.</param>
/// <param name="seed">The random seed.</param>
val createState : prng:Prng<'s> -> seed:'s -> PrngState

/// <summary>
/// Constructs a random number function.
/// </summary>
val random : StateBuilder
   
/// <summary>
/// Random number generator using <see cref="System.Random" />.
/// </summary>
/// <remarks>
/// You will get different result on each call because an instance of <see cref="System.Random" /> has state by itself.
/// </remarks>
val systemrandom : Prng<Random>
/// <summary>
/// Random number generator using Xorshift algorithm (Marsaglia 2003).
/// </summary>
val xorshift : Prng<uint32 * uint32 * uint32 * uint32>

/// <summary>
/// Returns a random 64-bit number.
/// </summary>
val rawBits : GeneratorFunction<uint64>
/// <summary>
/// Returns a random number in the range of (0, 1).
/// </summary>
val ``(0, 1)`` : GeneratorFunction<float>
/// <summary>
/// Returns a random number in the range of [0, 1).
/// </summary>
val ``[0, 1)`` : GeneratorFunction<float>
/// <summary>
/// Returns a random number in the range of (0, 1].
/// </summary>
val ``(0, 1]`` : GeneratorFunction<float>
/// <summary>
/// Returns a random number in the range of [0, 1].
/// </summary>
val ``[0, 1]`` : GeneratorFunction<float>
