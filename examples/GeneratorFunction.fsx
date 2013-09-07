#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

let approximatelyStandardNormal = random {
   let! values = Array.randomCreate 12 ``(0, 1)``  // ``(0, 1)`` is a standard random number generator in (0, 1)
   return Array.sum values - 6.0
}

let z = Random.get approximatelyStandardNormal state
printfn "%f" z
