/// <summary>
/// Provides the core random classes and the primitive random number generators.
/// </summary>
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
type GeneratorFunction<'a> = GeneratorFunction of (PrngState -> 'a * PrngState)

/// <summary>
/// Constructs a random state.
/// </summary>
/// <param name="prng">The PRNG.</param>
/// <param name="seed">The random seed.</param>
[<CompiledName("CreateState")>]
val createState : prng:Prng<'s> -> seed:'s -> PrngState

val inline internal ( |>> ) : m:GeneratorFunction<'a> -> f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
val inline internal ( &>> ) : m:GeneratorFunction<'a> -> b:GeneratorFunction<'b> -> GeneratorFunction<'b>
val internal bindRandom : m:GeneratorFunction<'a> -> f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
val internal returnRandom : a:'a -> GeneratorFunction<'a>
val internal runRandom : GeneratorFunction<'a> -> PrngState -> 'a * PrngState
val internal evaluateRandom : GeneratorFunction<'a> -> PrngState -> 'a
val internal executeRandom : GeneratorFunction<'a> -> PrngState -> PrngState

[<Class>]
type RandomBuilder =
   member Bind : m:GeneratorFunction<'a> * f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
   member Combine : a:GeneratorFunction<'a> * b:GeneratorFunction<'b> -> GeneratorFunction<'b>
   member Return : a:'a -> GeneratorFunction<'a>
   member ReturnFrom : m:GeneratorFunction<'a> -> GeneratorFunction<'a>
   member Zero : unit -> GeneratorFunction<'a>
   member Delay : (unit -> GeneratorFunction<'a>) -> GeneratorFunction<'a>
   member While : condition:(unit -> bool) * m:GeneratorFunction<'a> -> GeneratorFunction<'a>
   member For : source:seq<'a> * f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b>
   member TryFinally : m:GeneratorFunction<'a> * finalizer:(unit -> unit) -> GeneratorFunction<'a>
   member TryWith : m:GeneratorFunction<'a> * handler:(exn -> GeneratorFunction<'a>) -> GeneratorFunction<'a>
   member Using : a:'a * f:('a -> GeneratorFunction<'b>) -> GeneratorFunction<'b> when 'a :> IDisposable
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
[<CompiledName("SystemRandomPrng")>]
val systemrandom : Prng<Random>
/// <summary>
/// Random number generator using Xorshift algorithm (Marsaglia 2003).
/// </summary>
[<CompiledName("XorshiftPrng")>]
val xorshift : Prng<uint32 * uint32 * uint32 * uint32>

/// <summary>
/// Returns a random 64-bit number.
/// </summary>
[<CompiledName("RawBits")>]
val rawBits : GeneratorFunction<uint64>
/// <summary>
/// Returns a random number in the range of (0, 1).
/// </summary>
[<CompiledName("StandardOpen")>]
val ``(0, 1)`` : GeneratorFunction<float>
/// <summary>
/// Returns a random number in the range of [0, 1).
/// </summary>
[<CompiledName("Standard")>]
val ``[0, 1)`` : GeneratorFunction<float>
/// <summary>
/// Returns a random number in the range of (0, 1].
/// </summary>
[<CompiledName("StandardOpenClosed")>]
val ``(0, 1]`` : GeneratorFunction<float>
/// <summary>
/// Returns a random number in the range of [0, 1].
/// </summary>
[<CompiledName("StandardClosed")>]
val ``[0, 1]`` : GeneratorFunction<float>

/// <summary>
/// Returns a random 8-bit signed integer.
/// </summary>
[<CompiledName("RandomInt8")>]
val rint8 : GeneratorFunction<int8>
/// <summary>
/// Returns a random 16-bit signed integer.
/// </summary>
[<CompiledName("RandomInt16")>]
val rint16 : GeneratorFunction<int16>
/// <summary>
/// Returns a random 32-bit signed integer.
/// </summary>
[<CompiledName("RandomInt32")>]
val rint32 : GeneratorFunction<int32>
/// <summary>
/// Returns a random 64-bit signed integer.
/// </summary>
[<CompiledName("RandomInt64")>]
val rint64 : GeneratorFunction<int64>
/// <summary>
/// Returns a random 8-bit unsigned integer.
/// </summary>
[<CompiledName("RandomUInt8")>]
val ruint8 : GeneratorFunction<uint8>
/// <summary>
/// Returns a random 16-bit unsigned integer.
/// </summary>
[<CompiledName("RandomUInt16")>]
val ruint16 : GeneratorFunction<uint16>
/// <summary>
/// Returns a random 32-bit unsigned integer.
/// </summary>
[<CompiledName("RandomUInt32")>]
val ruint32 : GeneratorFunction<uint32>
/// <summary>
/// Returns a random 64-bit unsigned integer.
/// This function is an alias for <see cref="RawBits" />.
/// </summary>
[<CompiledName("RandomUInt64")>]
val ruint64 : GeneratorFunction<uint64>
