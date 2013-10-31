[<AutoOpen>]
module internal FsRandom.RuntimeHelper

let inline curry f x y = f (x, y)
let inline uncurry f (x, y) = f x y
let seed = 123456789u, 362436069u, 521288629u, 88675123u
let getDefaultTester () = createState xorshift seed