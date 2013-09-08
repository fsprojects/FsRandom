#r @"..\Build\FsRandom.dll"

open FsRandom

// Coefficients are cited from Wikipedi
let linear x = x, 6364136223846793005uL * x + 1442695040888963407uL
let y = Random.get ``[0, 1)`` <| createState linear 0x123456789ABCDEFuL
printfn "%f" y
