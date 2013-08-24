#r @"..\Build\FsRandom.dll"

open FsRandom
open FsRandom.Statistics

let gibbsBinormal (meanX, meanY, varX, varY, cov) (_ : float, y : float) = random {
   let! x' = normal (meanX + cov * (y - meanY) / varY, sqrt <| varX - cov ** 2.0 / varY)
   let! y' = normal (meanY + cov * (x' - meanX) / varX, sqrt <| varY - cov ** 2.0 / varX)
   return (x', y')
}
let binormal parameter = Seq.markovChain (gibbsBinormal parameter)

module Seq =
   let takeBy n (source:seq<'a>) = seq {
      use e = source.GetEnumerator ()
      while e.MoveNext () do
         yield e.Current
         let skip = ref (n - 1)
         while !skip > 0 && e.MoveNext () do
            decr skip
   }

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let parameter = (0.0, 0.0, 1.0, 1.0, 0.7)
let initialPoint = (0.0, 0.0)
let sampler =
   binormal parameter xorshift seed initialPoint
   |> Seq.skip 100  // burn-in
   |> Seq.takeBy 20  // to avoid autocorrelation

sampler
|> Seq.take 50
|> Seq.iter (fun (x, y) -> printfn "(%6.3f, %6.3f)" x y)
