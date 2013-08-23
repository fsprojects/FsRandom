module FsRandom.Array

open System

let swap i j (array:'a []) =
   let temp = array.[i]
   array.[i] <- array.[j]
   array.[j] <- temp

let randomCreate count (generator:GeneratorFunction<_, _>) =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      fun s0 ->
         let result = Array.zeroCreate count
         let mutable s0 = s0
         for index = 0 to count - 1 do
            let r, s' = generator s0
            result.[index] <- r
            s0 <- s'
         result, s0

let randomInit count (initializer:int -> GeneratorFunction<_, _>) =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      fun s0 ->
         let result = Array.zeroCreate count
         let mutable s0 = s0
         for index = 0 to count - 1 do
            let r, s' = initializer index s0
            result.[index] <- r
            s0 <- s'
         result, s0

let shuffleInPlace array =
   fun s0 ->
      let mutable s0 = s0
      for index = Array.length array - 1 downto 1 do
         let u, s' = ``[0, 1)`` s0
         s0 <- s'
         let randomIndex = int <| u * float (index + 1)
         swap index randomIndex array
      (), s0

let shuffle array =
   fun s0 ->
      let copiedArray = Array.copy array
      let _, s' = shuffleInPlace copiedArray s0
      copiedArray, s'

let sample n source =
   let size = Array.length source
   if n < 0 || size < n then
      outOfRange "n" "`n' must be between 0 and the number of elements in `source`."
   else
      fun s0 ->
         let result = Array.zeroCreate n
         let mutable p = size
         let mutable s0 = s0
         for index = n - 1 downto 0 do
            let mutable probability = 1.0
            let u, s' = ``[0, 1)`` s0
            s0 <- s'
            while u < probability do
               probability <- probability * float (p - index - 1) / float p
               p <- p - 1
            result.[index] <- source.[size - p - 1]
         result, s0

let weightedSample n weight source =
   let size = Array.length source
   if n < 0 || size < n then
      outOfRange "n" "`n' must be between 0 and the number of elements in `source`."
   elif Array.length weight <> size then
      invalidArg "weight" "`weight' must have the same length of `source'."
   else
      // Efraimidis and Spirakis (2006) Weighted random sampling with a reservoir (DOI: 10.1016/j.ipl.2005.11.003)
      random {
         let result = ref BinarySearchTree.empty
         for index = 0 to n - 1 do
            let! u = ``[0, 1)``
            let key = u ** (1.0 / weight.[index])
            result := BinarySearchTree.insert key source.[index] !result
         let index = ref n
         while !index < size do
            let threshold = BinarySearchTree.min !result |> fst
            let! u = ``[0, 1)``
            let x = log u / log threshold
            let weightSum = ref 0.0
            while !index < size && !weightSum < x do
               weightSum := !weightSum + weight.[!index]
               incr index
            if !weightSum >= x then
               let index = !index - 1
               let w = weight.[index]
               let! u = ``[0, 1)``
               let r = let t = threshold ** w in t + u * (1.0 - t)
               let key = r ** (1.0 / w)
               result := BinarySearchTree.removeMinimum !result |> BinarySearchTree.insert key source.[index]
         return !result |> (BinarySearchTree.toSeq >> Seq.map snd >> Seq.toArray)
      }

let sampleWithReplacement n source =
   if n < 0 then
      outOfRange "n" "`n' must not be negative."
   else
      fun s0 ->
         let result = Array.zeroCreate n
         let size = float <| Array.length source
         let mutable s0 = s0
         for index = 0 to n - 1 do
            let u, s' = ``[0, 1)`` s0
            s0 <- s'
            result.[index] <- source.[int (u * size)]
         result, s0

let weightedSampleWithReplacement n weight source =
   let size = Array.length source
   if n < 0 then
      outOfRange "n" "`n' must not be negative."
   elif Array.length weight <> size then
      invalidArg "weight" "`weight' must have the same length of `source'."
   else
      fun s0 ->
         let result = Array.zeroCreate n
         let cdf = Array.accumulate (+) weight
         let sum = cdf.[size - 1]
         let mutable s0 = s0
         for index = 0 to n - 1 do
            let u, s' = ``[0, 1)`` s0
            s0 <- s'
            let p = sum * u
            let resultIndex = Array.findIndex (fun x -> p < x) cdf
            result.[index] <- source.[resultIndex]
         result, s0
