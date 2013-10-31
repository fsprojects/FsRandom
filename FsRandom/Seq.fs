module FsRandom.Seq

let ofRandom (generator:GeneratorFunction<_, _>) =
   let f = Random.next generator
   fun s0 -> seq {
      let s = ref s0
      while true do
         let r, s' = f !s
         yield r
         s := s'
   }
