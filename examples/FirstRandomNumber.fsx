#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

let generator = Statistics.normal (0.0, 1.0)

let z = Random.get generator state
printfn "%f" z

let z1, nextState = Random.next generator state
let z2 = Random.get generator nextState
printfn "%f\n%f" z1 z2
