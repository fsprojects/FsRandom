[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.List

[<CompiledName("RandomCreate")>]
let randomCreate count generator =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      let rec loop s n cont =
         match n with
            | 0 -> ([], s) |> cont
            | n ->
               Random.next generator s
               |> (fun (r, s1) ->
                  loop s1  (n - 1) (fun (acc, s2) -> (r :: acc, s2) |> cont)
               )
      GeneratorFunction (fun s0 -> loop s0 count id)

[<CompiledName("RandomInitialize")>]
let randomInit count initializer =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      let rec loop s n cont =
         match n with
            | 0 -> ([], s) |> cont
            | n ->
               Random.next (initializer (count - n)) s
               |> (fun (r, s1) ->
                  loop s1  (n - 1) (fun (acc, s2) ->  (r :: acc, s2) |> cont)
               )
      GeneratorFunction (fun s0 -> loop s0 count id)
