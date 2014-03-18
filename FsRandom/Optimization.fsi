module FsRandom.Optimization

open Microsoft.FSharp.Quotations

/// <summary>
/// Optimizes the generator function. NOT IMPLEMENTED YET.
/// </summary>
val optimize : Expr<GeneratorFunction<'a>> -> GeneratorFunction<'a>
