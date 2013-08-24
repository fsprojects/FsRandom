#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u

let plusOne x = x + 1.0
xorshift {
   return! getRandomBy plusOne <| Statistics.uniform (0.0, 1.0)
} <| seed
|> fst
|> printfn "%f"

xorshift {
   let! u = getRandom <| Statistics.uniform (0.0, 1.0)
   return plusOne u
} <| seed
|> fst
|> printfn "%f"

xorshift {
   let! u = Statistics.uniform (0.0, 1.0)
   return plusOne u
} <| seed
|> fst
|> printfn "%f"
