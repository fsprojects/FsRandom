#r @"..\Build\FsRandom.dll"

open FsRandom

let r0 = System.Random ()
let u, r1 = systemrandom { return! Statistics.gamma (2.0, 1.0) } <| r0
printfn "%b" (r0 = r1)  // true
