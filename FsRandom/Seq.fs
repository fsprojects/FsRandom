module FsRandom.Seq

let ofRandom (generator:GeneratorFunction<_, _>) prng =
   let f = curry (runRandom generator) prng
   let rec loop seed = seq {
      let r, (_, next) = f seed
      yield r
      yield! loop next
   }
   loop
