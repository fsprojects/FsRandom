module FsRandom.RandomNumberGenerator

open System
open FsRandom.StateMonad

type Prng<'s> = 's -> uint64 * 's
type PrngState<'s> = Prng<'s> * 's
type GeneratorFunction<'s, 'a> = State<PrngState<'s>, 'a>

let createState (prng:Prng<'s>) (seed:'s) = prng, seed

let random = state

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

let rawBits ((f, s0) : PrngState<_>) = let r, s' = f s0 in r, (f, s')
[<Literal>]
let ``1 / 2^52`` = 2.22044604925031308084726333618e-16
[<Literal>]
let ``1 / 2^53`` = 1.11022302462515654042363166809e-16
[<Literal>]
let ``1 / (2^53 - 1)`` = 1.1102230246251566636831481e-16
let ``(0, 1)`` ((f, s0) : PrngState<_>) = let r, s' = f s0 in (float (r >>> 12) + 0.5) * ``1 / 2^52``, (f, s')
let ``[0, 1)`` ((f, s0) : PrngState<_>) = let r, s' = f s0 in float (r >>> 11) * ``1 / 2^53``, (f, s')
let ``(0, 1]`` ((f, s0) : PrngState<_>) = let r, s' = f s0 in (float (r >>> 12) + 1.0) * ``1 / 2^52``, (f, s')
let ``[0, 1]`` ((f, s0) : PrngState<_>) = let r, s' = f s0 in float (r >>> 11) * ``1 / (2^53 - 1)``, (f, s')
