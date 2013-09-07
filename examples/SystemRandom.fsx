#r @"..\Build\FsRandom.dll"

open FsRandom

let r0 = System.Random ()
let s = createState systemrandom r0

printfn "%f" <| Random.get ``[0, 1)`` s
printfn "%f" <| Random.get ``[0, 1)`` s
