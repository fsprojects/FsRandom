[<RequireQualifiedAccess>]
module FsRandom.Seq

/// <summary>
/// Makes infinite sequence of random numbers.
/// </summary>
/// <param name="generator">A random function.</param>
/// <param name="builder">A random builder.</param>
/// <returns>
/// A function which takes a seed and returns infinite sequence of random numbers.
/// </returns>
val ofRandom : generator:GeneratorFunction<'s, 'a> -> (PrngState<'s> -> seq<'a>)
