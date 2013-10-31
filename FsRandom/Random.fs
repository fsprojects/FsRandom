module FsRandom.Random

let inline next (generator:GeneratorFunction<_>) = runRandom generator
let inline get (generator:GeneratorFunction<_>) = evaluateRandom generator

let inline singleton x = returnRandom x
let inline identity (generator:GeneratorFunction<_>) = generator
let inline transformBy f (generator:GeneratorFunction<_>) =
   fun s0 -> let r, s' = generator s0 in f r, s'
let inline transformBy2 f (g1:GeneratorFunction<_>) (g2:GeneratorFunction<_>) =
   fun s0 ->
      let r1, s1 = g1 s0
      let r2, s2 = g2 s1
      f r1 r2, s2
let inline transformBy3 f (g1:GeneratorFunction<_>) (g2:GeneratorFunction<_>) (g3:GeneratorFunction<_>) =
   fun s0 ->
      let r1, s1 = g1 s0
      let r2, s2 = g2 s1
      let r3, s3 = g3 s2
      f r1 r2 r3, s3

let inline zip g1 g2 = transformBy2 tuple g1 g2
let inline zip3 g1 g2 g3 = transformBy3 tuple3 g1 g2 g3
let inline merge gs = List.foldBack (transformBy2 cons) gs (returnRandom [])
let inline mergeWith f gs = merge gs |> transformBy f
