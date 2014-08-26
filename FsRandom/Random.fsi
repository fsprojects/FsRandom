/// <summary>
/// Provides basic operations on generator functions.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.Random

/// <summary>
/// Invokes a function on a random function.
/// </summary>
[<CompiledName("Bind")>]
val bind : binder:('a -> GeneratorFunction<'b>) -> generator:GeneratorFunction<'a> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number with the next random state.
/// </summary>
[<CompiledName("Next")>]
val next : generator:GeneratorFunction<'a> -> PrngState -> 'a * PrngState
/// <summary>
/// Generates a random number.
/// </summary>
[<CompiledName("Get")>]
val get : generator:GeneratorFunction<'a> -> PrngState -> 'a

/// <summary>
/// Always returns the specified value.
/// </summary>
/// <param name="x">The value.</param>
[<CompiledName("Singleton")>]
val singleton : x:'a -> GeneratorFunction<'a>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns the value.
/// </summary>
/// <param name="generator">The random number generator.</param>
[<CompiledName("Identity")>]
val identity : generator:GeneratorFunction<'a> -> GeneratorFunction<'a>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns a transformed value by <paramref name="transformation" /> function.
/// </summary>
/// <param name="transformation">The function to transform a random value.</param>
/// <param name="generator">The random number generator.</param>
[<CompiledName("Map")>]
val map : transformation:('a -> 'b) -> generator:GeneratorFunction<'a> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number by using two random numbers.
/// </summary>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
[<CompiledName("Map2")>]
val map2 : transformation:('a1 -> 'a2 -> 'b) -> generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number by using three random numbers.
/// </summary>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
/// <param name="generator3">The third random number generator.</param>
[<CompiledName("Map3")>]
val map3 : transformation:('a1 -> 'a2 -> 'a3 -> 'b) -> generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> generator2:GeneratorFunction<'a3> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number by <paramref name="generator" /> and returns a transformed value by <paramref name="transformation" /> function.
/// </summary>
/// <remarks>
/// This function is a synonym for <see cref="map" />.
/// </remarks>
/// <param name="transformation">The function to transform a random value.</param>
/// <param name="generator">The random number generator.</param>
[<CompiledName("TransformBy")>]
val transformBy : transformation:('a -> 'b) -> generator:GeneratorFunction<'a> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number by using two random numbers.
/// </summary>
/// <remarks>
/// This function is a synonym for <see cref="map2" />.
/// </remarks>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
[<CompiledName("TransformBy")>]
val transformBy2 : transformation:('a1 -> 'a2 -> 'b) -> generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> GeneratorFunction<'b>
/// <summary>
/// Generates a random number by using three random numbers.
/// </summary>
/// <remarks>
/// This function is a synonym for <see cref="map3" />.
/// </remarks>
/// <param name="transformation">The function to transform two random values into one.</param>
/// <param name="generator1">The first random number generator.</param>
/// <param name="generator2">The second random number generator.</param>
/// <param name="generator3">The third random number generator.</param>
[<CompiledName("TransformBy")>]
val transformBy3 : transformation:('a1 -> 'a2 -> 'a3 -> 'b) -> generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> generator2:GeneratorFunction<'a3> -> GeneratorFunction<'b>

/// <summary>
/// Merges two random streams into one.
/// </summary>
[<CompiledName("Zip")>]
val zip : generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> GeneratorFunction<'a1 * 'a2>
/// <summary>
/// Merges three random streams into one.
/// </summary>
[<CompiledName("Zip3")>]
val zip3 : generator1:GeneratorFunction<'a1> -> generator2:GeneratorFunction<'a2> -> generator3:GeneratorFunction<'a3> -> GeneratorFunction<'a1 * 'a2 * 'a3>
/// <summary>
/// Merges random stream list into one.
/// </summary>
[<CompiledName("Merge")>]
val merge : generators:GeneratorFunction<'a> list -> GeneratorFunction<'a list>
/// <summary>
/// Merges random stream list into one and then apply the specified function.
/// </summary>
[<CompiledName("MergeWith")>]
val mergeWith : f:('a list -> 'b) -> (GeneratorFunction<'a> list -> GeneratorFunction<'b>)
