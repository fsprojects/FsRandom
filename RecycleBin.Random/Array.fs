module RecycleBin.Random.Array

open System

let swap i j (array:'a []) =
   let temp = array.[i]
   array.[i] <- array.[j]
   array.[j] <- temp

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

let shuffle array =
   fun s0 ->
      let copiedArray = Array.copy array
      let mutable s = s0
      for index = Array.length copiedArray - 1 downto 1 do
         let u, s' = ``[0, 1)`` s
         s <- s'
         let randomIndex = int <| u * float (index + 1)
         swap index randomIndex copiedArray
      copiedArray, s
