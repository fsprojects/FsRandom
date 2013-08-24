#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u

let generator = xorshift { return! Statistics.bernoulli 0.5 }
let rec binaries initialSeed = seq {
   let binary, nextSeed = generator initialSeed
   yield binary
   yield! binaries nextSeed  // recursively generating binaries.
}
binaries seed |> Seq.take 20 |> Seq.iter (printf "%d")
printfn ""

let binaries2 = Seq.ofRandom (Statistics.bernoulli 0.5) xorshift
binaries2 seed |> Seq.take 20 |> Seq.iter (printf "%d")
printfn ""
