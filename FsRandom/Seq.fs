module FsRandom.Seq

let ofRandom (generator:GeneratorFunction<_, _>) =
   let f = Random.next generator
   let rec loop seed = seq {
      let r, next = f seed
      yield r
      yield! loop next
   }
   loop
