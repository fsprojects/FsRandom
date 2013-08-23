module FsRandom.Utility

open System

let flipCoin probability =
   ensuresFiniteValue probability "probability"
   if probability < 0.0 || 1.0 < probability then
      ArgumentOutOfRangeException ("probability", "`probability' must be in the range of [0, 1].") |> raise
   else
      let transform u = u < probability
      getRandomBy transform ``[0, 1)``
