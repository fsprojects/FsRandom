module FsRandom.Seq

open FsRandom.StateMonad

let ofRandom (generator:State<PrngState<'s>, 'a>) (builder:RandomBuilder<'s>) =
   let f = builder { return! generator }
   let rec loop seed = seq {
      let r, next = f seed
      yield r
      yield! loop next
   }
   loop