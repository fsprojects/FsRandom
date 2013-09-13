#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u

let rec binaries prng initialSeed = seq {
   let binary, nextSeed = Random.next (Statistics.bernoulli 0.5) prng initialSeed
   yield binary
   yield! binaries prng nextSeed // recursively generating binaries.
}
binaries xorshift seed |> Seq.take 20 |> Seq.iter (printf "%d")
printfn ""

let binaries2 = Seq.ofRandom (Statistics.bernoulli 0.5)
binaries2 xorshift seed |> Seq.take 20 |> Seq.iter (printf "%d")
printfn ""
