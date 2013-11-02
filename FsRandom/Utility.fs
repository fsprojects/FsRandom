module FsRandom.Utility

open Microsoft.FSharp.Core.LanguagePrimitives

let inline randomSign () =
   let inline g s0 =
      let r, s' = Random.next rawBits s0
      let sign = if r &&& 1uL = 0uL then GenericOne else -GenericOne
      sign, s'
   GeneratorFunction (g)

let flipCoin probability =
   ensuresFiniteValue probability "probability"
   if probability < 0.0 || 1.0 < probability then
      outOfRange "probability" "`probability' must be in the range of [0, 1]."
   else
      let transform u = u < probability
      Random.transformBy transform ``[0, 1)``

let choose m n =
   if m <= 0 then
      outOfRange "m" "`m' must be positive."
   elif n < 0 || m < n then
      outOfRange "n" "`n' must be in the range of [0, m]."
   else
      GeneratorFunction (fun s0 ->
         let rec loop (acc, p, s) = function
            | index when index < 0 ->
               // List.rev is for ascending order.
               // This is undocumented specification, and might be changed in the future.
               List.rev acc, s
            | index ->
               let mutable probability = 1.0
               let mutable p = p
               let u, s' = Random.next ``[0, 1)`` s
               while u < probability do
                  probability <- probability * float (p - index - 1) / float p
                  p <- p - 1
               loop (m - p - 1 :: acc, p, s') (index - 1)
         loop ([], m, s0) (n - 1)
      )