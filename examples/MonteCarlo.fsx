#r @"..\Build\FsRandom.dll"

open FsRandom

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

// Generates random points on [-1, 1] x [-1, 1].
let randomPointGenerator = random {
   let! x = Statistics.uniform (-1.0, 1.0)
   let! y = Statistics.uniform (-1.0, 1.0)
   return (x, y)
}
// Weight of a point
// If the distance from (0, 0) is equal to or less than 1 (in the unit circle),
// the weight is 4 (because random points are distributed on [-1, 1] x [-1, 1]).
let weight (x, y) = if x * x + y * y <= 1.0 then 4.0 else 0.0
// Function to generate a sequence
let values = Seq.ofRandom (Random.transformBy weight randomPointGenerator)

// Monte Carlo integration
// Generates 1,000,000 random values and the average becomes estimator of pi
values state
|> Seq.take 1000000
|> Seq.average
|> printfn "%f"
