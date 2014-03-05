#if INTERACTIVE
#I "../Build"
#r "FsRandom.dll"
#endif

open FsRandom
open FsRandom.Statistics

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

type HiddenSystem = A | B
let switch = function A -> B | B -> A
let observe correctly = function
   | A -> if correctly then 'A' else 'B'
   | B -> if correctly then 'B' else 'A'
let model (theta, gamma, length) = random {
   let state = ref A
   let builder = System.Text.StringBuilder ()
   for index = 1 to length do
      let! switchState = Utility.flipCoin theta
      if switchState then state := switch !state
      let! correctly = Utility.flipCoin gamma
      builder.Append (observe correctly !state) |> ignore
   return builder.ToString ()
}

let observed = "AAAABAABBAAAAAABAAAA"
let prior = uniform (0.0, 1.0)  // try `beta (6.0, 13.0)` for better acceptance ratio
let gamma = 0.8

let w (data:string) =
   Seq.windowed 2 data
   |> Seq.filter (fun c -> c.[0] <> c.[1])  // switch
   |> Seq.length
let rho simulated = abs (w observed - w simulated)

/// Datail to output
type SimulationDetail = {
   Theta : float
   SimulatedData : string
   SummaryStatistic : int
   Distance : int
   Accepted : bool
}
let simulate epsilon = random {
   let! theta = prior
   let! simulated = model (theta, gamma, String.length observed)
   let distance = rho simulated
   return {
      Theta = theta
      SimulatedData = simulated
      SummaryStatistic = w simulated
      Distance = distance
      Accepted = distance <= epsilon 
   }
}

let n = 25
let epsilon = 2
printfn "epsilon = %d" epsilon
printfn "index  theta        simulated data  summary  rho   outcome"
Seq.ofRandom (simulate epsilon) state
|> Seq.take n
|> Seq.iteri (fun index detail ->
   printfn "%5d  %.3f  %s  %7d  %3d  %7s"
   <| index + 1
   <| detail.Theta
   <| detail.SimulatedData
   <| detail.SummaryStatistic
   <| detail.Distance
   <| if detail.Accepted then "accepted" else "rejected"
)
