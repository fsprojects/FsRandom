[<AutoOpen>]
module internal FsRandom.RuntimeHelper

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let getDefaultTester () = createState xorshift seed
