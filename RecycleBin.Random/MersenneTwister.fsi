[<AutoOpen>]
module RecycleBin.Random.MersenneTwister

/// <summary>
/// Keeps a random state used in the Mersenne Twister process.
/// </summary>
/// <seealso cref="initialize" />
[<Class>]
type StateVector =
   static member Initialize : seed:uint32 -> StateVector
   static member Initialize : seed:uint32 [] -> StateVector
   
/// <summary>
/// Random number generator using Mersenne Twister algorithm (Matsumoto &amp; Nishimura 1998).
/// </summary>
/// <param name="seed">Random seed composed of four 32-bit unsigned integers.</param>
val mersenne : StateVector -> RandomBuilder<StateVector>
