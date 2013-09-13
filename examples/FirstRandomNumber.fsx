#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u

let generator = Statistics.normal (0.0, 1.0)

let z = Random.get generator xorshift seed
printfn "%f" z

let z1, nextSeed = Random.next generator xorshift seed
let z2 = Random.get generator xorshift seed
printfn "%f\n%f" z1 z2
