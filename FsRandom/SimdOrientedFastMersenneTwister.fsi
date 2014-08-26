/// <summary>
/// Implements SIMD-Oriented Fast Mersenne Twister.
/// See http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/SFMT/index.html for details.
/// </summary>
/// <remarks>
/// Basically, SIMD is not used in this module because of the lack of SIMD support in .NET Framework.
/// </remarks>
module FsRandom.SimdOrientedFastMersenneTwister

/// <summary>
/// Defines parameters to produce a random cycle of SIMD-Oriented Fast Mersenne Twister.
/// </summary>
[<Class>]
type SfmtParams =
   /// <summary>
   /// Parameter for period 2^607 - 1.
   /// </summary>
   static member Params607 : SfmtParams
   /// <summary>
   /// Parameter for period 2^1279 - 1.
   /// </summary>
   static member Params1279 : SfmtParams
   /// <summary>
   /// Parameter for period 2^2281 - 1.
   /// </summary>
   static member Params2281 : SfmtParams 
   /// <summary>
   /// Parameter for period 2^4253 - 1.
   /// </summary>
   static member Params4253 : SfmtParams
   /// <summary>
   /// Parameter for period 2^11213 - 1.
   /// </summary>
   static member Params11213 : SfmtParams
   /// <summary>
   /// Parameter for period 2^19937 - 1.
   /// </summary>
   static member Params19937 : SfmtParams
   /// <summary>
   /// Parameter for period 2^44497 - 1.
   /// </summary>
   static member Params44497 : SfmtParams
   /// <summary>
   /// Parameter for period 2^86243 - 1.
   /// </summary>
   static member Params86243 : SfmtParams
   /// <summary>
   /// Parameter for period 2^132049 - 1.
   /// </summary>
   static member Params132049 : SfmtParams
   /// <summary>
   /// Parameter for period 2^216091 - 1.
   /// </summary>
   static member Params216091 : SfmtParams
   /// <summary>
   /// The approximate random period in log 2 scale.
   /// 2^<see cref="Period" /> - 1 is the exact length.
   /// </summary>
   member Period : int with get

/// <summary>
/// Keeps a random state used in the SIMD-Oriented Fast Mersenne Twister process.
/// </summary>
[<Class>]
type StateVector =
   /// <summary>
   /// Initializes a new instance with an integer.
   /// </summary>
   /// <param name="parameter">A parameter to determine the period of the random sequence.</param>
   /// <param name="seed">A random seed integer.</param>
   static member Initialize : parameter:SfmtParams * seed:uint32 -> StateVector
   /// <summary>
   /// Initializes a new instance with integers.
   /// </summary>
   /// <param name="parameter">A parameter to determine the period of the random sequence.</param>
   /// <param name="seed">A random seed array.</param>
   static member Initialize : parameter:SfmtParams * seed:uint32 [] -> StateVector
   
/// <summary>
/// Random number generator using SIMD-Oriented Fast Mersenne Twister algorithm (Saito &amp; Matsumoto 2006).
/// </summary>
/// <remarks>
/// SIMD is not supported.
/// </remarks>
[<CompiledName("SfmtPrng")>]
val sfmt : Prng<StateVector>
