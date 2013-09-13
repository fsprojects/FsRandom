module FsRandom.Random

let inline next (generator:GeneratorFunction<_>) =
   runRandom generator
let inline get (generator:GeneratorFunction<_>) =
   evaluateRandom generator

let inline identity (generator:GeneratorFunction<_>) = generator
let inline transformBy f (generator:GeneratorFunction<_>) =
   fun s0 -> let r, s' = generator s0 in f r, s'
let inline transformBy2 f (g1:GeneratorFunction<_>) (g2:GeneratorFunction<_>) =
   fun s0 ->
      let r1, s1 = g1 s0
      let r2, s2 = g2 s1
      f r1 r2, s2

let inline zip (g1:GeneratorFunction<_>) (g2:GeneratorFunction<_>) =
   fun s ->
      let r1, s = g1 s
      let r2, s = g2 s
      (r1, r2), s
let inline zip3 (g1:GeneratorFunction<_>) (g2:GeneratorFunction<_>) (g3:GeneratorFunction<_>) =
   fun s ->
      let r1, s = g1 s
      let r2, s = g2 s
      let r3, s = g3 s
      (r1, r2, r3), s
let inline merge (gs:GeneratorFunction<_> list) =
   let inline cons x xs = x :: xs
   List.foldBack (transformBy2 cons) gs (returnRandom [])
