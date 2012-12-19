module RecycleBin.Random.Utility

open System

let flipCoin probability =
   ensuresFiniteValue probability "probability"
   if probability < 0.0 || 1.0 < probability
   then
      ArgumentOutOfRangeException ("probability", "`probability' must be in the range of (0, 1).") |> raise
   else
      fun s0 ->
         let u, s' = ``[0, 1)`` s0
         u < probability, s'

let sample n source =
   let size = Array.length source
   if n < 0 || size < n
   then
      ArgumentOutOfRangeException ("n", "`n' must be between 0 and the number of elements in `source`.") |> raise
   else
      fun s0 ->
         let result = Array.zeroCreate n
         let mutable p = size
         let mutable s = s0
         for index = n - 1 downto 0 do
            let mutable probability = 1.0
            let u, s' = ``[0, 1)`` s0
            s <- s'
            while u < probability do
               probability <- probability * float (p - index - 1) / float p
               p <- p - 1
            result.[index] <- source.[size - p - 1]
         result, s

let sampleWithReplacement n source =
   if n < 0
   then
      ArgumentOutOfRangeException ("n", "`n' must not be negative.") |> raise
   else
      fun s0 ->
         let result = Array.zeroCreate n
         let size = Array.length source
         let mutable s = s0
         for index = 0 to n - 1 do
            let u, s' = ``[0, 1)`` s0
            s <- s'
            result.[index] <- source.[int (u * float size)]
         result, s