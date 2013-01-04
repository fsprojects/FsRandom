module RecycleBin.Random.RandomNumberGenerator

open System
open RecycleBin.Random.StateMonad

type Prng<'s> = 's -> uint32 * 's
type PrngState<'s> = Prng<'s> * 's

type RandomBuilder<'s> (initial:PrngState<'s>) =
   inherit StateBuilder ()
   member this.Run (m) = m initial |> (fun (random, (_, state) : PrngState<'s>) -> random, state)
let random prng seed = RandomBuilder (prng, seed)

let buffer = Array.zeroCreate sizeof<uint32>
let systemrandomPrng (random : Random) = 
   random.NextBytes (buffer)
   BitConverter.ToUInt32 (buffer, 0), random
let systemrandom = random systemrandomPrng
   
let xorshiftPrng (x:uint32, y:uint32, z:uint32, w:uint32) =
   let t = x ^^^ (x <<< 11)
   let (_, _, _, w') as s = y, z, w, (w ^^^ (w >>> 19)) ^^^ (t ^^^ (t >>> 8))
   w', s
let xorshift = random xorshiftPrng

let getRandom (generator : State<PrngState<'s>, 'a >) =
   getState |>> (fun s0 -> let r, s' = generator s0 in setState s' &>> returnState r)
let getRandomBy f (generator : State<PrngState<'s>, 'a >) =
   getState |>> (fun s0 -> let r, s' = generator s0 in setState s' &>> returnState (f r))

[<Literal>]
let ``1 / 2^32`` = 2.3283064365386963e-10
[<Literal>]
let ``1 / (2^32 - 1)`` = 2.3283064370807974e-10
let ``(0, 1)`` ((f, s0) : PrngState<'s>) = let r, s' = f s0 in (float r + 0.5) * ``1 / 2^32``, (f, s')
let ``[0, 1)`` ((f, s0) : PrngState<'s>) = let r, s' = f s0 in float r * ``1 / 2^32``, (f, s')
let ``(0, 1]`` ((f, s0) : PrngState<'s>) = let r, s' = f s0 in (float r + 1.0) * ``1 / 2^32``, (f, s')
let ``[0, 1]`` ((f, s0) : PrngState<'s>) = let r, s' = f s0 in float r * ``1 / (2^32 - 1)``, (f, s')
     