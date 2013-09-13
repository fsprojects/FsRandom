[<AutoOpen>]
module internal FsRandom.RuntimeHelper

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let getDefaultTester () = xorshift, seed
let nextRandom g tester = Random.next g <|| tester
let getRandom g tester = Random.get g <|| tester
let seqRandom g tester = Seq.ofRandom g <|| tester
