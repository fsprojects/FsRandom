module FsRandom.Utility


/// <summary>
/// Returns +1 or -1 randomly.
/// </summary>
val randomSign : GeneratorFunction<int>
//val inline randomSign : GeneratorFunction<(^a)>
//   when ^a : (static member One : ^a)
//   and ^a : (static member (~-) : ^a -> ^a)

/// <summary>
/// Returns a random Boolean value with the specified probability.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="Statistics.bernoulli" />
val flipCoin : probability:float -> GeneratorFunction<bool>

/// <summary>
/// Returns random indices of collections.
/// </summary>
/// <param name="size">The size of collections.</param>
/// <param name="n">The number of indices to take.</param>
val choose : size:int -> n:int -> GeneratorFunction<int list>
