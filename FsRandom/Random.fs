[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Random

[<CompiledName("Bind")>]
let bind f m = bindRandom m f
[<CompiledName("Next")>]
let next generator s = runRandom generator s
[<CompiledName("Get")>]
let get generator s = evaluateRandom generator s

[<CompiledName("Singleton")>]
let singleton x = returnRandom x
[<CompiledName("Identity")>]
let identity (generator:GeneratorFunction<_>) = generator
[<CompiledName("Map")>]
let map f generator =
   GeneratorFunction (fun s0 -> let r, s' = next generator s0 in f r, s')
[<CompiledName("Map2")>]
let map2 f g1 g2 =
   GeneratorFunction (fun s0 ->
      let r1, s1 = next g1 s0
      let r2, s2 = next g2 s1
      f r1 r2, s2
   )
[<CompiledName("Map3")>]
let map3 f g1 g2 g3 =
   GeneratorFunction (fun s0 ->
      let r1, s1 = next g1 s0
      let r2, s2 = next g2 s1
      let r3, s3 = next g3 s2
      f r1 r2 r3, s3
   )
[<CompiledName("TransformBy")>]
let transformBy f generator = map f generator
[<CompiledName("TransformBy")>]
let transformBy2 f g1 g2 = map2 f g1 g2
[<CompiledName("TransformBy")>]
let transformBy3 f g1 g2 g3 = map3 f g1 g2 g3

[<CompiledName("Zip")>]
let zip g1 g2 = map2 tuple g1 g2
[<CompiledName("Zip3")>]
let zip3 g1 g2 g3 = map3 tuple3 g1 g2 g3
[<CompiledName("Merge")>]
let merge gs = List.foldBack (map2 cons) gs (returnRandom [])
[<CompiledName("MergeWith")>]
let mergeWith f gs = merge gs |> map f
