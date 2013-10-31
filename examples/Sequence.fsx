#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

let rec binaries initialState = seq {
   let binary, nextState = Random.next (Statistics.bernoulli 0.5) initialState
   yield binary
   yield! binaries nextState // recursively generating binaries.
}
binaries state |> Seq.take 20 |> Seq.iter (printf "%d")
printfn ""

let binaries2 = Seq.ofRandom (Statistics.bernoulli 0.5)
binaries2 state |> Seq.take 20 |> Seq.iter (printf "%d")
printfn ""
