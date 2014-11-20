[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.List

[<CompiledName("RandomCreate")>]
let randomCreate count generator =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      let rec loop s = function
         | 0 -> [], s
         | n ->
            let r, s1 = Random.next generator s
            let acc, s2 = loop s1  (n - 1)
            r :: acc, s2 
      GeneratorFunction (fun s0 -> loop s0 count)

[<CompiledName("RandomInitialize")>]
let randomInit count initializer =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      let rec loop s = function
         | 0 -> [], s
         | n ->
            let r, s1 = Random.next (initializer (count - n)) s
            let acc, s2 = loop s1  (n - 1)
            r :: acc, s2 
      GeneratorFunction (fun s0 -> loop s0 count)
