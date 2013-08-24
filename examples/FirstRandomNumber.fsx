#r @"..\Build\FsRandom.dll"

open FsRandom

let generator = Statistics.normal (0.0, 1.0)

let initialSeed = 123456789u, 362436069u, 521288629u, 88675123u
let z, nextSeed = xorshift { return! generator } <| initialSeed
printfn "%f" z

let z2, _ = xorshift { return! generator } <| nextSeed
printfn "%f" z2
