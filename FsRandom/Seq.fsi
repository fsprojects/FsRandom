[<RequireQualifiedAccess>]
module FsRandom.Seq

/// <summary>
/// Makes infinite sequence of random numbers.
/// </summary>
/// <param name="prng">A PRNG.</param>
/// <param name="generator">A random function.</param>
/// <returns>
/// A function which takes a seed and returns infinite sequence of random numbers.
/// </returns>
val ofRandom : prng:Prng<'s> -> generator:GeneratorFunction<'s, 'a> -> ('s -> seq<'a>)
