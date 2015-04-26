[<AutoOpen>]
module internal FsRandom.RuntimeHelper

let curry f x y = f (x, y)
let uncurry f (x, y) = f x y
