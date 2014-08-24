[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Random

let bind f m = bindRandom m f
let next generator s = runRandom generator s
let get generator s = evaluateRandom generator s

let singleton x = returnRandom x
let identity (generator:GeneratorFunction<_>) = generator
let map f generator =
   GeneratorFunction (fun s0 -> let r, s' = next generator s0 in f r, s')
let map2 f g1 g2 =
   GeneratorFunction (fun s0 ->
      let r1, s1 = next g1 s0
      let r2, s2 = next g2 s1
      f r1 r2, s2
   )
let map3 f g1 g2 g3 =
   GeneratorFunction (fun s0 ->
      let r1, s1 = next g1 s0
      let r2, s2 = next g2 s1
      let r3, s3 = next g3 s2
      f r1 r2 r3, s3
   )
let transformBy f generator = map f generator
let transformBy2 f g1 g2 = map2 f g1 g2
let transformBy3 f g1 g2 g3 = map3 f g1 g2 g3

let zip g1 g2 = map2 tuple g1 g2
let zip3 g1 g2 g3 = map3 tuple3 g1 g2 g3
let merge gs = List.foldBack (map2 cons) gs (returnRandom [])
let mergeWith f gs = merge gs |> map f
