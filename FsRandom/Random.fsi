[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.Random

/// <summary>
/// Generates a random number with the next random state.
/// </summary>
val inline next : generator:GeneratorFunction<'a> -> PrngState -> 'a * PrngState
/// <summary>
/// Generates a random number.
/// </summary>
val inline get : generator:GeneratorFunction<'a> -> PrngState -> 'a

/// <summary>
/// Always returns the specified value.
/// </summary>
/// <param name="x">The value.</param>
val inline singleton : x:'a -> GeneratorFunction<'a>
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
/// <summary>
/// Generates a random number by using three random numbers.
/// </summary>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
/// <param name="generator3">The third random number generator.</param>
val inline transformBy3 : transformation:('a1 -> 'a2 -> 'a3 -> 'b) -> generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> generator2:GeneratorFunction<'a3> -> GeneratorFunction<'b>

/// <summary>
/// Merges two random streams into one.
/// </summary>
val inline zip : generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> GeneratorFunction<'a1 * 'a2>
/// <summary>
/// Merges three random streams into one.
/// </summary>
val inline zip3 : generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> generator3:GeneratorFunction<'a3> -> GeneratorFunction<'a1 * 'a2 * 'a3>
/// <summary>
/// Merges random stream list into one.
/// </summary>
val inline merge : generators:GeneratorFunction<'a> list -> GeneratorFunction<'a list>
/// <summary>
/// Merges random stream list into one and then apply the specified function.
/// </summary>
val inline mergeWith : f:('a list -> 'b) -> (GeneratorFunction<'a> list -> GeneratorFunction<'b>)
