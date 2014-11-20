/// <summary>
/// Provides basic operations on 2-dimensional arrays.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.Array2D

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="rowCount">The length of the first dimension of the array.</param>
/// <param name="columnCount">The length of the second dimension of the array.</param>
/// <param name="generator">The generator function.</param>
[<CompiledName("RandomCreate")>]
val randomCreate : rowCount:int -> columnCount:int -> generator:GeneratorFunction<'a> -> GeneratorFunction<'a [,]>

/// <summary>
/// Creates a based array whose elements are randomly generated. 
/// </summary>
/// <param name="rowBase">The base of the first dimension of the array.</param>
/// <param name="columnBase">The base of the second dimension of the array.</param>
/// <param name="rowCount">The length of the first dimension of the array.</param>
/// <param name="columnCount">The length of the second dimension of the array.</param>
/// <param name="generator">The generator function.</param>
[<CompiledName("RandomCreateBased")>]
val randomCreateBased : rowBase:int -> columnBase:int -> rowCount:int -> columnCount:int -> generator:GeneratorFunction<'a> -> GeneratorFunction<'a [,]>

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="rowCount">The length of the first dimension of the array.</param>
/// <param name="columnCount">The length of the second dimension of the array.</param>
/// <param name="initializer">The function to take an index and produce a random number generating function.</param>
[<CompiledName("RandomInitialize")>]
val randomInit : rowCount:int -> columnCount:int -> initializer:(int -> int -> GeneratorFunction<'a>) -> GeneratorFunction<'a [,]>

/// <summary>
/// Creates a based array whose elements are randomly generated. 
/// </summary>
/// <param name="rowBase">The base of the first dimension of the array.</param>
/// <param name="columnBase">The base of the second dimension of the array.</param>
/// <param name="rowCount">The length of the first dimension of the array.</param>
/// <param name="columnCount">The length of the second dimension of the array.</param>
/// <param name="generator">The generator function.</param>
[<CompiledName("RandomInitializeBased")>]
val randomInitBased : rowBase:int -> columnBase:int -> rowCount:int -> columnCount:int -> initializer:(int -> int -> GeneratorFunction<'a>) -> GeneratorFunction<'a [,]>
