module FsRandom.RandomNumberGenerator

open System

type Prng<'s> = 's -> uint64 * 's
type PrngState<'s> = Prng<'s> * 's
type GeneratorFunction<'s, 'a> = PrngState<'s> -> 'a * PrngState<'s>

let createState (prng:Prng<'s>) (seed:'s) = prng, seed

let inline bindRandom (m:GeneratorFunction<_, _>) (f:_ -> GeneratorFunction<_, _>) = fun s0 -> let v, s' = m s0 in f v s'
let inline returnRandom x = fun (s:PrngState<_>) -> x, s
let inline getRandom ((_, seed):PrngState<_> as s) = seed, s
let inline setRandom (x:PrngState<'s>) = fun (_:PrngState<'s>) -> (), x
let inline runRandom (m:GeneratorFunction<_, _>) x = m x
let inline evaluateRandom (m:GeneratorFunction<_, _>) x = m x |> fst
let inline executeRandom (m:GeneratorFunction<_, _>) x = m x |> snd

let inline (|>>) m f = bindRandom m f
let inline (&>>) m b = bindRandom m (fun _ -> b)

type RandomBuilder () =
   member this.Bind (m, f:'a->GeneratorFunction<_, _>) = m |>> f
   member this.Combine (a:GeneratorFunction<_, _>, b:GeneratorFunction<_, _>) = a &>> b
   member this.Return (x):GeneratorFunction<_, _> = returnRandom x
   member this.ReturnFrom (m : GeneratorFunction<'s, 'a>) = m
   member this.Zero () = fun (x:PrngState<_>) -> (), x
   member this.Delay (f):GeneratorFunction<_, _> = returnRandom () |>> f
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

let rawBits (f, s0) = let r, s' = f s0 in r, createState f s'
[<Literal>]
let ``1 / 2^52`` = 2.22044604925031308084726333618e-16
[<Literal>]
let ``1 / 2^53`` = 1.11022302462515654042363166809e-16
[<Literal>]
let ``1 / (2^53 - 1)`` = 1.1102230246251566636831481e-16
let ``(0, 1)`` (f, s0) = let r, s' = f s0 in (float (r >>> 12) + 0.5) * ``1 / 2^52``, createState f s'
let ``[0, 1)`` (f, s0) = let r, s' = f s0 in float (r >>> 11) * ``1 / 2^53``, createState f s'
let ``(0, 1]`` (f, s0) = let r, s' = f s0 in (float (r >>> 12) + 1.0) * ``1 / 2^52``, createState f s'
let ``[0, 1]`` (f, s0) = let r, s' = f s0 in float (r >>> 11) * ``1 / (2^53 - 1)``, createState f s'
