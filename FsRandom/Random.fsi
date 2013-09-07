[<RequireQualifiedAccess>]
module FsRandom.Random

/// <summary>
/// Generates a random number with the next random state.
/// </summary>
val inline next : generator:GeneratorFunction<'s, 'a> -> (PrngState<'s> -> 'a * PrngState<'s>)
/// <summary>
/// Generates a random number.
/// </summary>
val inline get : generator:GeneratorFunction<'s, 'a> -> (PrngState<'s> -> 'a)

/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns the value.
/// </summary>
/// <param name="generator">The random number generator.</param>
val inline identity : generator:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, 'a>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns a transformed value by <paramref name="transformation" /> function.
/// </summary>
/// <param name="transformation">The function to transform a random value.</param>
/// <param name="generator">The random number generator.</param>
val inline transformBy : transformation:('a -> 'b) -> generator:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, 'b>
/// <summary>
/// Generates a random number by using two random numbers.
/// </summary>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
val inline transformBy2 : transformation:('a1 -> 'a2 -> 'b) -> generator1:GeneratorFunction<'s, 'a1> -> generator2:GeneratorFunction<'s, 'a2> -> GeneratorFunction<'s, 'b>
