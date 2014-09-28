/// <summary>
/// Provides utility functions.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Utility

/// <summary>
/// Provides a default random state.
/// </summary>
[<CompiledName("DefaultState")>]
val defaultState : PrngState
/// <summary>
/// Creates a random random state.
/// </summary>
[<CompiledName("CreateRandomState")>]
val createRandomState : unit -> PrngState

/// <summary>
/// Returns +1 or -1 randomly.
/// </summary>
//val randomSign : GeneratorFunction<int>
[<CompiledName("RandomSign")>]
val inline randomSign : unit -> GeneratorFunction<(^a)>
   when ^a : (static member One : ^a)
   and ^a : (static member (~-) : ^a -> ^a)

/// <summary>
/// Returns a random Boolean value with the specified probability.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="Statistics.bernoulli" />
[<CompiledName("FlipCoin")>]
val flipCoin : probability:float -> GeneratorFunction<bool>

/// <summary>
/// Returns random indices of collections.
/// </summary>
/// <param name="size">The size of collections.</param>
/// <param name="count">The number of indices to take.</param>
[<CompiledName("Choose")>]
val choose : size:int -> count:int -> GeneratorFunction<int list>

/// <summary>
/// Returns a random number less than the specified value.
/// </summary>
/// <param name="upper">The exclusive upper bound.</param>
[<CompiledName("ChooseOne")>]
val chooseOne : upper:int -> GeneratorFunction<int>
