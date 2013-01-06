module RecycleBin.Random.Utility

open System

let flipCoin probability =
   ensuresFiniteValue probability "probability"
   if probability < 0.0 || 1.0 < probability
   then
      ArgumentOutOfRangeException ("probability", "`probability' must be in the range of [0, 1].") |> raise
   else
      fun s0 ->
         let u, s' = ``[0, 1)`` s0
         u < probability, s'
