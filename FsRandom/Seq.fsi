[<RequireQualifiedAccess>]
module FsRandom.Seq

open FsRandom.StateMonad

/// <summary>
/// Makes infinite sequence of random numbers.
/// </summary>
/// <param name="generator">A random function.</param>
/// <param name="builder">A random builder.</param>
/// <returns>
/// A function which takes a seed and returns infinite sequence of random numbers.
/// </returns>
val ofRandom : generator:State<PrngState<'s>, 'a> -> builder:RandomBuilder<'s> -> ('s -> seq<'a>)
