[<RequireQualifiedAccess>]
module FsRandom.Seq

/// <summary>
/// Makes infinite sequence of random numbers.
/// </summary>
/// <param name="generator">A random function.</param>
/// <returns>
/// A function which takes a seed and returns infinite sequence of random numbers.
/// </returns>
val ofRandom : generator:GeneratorFunction<'s, 'a> -> (Prng<'s> -> 's -> seq<'a>)
