module RecycleBin.Random.Array

open System

let randomCreate count (generator:State<PrngState<'s>, 'a>) =
   if count < 0
   then
      ArgumentOutOfRangeException ("count") |> raise
   else
      fun s0 ->
         let result = Array.zeroCreate count
         let mutable s = s0
         for index = 0 to count - 1 do
            let r, s' = generator s
            result.[index] <- r
            s <- s'
         result, s
