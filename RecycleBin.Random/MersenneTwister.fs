﻿module RecycleBin.Random.MersenneTwister

[<Literal>]
let N = 624
[<Literal>]
let M = 397
[<Literal>]
let MatrixA = 0x9908B0DFu
[<Literal>]
let UpperMask = 0x80000000u
[<Literal>]
let LowerMask = 0x7FFFFFFFu

type StateVector (index : int, vector : uint32 []) =
   static member Initialize (seed : uint32) =
      let vector = Array.zeroCreate N
      vector.[0] <- seed &&& 0xFFFFFFFFu
      for index = 1 to N - 1 do
         let previous = vector.[index - 1]
         vector.[index] <- 1812433253u * (previous ^^^ (previous >>> 30)) + uint32 index
      StateVector (N, vector)
   static member Initialize (seed : uint32 []) =
      let state = StateVector.Initialize (19650218u)
      let mutable i = 1
      let mutable j = 0
      for k = (if N > seed.Length then N else seed.Length) downto 1 do
         state.[i] <- state.[i] ^^^ ((state.[i - 1] ^^^ (state.[i - 1] >>> 30)) * 1664525u) + seed.[j] + uint32 j
         i <- i + 1
         j <- j + 1
         if i >= N
         then
            state.[0] <- state.[N - 1]
            i <- 1
         if j >= seed.Length
         then
            j <- 0
      for k = N - 1 downto 1 do
         state.[i] <- state.[i] ^^^ ((state.[i - 1] ^^^ (state.[i - 1] >>> 30)) * 1566083941u) - uint32 i
         i <- i + 1
         if i >= N
         then
            state.[0] <- state.[N - 1]
            i <- 1
      state.[0] <- 0x80000000u
      state
   member val Index = index with get, set
   member this.Item
      with get (index) = vector.[index]
      and set index value = vector.[index] <- value
   member this.CopyVector () = Array.copy vector
   
let inline twist u l v =
   let y = (u &&& UpperMask) ||| (l &&& LowerMask)
   let mag = if y &&& 1u = 0u then 0u else MatrixA  // mag01[y & 0x1]
   v ^^^ (y >>> 1) ^^^ mag
let refresh (state : StateVector) =
   let vector = state.CopyVector ()
   for kk = 0 to N - M - 1 do
      vector.[kk] <-  twist vector.[kk] vector.[kk + 1] vector.[kk + M]
   for kk = N - M to N - 2 do
      vector.[kk] <- twist vector.[kk] vector.[kk + 1] vector.[kk + (M - N)]
   vector.[N - 1] <- twist vector.[N - 1] vector.[0] vector.[M - 1]
   StateVector (0, vector)

let mersennePrng (state : StateVector) =
   let state = if state.Index >= N then refresh state else state
   let index = state.Index
   let mutable y = state.[index]
   y <- y ^^^ (y >>> 11)
   y <- y ^^^ ((y <<< 7) &&& 0x9D2C5680u)
   y <- y ^^^ ((y <<< 15) &&& 0xEFC60000u)
   y <- y ^^^ (y >>> 18)
   state.Index <- index + 1
   y, state
let mersenne seed = random mersennePrng seed
