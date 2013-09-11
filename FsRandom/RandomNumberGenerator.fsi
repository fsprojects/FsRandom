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
type PrngState<'s> = Prng<'s> * 's
/// <summary>
/// Generates random numbers.
/// </summary>
type GeneratorFunction<'s, 'a> = PrngState<'s> -> 'a * PrngState<'s>

val inline internal ( |>> ) : m:GeneratorFunction<'s, 'a> -> f:('a -> GeneratorFunction<'s, 'b>) -> GeneratorFunction<'s, 'b>
val inline internal ( &>> ) : m:GeneratorFunction<'s, 'a> -> b:GeneratorFunction<'s, 'b> -> GeneratorFunction<'s, 'b>
val inline internal bindRandom : m:GeneratorFunction<'s, 'a> -> f:('a -> GeneratorFunction<'s, 'b>) -> GeneratorFunction<'s, 'b>
val inline internal returnRandom : a:'a -> GeneratorFunction<'s, 'a>
val inline internal getRandom : GeneratorFunction<'s, 's>
val inline internal setRandom : state:PrngState<'s> -> GeneratorFunction<'s, unit>
val inline internal runRandom : GeneratorFunction<'s, 'a> -> PrngState<'s> -> 'a * PrngState<'s>
val inline internal evaluateRandom : GeneratorFunction<'s, 'a> -> PrngState<'s> -> 'a
val inline internal executeRandom : GeneratorFunction<'s, 'a> -> PrngState<'s> -> PrngState<'s>

type RandomBuilder =
   new : unit -> RandomBuilder
   member Bind : m:GeneratorFunction<'s, 'a> * f:('a -> GeneratorFunction<'s, 'b>) -> GeneratorFunction<'s, 'b>
   member Combine : a:GeneratorFunction<'s, 'a> * b:GeneratorFunction<'s, 'b> -> GeneratorFunction<'s, 'b>
   member Return : a:'a -> GeneratorFunction<'s, 'a>
   member ReturnFrom : m:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, 'a>
   member Zero : unit -> GeneratorFunction<'s, unit>
   member Delay : (unit -> GeneratorFunction<'s, 'a>) -> GeneratorFunction<'s, 'a>
   member While : condition:(unit -> bool) * m:GeneratorFunction<'s, unit> -> GeneratorFunction<'s, unit>
   member For : source:seq<'a> * f:('a -> GeneratorFunction<'s, unit>) -> GeneratorFunction<'s, unit>
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
