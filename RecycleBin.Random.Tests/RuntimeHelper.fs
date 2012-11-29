[<AutoOpen>]
module internal RecycleBin.Random.RuntimeHelper

let raw ((f, s0) : PrngState<'s>) = let r, s' = f s0 in r, (f, s')
