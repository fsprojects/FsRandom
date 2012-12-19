module RecycleBin.Random.Utility

/// <summary>
/// Returns a random Boolean value with the specified probability.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="bernoulli" />
val flipCoin : probability:float -> State<PrngState<'s>, bool>
