module FsRandom.MersenneTwister

/// <summary>
/// Keeps a random state used in the Mersenne Twister process.
/// </summary>
[<Class>]
type StateVector =
   /// <summary>
   /// Initializes a new instance with an integer.
   /// </summary>
   /// <param name="seed">A random seed integer.</param>
   static member Initialize : seed:uint64 -> StateVector
   /// <summary>
   /// Initializes a new instance with integers.
   /// </summary>
   /// <param name="seed">A random seed array.</param>
   static member Initialize : seed:uint64 [] -> StateVector
   
/// <summary>
/// Random number generator using Mersenne Twister algorithm (Matsumoto &amp; Nishimura 1998).
/// </summary>
val mersenne : Prng<StateVector>
