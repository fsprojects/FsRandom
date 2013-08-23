module FsRandom.Seq

open FsRandom.StateMonad

let ofRandom (generator:GeneratorFunction<_, _>) (builder:RandomBuilder<_>) =
   let f = builder { return! generator }
   let rec loop seed = seq {
      let r, next = f seed
      yield r
      yield! loop next
   }
   loop