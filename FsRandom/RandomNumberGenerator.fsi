[<AutoOpen>]
module FsRandom.RandomNumberGenerator

open System

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
type GeneratorFunction<'a> = PrngState -> 'a * PrngState

/// <summary>
/// Constructs a random state.
/// </summary>
/// <param name="prng">The PRNG.</param>
/// <param name="seed">The random seed.</param>
val createState : prng:Prng<'s> -> seed:'s -> PrngState

val inline internal ( |>> ) : m:GeneratorFunction<'a> -> f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
val inline internal ( &>> ) : m:GeneratorFunction<'a> -> b:GeneratorFunction<'b> -> GeneratorFunction<'b>
val inline internal bindRandom : m:GeneratorFunction<'a> -> f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
val inline internal returnRandom : a:'a -> GeneratorFunction<'a>
//val inline internal getRandom : GeneratorFunction<'s>
//val inline internal setRandom : state:PrngState -> GeneratorFunction<unit>
val inline internal runRandom : GeneratorFunction<'a> -> PrngState -> 'a * PrngState
val inline internal evaluateRandom : GeneratorFunction<'a> -> PrngState -> 'a
val inline internal executeRandom : GeneratorFunction<'a> -> PrngState -> PrngState

type RandomBuilder =
   new : unit -> RandomBuilder
   member Bind : m:GeneratorFunction<'a> * f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
   member Combine : a:GeneratorFunction<'a> * b:GeneratorFunction<'b> -> GeneratorFunction<'b>
   member Return : a:'a -> GeneratorFunction<'a>
   member ReturnFrom : m:GeneratorFunction<'a> -> GeneratorFunction<'a>
   member Zero : unit -> GeneratorFunction<unit>
   member Delay : (unit -> GeneratorFunction<'a>) -> GeneratorFunction<'a>
   member While : condition:(unit -> bool) * m:GeneratorFunction<unit> -> GeneratorFunction<unit>
   member For : source:seq<'a> * f:('a -> GeneratorFunction<unit>) -> GeneratorFunction<unit>
/// <summary>
/// Constructs a random number function.
/// </summary>
val random : RandomBuilder
   
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
