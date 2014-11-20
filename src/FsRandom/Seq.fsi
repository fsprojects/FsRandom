/// <summary>
/// Generates sequences for generator functions.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.Seq

/// <summary>
/// Makes infinite sequence of random numbers.
/// </summary>
/// <param name="generator">A random function.</param>
/// <returns>
/// A function which takes a seed and returns infinite sequence of random numbers.
/// </returns>
[<CompiledName("OfRandom")>]
val ofRandom : generator:GeneratorFunction<'a> -> (PrngState -> seq<'a>)
