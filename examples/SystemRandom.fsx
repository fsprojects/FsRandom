#r @"..\Build\FsRandom.dll"

open FsRandom

let r0 = System.Random ()

// systemrandom offers a statefun workflow.
// The result should be different.
printfn "%f" <| Random.get ``[0, 1)`` systemrandom r0
printfn "%f" <| Random.get ``[0, 1)`` systemrandom r0
