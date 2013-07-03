[<AutoOpen>]
module internal FsRandom.RuntimeHelper

let getDefaultTester () = xorshift, (123456789u, 362436069u, 521288629u, 88675123u)

let raw ((f, s0) : PrngState<'s>) = let r, s' = f s0 in r, (f, s')
