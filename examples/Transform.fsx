#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

let plusOne x = x + 1.0
Random.transformBy plusOne <| Statistics.uniform (0.0, 1.0)
|> Random.get
<| state
|> printfn "%f"

Random.identity <| Statistics.uniform (0.0, 1.0)
|> Random.get
<| state
|> plusOne
|> printfn "%f"

Statistics.uniform (0.0, 1.0)
|> Random.get
<| state
|> plusOne
|> printfn "%f"
