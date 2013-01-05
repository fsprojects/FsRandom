module RecycleBin.Random.Array

/// <summary>
/// Creates an array whose elements are randomly generated. 
/// </summary>
/// <param name="count">The length of the array to create.</param>
/// <param name="generator">The generator function.</param>
val randomCreate : count:int -> generator:State<PrngState<'s>, 'a> -> State<PrngState<'s>, 'a []>
