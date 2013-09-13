module FsRandom.Seq

let ofRandom (generator:GeneratorFunction<_>) =
   let f = runRandom generator
   let rec loop seed = seq {
      let r, next = f seed
      yield r
      yield! loop next
   }
   curry (uncurry createState >> loop)
