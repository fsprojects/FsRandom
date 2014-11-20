/// <summary>
/// Provides basic operations on arrays.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.Array

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="generator">The generator function.</param>
[<CompiledName("RandomCreate")>]
val randomCreate : count:int -> generator:GeneratorFunction<'a> -> GeneratorFunction<'a []>

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="initializer">The function to take an index and produce a random number generating function.</param>
[<CompiledName("RandomInitialize")>]
val randomInit : count:int -> initializer:(int -> GeneratorFunction<'a>) -> GeneratorFunction<'a []>

/// <summary>
/// Fills an array whose elements are randomly generated.
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="generator">The generator function.</param>
[<CompiledName("RandomFill")>]
val randomFill : array:'a [] -> targetIndex:int -> count:int -> generator:GeneratorFunction<'a> -> GeneratorFunction<unit>

/// <summary>
/// Creates a new array whose elements are random set of the elements of the specified array.
/// </summary>
/// <param name="array">The array to shuffle.</param>
/// <seealso cref="shuffleInPlace" />
[<CompiledName("Shuffle")>]
val shuffle : array:'a [] -> GeneratorFunction<'a []>

/// <summary>
/// Shuffles the elements of the specified array by mutating it in-place.
/// </summary>
/// <param name="array">The array to shuffle.</param>
/// <seealso cref="shuffle" />
[<CompiledName("ShuffleInPlace")>]
val shuffleInPlace : array:'a [] -> GeneratorFunction<unit>

/// <summary>
/// Picks up random samples without replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <seealso cref="sampleWithReplacement" />
[<CompiledName("Sample")>]
val sample : n:int -> source:'a [] -> GeneratorFunction<'a []>

/// <summary>
/// Picks up weighted random samples without replacement in the specified array.
/// </summary>
/// <remarks>
/// Implements Efraimidis &amp; Spirakis's A-ExpJ algorithm (Efraimidis &amp; Spirakis 2006).
/// </remarks>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <param name="weight">The sampling weight for each sample.</param>
[<CompiledName("WeightedSample")>]
val weightedSample : n:int -> weight:float [] -> source:'a [] -> GeneratorFunction<'a []>

/// <summary>
/// Picks up random samples with replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <seealso cref="sample" />
[<CompiledName("SampleWithReplacement")>]
val sampleWithReplacement : n:int -> source:'a [] -> GeneratorFunction<'a []>

/// <summary>
/// Picks up weighted random samples with replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <param name="weight">The sampling weight for each sample.</param>
[<CompiledName("WeightedSampleWithReplacement")>]
val weightedSampleWithReplacement : n:int -> weight:float [] -> source:'a [] -> GeneratorFunction<'a []>
