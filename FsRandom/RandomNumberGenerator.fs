module FsRandom.RandomNumberGenerator

open System

type Prng<'s> = 's -> uint64 * 's
type PrngState =
   abstract Seed : obj with get
   abstract Next64Bits : unit -> uint64 * PrngState
type GeneratorFunction<'a> = PrngState -> 'a * PrngState

let rec createState (prng:Prng<'s>) (seed:'s) = {
   new PrngState with
      member __.Seed with get () = box seed
      member __.Next64Bits () =
         let r, next = prng seed
         r, createState prng next
}

let inline bindRandom (m:GeneratorFunction<_>) (f:_ -> GeneratorFunction<_>) = fun s0 -> let v, s' = m s0 in f v s'
let inline returnRandom x = fun (s:PrngState) -> x, s
let inline getSeed (s:PrngState) = unbox s.Seed, s
let inline runRandom (m:GeneratorFunction<_>) x = m x
let inline evaluateRandom (m:GeneratorFunction<_>) x = m x |> fst
let inline executeRandom (m:GeneratorFunction<_>) x = m x |> snd

let inline (|>>) m f = bindRandom m f
let inline (&>>) m b = bindRandom m (fun _ -> b)

type RandomBuilder () =
   member this.Bind (m, f:'a->GeneratorFunction<_>) = m |>> f
   member this.Combine (a:GeneratorFunction<_>, b:GeneratorFunction<_>) = a &>> b
   member this.Return (x):GeneratorFunction<_> = returnRandom x
   member this.ReturnFrom (m : GeneratorFunction<'a>) = m
   member this.Zero () = fun (x:PrngState) -> (), x
   member this.Delay (f):GeneratorFunction<_> = returnRandom () |>> f
   member this.While (condition, m) =
      if condition () then
         m |>> (fun () -> this.While (condition, m))
      else
         this.Zero ()
   member this.For (source : seq<'a>, f) =
      use e = source.GetEnumerator ()
      this.While (e.MoveNext, this.Delay (fun () -> f e.Current))
let random = RandomBuilder ()

let buffer = Array.zeroCreate sizeof<uint64>
let systemrandom (random : Random) = 
   random.NextBytes (buffer)
   BitConverter.ToUInt64 (buffer, 0), random
   
let inline xor128 (x:uint32, y:uint32, z:uint32, w:uint32) =
   let t = x ^^^ (x <<< 11)
   let (_, _, _, w') as s = y, z, w, (w ^^^ (w >>> 19)) ^^^ (t ^^^ (t >>> 8))
   w', s
let xorshift s =
   let lower, s = xor128 s
   let upper, s = xor128 s
   to64bit lower upper, s

let rawBits (s:PrngState) = s.Next64Bits ()
[<Literal>]
let ``1 / 2^52`` = 2.22044604925031308084726333618e-16
[<Literal>]
let ``1 / 2^53`` = 1.11022302462515654042363166809e-16
[<Literal>]
let ``1 / (2^53 - 1)`` = 1.1102230246251566636831481e-16
let ``(0, 1)`` (s0:PrngState) = let r, s' = s0.Next64Bits () in (float (r >>> 12) + 0.5) * ``1 / 2^52``, s'
let ``[0, 1)`` (s0:PrngState) = let r, s' = s0.Next64Bits () in float (r >>> 11) * ``1 / 2^53``, s'
let ``(0, 1]`` (s0:PrngState) = let r, s' = s0.Next64Bits () in (float (r >>> 12) + 1.0) * ``1 / 2^52``, s'
let ``[0, 1]`` (s0:PrngState) = let r, s' = s0.Next64Bits () in float (r >>> 11) * ``1 / (2^53 - 1)``, s'
