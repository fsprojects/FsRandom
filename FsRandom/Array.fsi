[<RequireQualifiedAccess>]
module FsRandom.Array

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="generator">The generator function.</param>
val randomCreate : count:int -> generator:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, 'a []>

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="initializer">The function to take an index and produce a random number generating function.</param>
val randomInit : count:int -> initializer:(int -> GeneratorFunction<'s, 'a>) -> GeneratorFunction<'s, 'a []>

/// <summary>
/// Fills an array whose elements are randomly generated.
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="generator">The generator function.</param>
val randomFill : array:'a [] -> targetIndex:int -> count:int -> generator:GeneratorFunction<'s, 'a> -> GeneratorFunction<'s, unit>

/// <summary>
/// Creates a new array whose elements are random set of the elements of the specified array.
/// </summary>
/// <param name="array">The array to shuffle.</param>
/// <seealso cref="shuffleInPlace" />
val shuffle : array:'a [] -> GeneratorFunction<'s, 'a []>

/// <summary>
/// Shuffles the elements of the specified array by mutating it in-place.
/// </summary>
/// <param name="array">The array to shuffle.</param>
/// <seealso cref="shuffle" />
val shuffleInPlace : array:'a [] -> GeneratorFunction<'s, unit>

/// <summary>
/// Picks up random samples without replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <seealso cref="sampleWithReplacement" />
val sample : n:int -> source:'a [] -> GeneratorFunction<'s, 'a []>

/// <summary>
/// Picks up weighted random samples without replacement in the specified array.
/// </summary>
/// <remarks>
/// Implements Efraimidis &amp; Spirakis's A-ExpJ algorithm (Efraimidis &amp; Spirakis 2006).
/// </remarks>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <param name="weight">The sampling weight for each sample.</param>
val weightedSample : n:int -> weight:float [] -> source:'a [] -> GeneratorFunction<'s, 'a []>

/// <summary>
/// Picks up random samples with replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <seealso cref="sample" />
val sampleWithReplacement : n:int -> source:'a [] -> GeneratorFunction<'s, 'a []>

/// <summary>
/// Picks up weighted random samples with replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <param name="weight">The sampling weight for each sample.</param>
val weightedSampleWithReplacement : n:int -> weight:float [] -> source:'a [] -> GeneratorFunction<'s, 'a []>
