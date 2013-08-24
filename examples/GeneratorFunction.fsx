#r @"..\Build\FsRandom.dll"

open FsRandom

let approximatelyStandardNormal = random {
   let! values = Array.randomCreate 12 ``(0, 1)``  // ``(0, 1)`` is a standard random number generator in (0, 1)
   return Array.sum values - 6.0
}

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let generator = xorshift { return! approximatelyStandardNormal }
let z = fst <| generator seed
printfn "%f" z
