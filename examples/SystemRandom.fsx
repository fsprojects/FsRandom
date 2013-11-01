#r "../Build/FsRandom.dll"

open FsRandom

let r0 = System.Random ()
let state = createState systemrandom r0

// systemrandom offers a statefun workflow.
// The result should be different.
printfn "%f" <| Random.get ``[0, 1)`` state
printfn "%f" <| Random.get ``[0, 1)`` state
