/// <summary>
/// Provides basic operations on lists.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.List

/// <summary>
/// Creates a list whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="generator">The generator function.</param>
[<CompiledName("RandomCreate")>]
val randomCreate : count:int -> generator:GeneratorFunction<'a> -> GeneratorFunction<'a list>

/// <summary>
/// Creates a list whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="initializer">The function to take an index and produce a random number generating function.</param>
[<CompiledName("RandomInitialize")>]
val randomInit : count:int -> initializer:(int -> GeneratorFunction<'a>) -> GeneratorFunction<'a list>
