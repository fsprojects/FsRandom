module FsRandom.RandomNumberGenerator

open System

type Prng<'s> = 's -> uint64 * 's
type PrngState =
   abstract Next64Bits : unit -> uint64 * PrngState
type GeneratorFunction<'a> = GeneratorFunction of (PrngState -> 'a * PrngState)

[<CompiledName("CreateState")>]
let rec createState (prng:Prng<'s>) (seed:'s) = {
   new PrngState with
      member __.Next64Bits () =
         let r, next = prng seed
         r, createState prng next
}

let inline bindRandom (GeneratorFunction m) f =
   GeneratorFunction (fun s0 -> let v, s' = m s0 in match f v with GeneratorFunction (g) -> g s')
let inline returnRandom x = GeneratorFunction (fun s -> x, s)
let inline runRandom (GeneratorFunction m) x = m x
let inline evaluateRandom (GeneratorFunction m) x = m x |> fst
let inline executeRandom (GeneratorFunction m) x = m x |> snd

let inline (|>>) m f = bindRandom m f
let inline (&>>) m b = bindRandom m (fun _ -> b)

type RandomBuilder () =
   member this.Bind (m, f) = m |>> f
   member this.Combine (a, b) = a &>> b
   member this.Return (x) = returnRandom x
   member this.ReturnFrom (m : GeneratorFunction<_>) = m
   member this.Zero () = GeneratorFunction (fun s -> Unchecked.defaultof<_>, s)
   member this.Delay (f) = returnRandom () |>> f
   member this.While (condition, m:GeneratorFunction<'a>) : GeneratorFunction<'a> =
      if condition () then
         m |>> (fun _ -> this.While (condition, m))
      else
         this.Zero ()
   member this.For (source : seq<'a>, f) =
      use e = source.GetEnumerator ()
      this.While (e.MoveNext, this.Delay (fun () -> f e.Current))
   member this.TryFinally (GeneratorFunction g, finalizer) =
      GeneratorFunction (fun s -> try g s finally finalizer ())
   member this.TryWith (GeneratorFunction g, handler) =
      GeneratorFunction (fun s -> try g s with ex -> let (GeneratorFunction h) = handler ex in h s)
   member this.Using (x:#IDisposable, f) =
      this.TryFinally (f x, fun () -> using x ignore)
let random = RandomBuilder ()

[<CompiledName("SystemRandomPrng")>]
let systemrandom (random : Random) =
   let lower  = (uint64 (random.Next ())       ) &&& 0b0000000000000000000000000000000000000000000011111111111111111111uL
   let middle = (uint64 (random.Next ()) <<< 20) &&& 0b0000000000000000000000111111111111111111111100000000000000000000uL
   let upper  = (uint64 (random.Next ()) <<< 42) &&& 0b1111111111111111111111000000000000000000000000000000000000000000uL
   lower ||| middle ||| upper, random

[<CompiledName("XorshiftPrng")>]   
let xorshift (x:uint32, y:uint32, z:uint32, w:uint32) =
   let s = x ^^^ (x <<< 11)
   let t = y ^^^ (y <<< 11)
   let u = (w ^^^ (w >>> 19)) ^^^ (s ^^^ (s >>> 8))
   let v = (u ^^^ (u >>> 19)) ^^^ (t ^^^ (t >>> 8))
   to64bit u v, (z, w, u, v)

[<CompiledName("RawBits")>]
let rawBits = GeneratorFunction (fun s -> s.Next64Bits ())
[<Literal>]
let ``1 / 2^52`` = 2.22044604925031308084726333618e-16
[<Literal>]
let ``1 / 2^53`` = 1.11022302462515654042363166809e-16
[<Literal>]
let ``1 / (2^53 - 1)`` = 1.1102230246251566636831481e-16
[<CompiledName("StandardExclusive")>]
let ``(0, 1)`` = GeneratorFunction (fun s0 -> let r, s' = s0.Next64Bits () in (float (r >>> 12) + 0.5) * ``1 / 2^52``, s')
[<CompiledName("Standard")>]
let ``[0, 1)`` = GeneratorFunction (fun s0 -> let r, s' = s0.Next64Bits () in float (r >>> 11) * ``1 / 2^53``, s')
[<CompiledName("StandardLowerExclusiveUpperInclusive")>]
let ``(0, 1]`` = GeneratorFunction (fun s0 -> let r, s' = s0.Next64Bits () in (float (r >>> 12) + 1.0) * ``1 / 2^52``, s')
[<CompiledName("StandardInclusive")>]
let ``[0, 1]`` = GeneratorFunction (fun s0 -> let r, s' = s0.Next64Bits () in float (r >>> 11) * ``1 / (2^53 - 1)``, s')
