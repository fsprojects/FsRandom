namespace FsRandom

open System
open System.ComponentModel
open System.Runtime.CompilerServices

/// <summary>
/// Provides extension methods that allow languages to use LINQ expressions.
/// </summary>
[<EditorBrowsable(EditorBrowsableState.Never)>]
[<Extension>]
module FsRandomExtensions =
   [<Extension>]
   val Select : GeneratorFunction<'a> -> Func<'a, 'b> -> GeneratorFunction<'b>
   [<Extension>]
   val SelectMany : GeneratorFunction<'a> -> Func<'a, GeneratorFunction<'b>> -> Func<'a, 'b, 'c> -> GeneratorFunction<'c>
