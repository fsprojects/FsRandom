[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Utility

open System
open Microsoft.FSharp.Core.LanguagePrimitives

[<CompiledName("DefaultState")>]
let defaultState = createState xorshift (123456789u, 362436069u, 521288629u, 88675123u)
[<CompiledName("CreateRandomState")>]
let createRandomState () =
   let guid = Guid.NewGuid ()
   let bytes = guid.ToByteArray ()
   let seed = Array.init 4 (fun i -> BitConverter.ToUInt32 (bytes, i * 4))
   createState xorshift (seed.[0], seed.[1], seed.[2], seed.[3])

[<CompiledName("RandomSign")>]
let inline randomSign () =
   let inline g s0 =
      let r, s' = Random.next rawBits s0
      let sign = if r &&& 1uL = 0uL then GenericOne else -GenericOne
      sign, s'
   GeneratorFunction (g)

[<CompiledName("FlipCoin")>]
let flipCoin probability =
   ensuresFiniteValue probability "probability"
   if probability < 0.0 || 1.0 < probability then
      outOfRange "probability" "`probability' must be in the range of [0, 1]."
   else
      let transform u = u < probability
      Random.map transform ``[0, 1)``

[<CompiledName("Choose")>]
let choose m n =
   if m <= 0 then
      outOfRange "size" "`size' must be positive."
   elif n < 0 || m < n then
      outOfRange "count" "`count' must be in the range of [0, size]."
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

[<CompiledName("ChooseOne")>]
let chooseOne n =
   if n <= 0  then
      outOfRange "upper" "`upper' must be positive."
   else
      let n = float n
      Random.map (fun u -> int (u * n)) ``[0, 1)``
