module RecycleBin.Random.Utility

open RecycleBin.Random.StateMonad

/// <summary>
/// Returns a random Boolean value with the specified probability.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="Statistics.bernoulli" />
val flipCoin : probability:float -> State<PrngState<'s>, bool>
