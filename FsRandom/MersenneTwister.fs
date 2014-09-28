module FsRandom.MersenneTwister

[<Literal>]
let N = 312
[<Literal>]
let M = 156
[<Literal>]
let MatrixA = 0xB5026F5AA96619E9uL
[<Literal>]
let UpperMask = 0xFFFFFFFF80000000uL
[<Literal>]
let LowerMask = 0x7FFFFFFFuL

let initialize seed =
   let vector = Array.zeroCreate N
   vector.[0] <- seed
   for index = 1 to N - 1 do
      let previous = vector.[index - 1]
      vector.[index] <- 6364136223846793005uL * (previous ^^^ (previous >>> 62)) + uint64 index
   vector

type StateVector (index : int, vector : uint64 []) =
   static member Initialize (seed : uint64) =
      StateVector (N, initialize seed)
   static member Initialize (seed : uint64 []) =
      let vector = initialize 19650218uL
      let mutable i = 1
      let mutable j = 0
      for k = max N seed.Length downto 1 do
         vector.[i] <- (vector.[i] ^^^ ((vector.[i - 1] ^^^ (vector.[i - 1] >>> 62)) * 3935559000370003845uL)) + seed.[j] + uint64 j
         i <- i + 1
         j <- j + 1
         if i >= N then
            vector.[0] <- vector.[N - 1]
            i <- 1
         if j >= seed.Length then
            j <- 0
         ()
      for k = N - 1 downto 1 do
         vector.[i] <- (vector.[i] ^^^ ((vector.[i - 1] ^^^ (vector.[i - 1] >>> 62)) * 2862933555777941757uL)) - uint64 i
         i <- i + 1
         if i >= N then
            vector.[0] <- vector.[N - 1]
            i <- 1
      vector.[0] <- 1uL <<< 63
      StateVector (N, vector)
   member val Index = index with get
   member this.Item (index) = vector.[index]
   member this.Vector = vector
   
let inline twist u l v =
   let y = (u &&& UpperMask) ||| (l &&& LowerMask)
   let mag = if y &&& 1uL = 0uL then 0uL else MatrixA  // mag01[y & 0x1]
   v ^^^ (y >>> 1) ^^^ mag
let refresh (state : StateVector) =
   let vector = Array.copy state.Vector
   for kk = 0 to N - M - 1 do
      vector.[kk] <-  twist vector.[kk] vector.[kk + 1] vector.[kk + M]
   for kk = N - M to N - 2 do
      vector.[kk] <- twist vector.[kk] vector.[kk + 1] vector.[kk + (M - N)]
   vector.[N - 1] <- twist vector.[N - 1] vector.[0] vector.[M - 1]
   StateVector (0, vector)

[<CompiledName("MersenneTwisterPrng")>]
let mersenne (state : StateVector) =
   let state = if state.Index >= N then refresh state else state
   let index = state.Index
   let mutable y = state.[index]
   y <- y ^^^ ((y >>> 29) &&& 0x5555555555555555uL)
   y <- y ^^^ ((y <<< 17) &&& 0x71D67FFFEDA60000uL)
   y <- y ^^^ ((y <<< 37) &&& 0xFFF7EEE000000000uL)
   y <- y ^^^ (y >>> 43)
   // Creates a new instance of StateVector, but the internal vector refers to the same array to avoid cost of copying.
   y, StateVector(index + 1, state.Vector)
