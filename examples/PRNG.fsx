#r @"..\Build\FsRandom.dll"

open FsRandom

// linearPrng : uint64 * uint64 -> uint64 -> uint64 * uint64
let linearPrng (a, c) x = x, a * x + c
let linear (a, c) = createRandomBuilder (linearPrng (a, c))

let seed = uint64 System.Environment.TickCount
let myLinear = linear (6364136223846793005uL, 1442695040888963407uL)  // from Wikipedia
let generator = myLinear { return! Statistics.gamma (3.0, 1.0) }
let y, _ = generator seed
printfn "%f" y
