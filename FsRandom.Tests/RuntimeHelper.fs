[<AutoOpen>]
module internal FsRandom.RuntimeHelper

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let getDefaultTester () = xorshift, seed
let nextRandom g (prng, seed) = Random.next prng g seed
let getRandom g (prng, seed) = Random.get prng g seed
let seqRandom g (prng, seed) = Seq.ofRandom prng g seed
