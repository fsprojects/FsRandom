module RecycleBin.Random.Utility

/// <summary>
/// Returns a random Boolean value with the specified probability.
/// </summary>
/// <param name="probability">The probability of success.</param>
/// <seealso cref="bernoulli" />
val flipCoin : probability:float -> State<PrngState<'s>, bool>

/// <summary>
/// Picks up random samples without replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <seealso cref="sampleWithReplacement" />
val sample : n:int -> source:'a [] -> State<PrngState<'s>, 'a []>

/// <summary>
/// Picks up random samples with replacement in the specified array.
/// </summary>
/// <param name="n">The number of samples to pick up.</param>
/// <param name="source">The source array.</param>
/// <seealso cref="sample" />
val sampleWithReplacement : n:int -> source:'a [] -> State<PrngState<'s>, 'a []>
