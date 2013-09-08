[<RequireQualifiedAccess>]
module FsRandom.Random

/// <summary>
/// Generates a random number with the next random state.
/// </summary>
val inline next : generator:GeneratorFunction<'a> -> (PrngState -> 'a * PrngState)
/// <summary>
/// Generates a random number.
/// </summary>
val inline get : generator:GeneratorFunction<'a> -> (PrngState -> 'a)

/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns the value.
/// </summary>
/// <param name="generator">The random number generator.</param>
val inline identity : generator:GeneratorFunction<'a> -> GeneratorFunction<'a>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns a transformed value by <paramref name="transformation" /> function.
/// </summary>
/// <param name="transformation">The function to transform a random value.</param>
/// <param name="generator">The random number generator.</param>
val inline transformBy : transformation:('a -> 'b) -> generator:GeneratorFunction<'a> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number by using two random numbers.
/// </summary>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
val inline transformBy2 : transformation:('a1 -> 'a2 -> 'b) -> generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> GeneratorFunction<'b>
