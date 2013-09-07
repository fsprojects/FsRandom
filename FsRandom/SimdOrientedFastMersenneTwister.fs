module FsRandom.SimdOrientedFastMersenneTwister

#nowarn "9"
open System.Runtime.InteropServices

module W128 =
   [<Literal>]
   let Size = 4
   [<Struct; StructLayout(LayoutKind.Sequential)>]
   type W128_T =
      [<MarshalAs(UnmanagedType.ByValArray, SizeConst = Size)>]
      val u : uint32 []
      new (u0, u1, u2, u3) = { u = [|u0; u1; u2; u3|] }
   let inline rshift (input : W128_T) shift =
      let th = (uint64 input.u.[3] <<< 32) ||| (uint64 input.u.[2])
      let tl = (uint64 input.u.[1] <<< 32) ||| (uint64 input.u.[0])
      let s = shift * 8
      let oh = th >>> s
      let ol = let ol = tl >>> s in ol ||| (th <<< (64 - s))
      let u1 = uint32 (ol >>> 32)
      let u0 = uint32 ol
      let u3 = uint32 (oh >>> 32)
      let u2 = uint32 oh
      W128_T (u0, u1, u2, u3)
   let inline lshift (input : W128_T) shift =
      let th = (uint64 input.u.[3] <<< 32) ||| (uint64 input.u.[2])
      let tl = (uint64 input.u.[1] <<< 32) ||| (uint64 input.u.[0])
      let s = shift * 8
      let oh = let oh = th <<< s in oh ||| (tl >>> (64 - s))
      let ol = tl <<< s
      let u1 = uint32 (ol >>> 32)
      let u0 = uint32 ol
      let u3 = uint32 (oh >>> 32)
      let u2 = uint32 oh
      W128_T (u0, u1, u2, u3)
   module Array =
      let get index (vector : W128_T []) = vector.[index / Size].u.[index % Size]
      let set index value (vector : W128_T []) = vector.[index / Size].u.[index % Size] <- value
      let update index f (vector : W128_T []) = set index (f (get index vector)) vector
      let zeroCreate count =
         Array.init count (fun _ -> W128_T (0u, 0u, 0u, 0u))
      let init count initializer =
         Array.init count (fun index -> let u0, u1, u2, u3 = initializer index in W128_T (u0, u1, u2, u3))
      let copy (vector : W128_T []) =
         Array.init (Array.length vector) (fun index -> let u = vector.[index].u in W128_T (u.[0], u.[1], u.[2], u.[3]))

type SfmtParams (mexp, pos1, sl1, sl2, sr1, sr2, mask1, mask2, mask3, mask4, parity1, parity2, parity3, parity4) =
   static member Params607 = SfmtParams (607, 2, 15, 3, 13, 3, 0xFDFF37FFu, 0xEF7F3F7Du, 0xFF777B7Du, 0x7FF7FB2Fu, 0x00000001u, 0x00000000u, 0x00000000u, 0x5986F054u)
   static member Params1279 = SfmtParams (1279, 7, 14, 3, 5, 1, 0xF7FEFFFDu, 0x7FEFCFFFu, 0xAFF3EF3Fu, 0xB5FFFF7Fu, 0x00000001u, 0x00000000u, 0x00000000u, 0x20000000u)
   static member Params2281 = SfmtParams (2281, 12, 19, 1, 5, 1, 0xBFF7FFBFu, 0xFDFFFFFEu, 0xF7FFEF7Fu, 0xF2F7CBBFu, 0x00000001u, 0x00000000u, 0x00000000u, 0x41DFA600u)
   static member Params4253 = SfmtParams (4253, 17, 20, 1, 7, 1, 0x9F7BFFFFu, 0x9FFFFF5Fu, 0x3EFFFFFBu, 0xFFFFF7BBu, 0xA8000001u, 0xAF5390A3u, 0xB740B3F8u, 0x6C11486Du)
   static member Params11213 = SfmtParams (11213, 68, 14, 3, 7, 3, 0xEFFFF7FBu, 0xFFFFFFEFu, 0xDFDFBFFFu, 0x7FFFDBFDu, 0x00000001u, 0x00000000u, 0xE8148000u, 0xD0C7AFA3u)
   static member Params19937 = SfmtParams (19937, 122, 18, 1, 11, 1, 0xDFFFFFEFu, 0xDDFECB7Fu, 0xBFFAFFFFu, 0xBFFFFFF6u, 0x00000001u, 0x00000000u, 0x00000000u, 0x13C9E684u)
   static member Params44497 = SfmtParams (44497, 330, 5, 3, 9, 3, 0XEFFFFFFBu, 0xDFBEBFFFu, 0xBFBF7BEFu, 0x9FFD7BFFu, 0x00000001u, 0x00000000u, 0xA3AC4000u, 0xECC1327Au)
   static member Params86243 = SfmtParams (86243, 366, 6, 7, 19, 1, 0xFDBFFBFFu, 0xBFF7FF3Fu, 0xFD77EFFFu, 0xBF9FF3FFu, 0x00000001u, 0x00000000u, 0x00000000u, 0xE9528D85u)
   static member Params132049 = SfmtParams (132049, 110, 19, 1, 21, 1, 0xFFFFBB5Fu, 0xFB6EBF95u, 0xFFFEFFFAu, 0xCFF77FFFu, 0x00000001u, 0x00000000u, 0xCB520000u, 0xC7E91C7Du)
   static member Params216091 = SfmtParams (216091, 627, 11, 3, 10, 1, 0xBFF7BFF7u, 0xBFFFFFFFu, 0xBFFFFA7Fu, 0xFFDDFBFBu, 0xF8000001u, 0x89E80709u, 0x3BD2B64Bu, 0x0C64B1E4u)
   member val Period : int = mexp
   member val N = mexp / 128 + 1
   member val N32 = W128.Size * (mexp / 128 + 1)
   member val Pos1 : int = pos1
   member val SL1 : int = sl1
   member val SL2 : int = sl2
   member val SR1 : int = sr1
   member val SR2 : int = sr2
   member val Mask1 : uint32 = mask1
   member val Mask2 : uint32 = mask2
   member val Mask3 : uint32 = mask3
   member val Mask4 : uint32 = mask4
   member val Parity1 : uint32 = parity1
   member val Parity2 : uint32 = parity2
   member val Parity3 : uint32 = parity3
   member val Parity4 : uint32 = parity4

let certificatePeriod (parameter : SfmtParams) vector =
   let parity = [|parameter.Parity1; parameter.Parity2; parameter.Parity3; parameter.Parity4|]
   let mutable inner = 0u
   for index = 0 to 3 do
      inner <- inner ^^^ ((W128.Array.get index vector) &&& parity.[index])
   for i in [16; 8; 4; 2; 1] do
      inner <- inner ^^^ (inner >>> i)
   inner <- inner &&& 1u
   if inner <> 1u then
      let incomplete = ref true
      let mutable index = 0
      while !incomplete && index < Array.length parity do
         let mutable j = 0
         let work = ref 1u
         while !incomplete && j < 32 do
            if !work &&& parity.[index] <> 0u then
               W128.Array.update index (fun value -> value ^^^ !work) vector
               incomplete := false
            j <- j + 1
            work := !work <<< 1
         index <- index + 1
   vector

let initialize (parameter : SfmtParams) seed =
   let vector = W128.Array.zeroCreate parameter.N32
   W128.Array.set 0 seed vector
   let mutable value = seed
   for index = 1 to parameter.N32 - 1 do
      value <- 1812433253u * (value ^^^ (value >>> 30)) + uint32 index
      W128.Array.set index value vector
   certificatePeriod parameter vector
let inline func1 x = (x ^^^ (x >>> 27)) * 1664525u
let inline func2 x = (x ^^^ (x >>> 27)) * 1566083941u
let xor i j k vector = W128.Array.get i vector ^^^ W128.Array.get j vector ^^^ W128.Array.get k vector
let plus i j k vector = W128.Array.get i vector + W128.Array.get j vector + W128.Array.get k vector
let initializeByArray (parameter : SfmtParams) seed =
   let vector = W128.Array.init parameter.N32 (fun _ -> 0x8B8B8B8Bu, 0x8B8B8B8Bu, 0x8B8B8B8Bu, 0x8B8B8B8Bu)
   let size = parameter.N32
   let lag = if size >= 623 then 11 elif size >= 68 then 7 elif size >= 39 then 5 else 3
   let mid = (size - lag) / 2
   let mutable r = func1 <| xor 0 mid (size - 1) vector
   W128.Array.update mid ((+) r) vector
   r <- r + uint32 (Array.length seed)
   W128.Array.update (mid + lag) ((+) r) vector
   W128.Array.set 0 r vector
   let count = max (Array.length seed + 1) size - 1
   let mutable i = 1
   let mutable j = 0
   while j < count && j < Array.length seed do
      r <- func1 <| xor i ((i + mid) % size) ((i + size - 1) % size) vector
      W128.Array.update ((i + mid) % size) ((+) r) vector
      r <- r + seed.[j] + uint32 i
      W128.Array.update ((i + mid + lag) % size) ((+) r) vector
      W128.Array.set i r vector
      i <- (i + 1) % size
      j <- j + 1
   while j < count do
      r <- func1 <| xor i ((i + mid) % size) ((i + size - 1) % size) vector
      W128.Array.update ((i + mid) % size) ((+) r) vector
      r <- r + uint32 i
      W128.Array.update ((i + mid + lag) % size) ((+) r) vector
      W128.Array.set i r vector
      i <- (i + 1) % size
      j <- j + 1
   for j = 0 to size - 1 do
      r <- func2 <| plus i ((i + mid) % size) ((i + size - 1) % size) vector
      W128.Array.update ((i + mid) % size) ((^^^) r) vector
      r <- r - uint32 i
      W128.Array.update ((i + mid + lag) % size) ((^^^) r) vector
      W128.Array.set i r vector
      i <- (i + 1) % size
   certificatePeriod parameter vector

type StateVector (parameter : SfmtParams, index : int, vector : W128.W128_T []) =
   static member Initialize (parameter : SfmtParams, seed : uint32) =
      StateVector (parameter, parameter.N32, initialize parameter seed)
   static member Initialize (parameter : SfmtParams, seed : uint32 []) =
      StateVector (parameter, parameter.N32, initializeByArray parameter seed)
   member val Parameter = parameter
   member val Index = index
   member this.Item (index) = vector.[index]
   member this.Vector = vector

let doRecursion index (parameter : SfmtParams) a b c d (vector : W128.W128_T []) =
   let va = vector.[a]
   let vb = vector.[b]
   let vc = vector.[c]
   let vd = vector.[d]
   let x = W128.lshift va parameter.SL2
   let y = W128.rshift vc parameter.SR2
   let u0 = va.u.[0] ^^^ x.u.[0] ^^^ ((vb.u.[0] >>> parameter.SR1) &&& parameter.Mask1) ^^^ y.u.[0] ^^^ (vd.u.[0] <<< parameter.SL1)
   let u1 = va.u.[1] ^^^ x.u.[1] ^^^ ((vb.u.[1] >>> parameter.SR1) &&& parameter.Mask2) ^^^ y.u.[1] ^^^ (vd.u.[1] <<< parameter.SL1)
   let u2 = va.u.[2] ^^^ x.u.[2] ^^^ ((vb.u.[2] >>> parameter.SR1) &&& parameter.Mask3) ^^^ y.u.[2] ^^^ (vd.u.[2] <<< parameter.SL1)
   let u3 = va.u.[3] ^^^ x.u.[3] ^^^ ((vb.u.[3] >>> parameter.SR1) &&& parameter.Mask4) ^^^ y.u.[3] ^^^ (vd.u.[3] <<< parameter.SL1)
   vector.[index] <- W128.W128_T (u0, u1, u2, u3)
let refresh (state : StateVector) =
   let vector = W128.Array.copy state.Vector
   let parameter = state.Parameter
   let n = parameter.N
   let pos1 = parameter.Pos1
   let mutable r1 = n - 2
   let mutable r2 = n - 1
   for index = 0 to n - pos1 - 1 do
      doRecursion index parameter index (index + pos1) r1 r2 vector
      r1 <- r2
      r2 <- index
   for index = n - pos1 to n - 1 do
      doRecursion index parameter index (index + pos1 - n) r1 r2 vector
      r1 <- r2
      r2 <- index
   StateVector (parameter, 0, vector)
   
let sfmtImpl (state : StateVector) =
   let state = if state.Index >= state.Parameter.N32 then refresh state else state
   let index = state.Index
   let vector = state.Vector
   let r = W128.Array.get index vector
   // Creates a new instance of StateVector, but the parameter and the internal vector
   // refers to the same array to avoid cost of copying.
   r, StateVector(state.Parameter, index + 1, vector)
let sfmt (s : StateVector) =
   let lower, s = sfmtImpl s
   let upper, s = sfmtImpl s
   to64bit lower upper, s
