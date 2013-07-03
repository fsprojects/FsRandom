module RecycleBin.Random.Seq

let ofRandom (generator:State<PrngState<'s>, 'a>) (builder:RandomBuilder<'s>) =
   let rec loop seed = seq {
      let r, next = builder { return! generator } <| seed
      yield r
      yield! loop next
   }
   loop