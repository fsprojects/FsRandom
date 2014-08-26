[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Array

open System

let swap i j (array:'a []) =
   let temp = array.[i]
   array.[i] <- array.[j]
   array.[j] <- temp

[<CompiledName("RandomCreate")>]
let randomCreate count generator =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let result = Array.zeroCreate count
         let mutable s0 = s0
         for index = 0 to count - 1 do
            let r, s' = Random.next generator s0
            result.[index] <- r
            s0 <- s'
         result, s0
      )

[<CompiledName("RandomInitialize")>]
let randomInit count initializer =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let result = Array.zeroCreate count
         let mutable s0 = s0
         for index = 0 to count - 1 do
            let r, s' = Random.next (initializer index) s0
            result.[index] <- r
            s0 <- s'
         result, s0
      )

[<CompiledName("RandomFill")>]
let randomFill (array:'a []) targetIndex count generator =
   if count < 0 then
      outOfRange "count" "`count' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let mutable s0 = s0
         for index = targetIndex to targetIndex + count - 1 do
            let r, s' = Random.next generator s0
            array.[index] <- r
            s0 <- s'
         (), s0
      )

[<CompiledName("ShuffleInPlace")>]
let shuffleInPlace array =
   GeneratorFunction (fun s0 ->
      let mutable s0 = s0
      for index = Array.length array - 1 downto 1 do
         let u, s' = Random.next ``[0, 1)`` s0
         s0 <- s'
         let randomIndex = int <| u * float (index + 1)
         swap index randomIndex array
      (), s0
   )

[<CompiledName("Shuffle")>]
let shuffle array =
   GeneratorFunction (fun s0 ->
      let copiedArray = Array.copy array
      let _, s' = Random.next (shuffleInPlace copiedArray) s0
      copiedArray, s'
   )

[<CompiledName("Sample")>]
let sample n source =
   let size = Array.length source
   if n < 0 || size < n then
      outOfRange "n" "`n' must be between 0 and the number of elements in `source`."
   else
      GeneratorFunction (fun s0 ->
         let result = Array.zeroCreate n
         let mutable p = size
         let mutable s0 = s0
         for index = n - 1 downto 0 do
            let mutable probability = 1.0
            let u, s' = Random.next ``[0, 1)`` s0
            s0 <- s'
            while u < probability do
               probability <- probability * float (p - index - 1) / float p
               p <- p - 1
            result.[index] <- source.[size - p - 1]
         result, s0
      )

[<CompiledName("WeightedSample")>]
let weightedSample n weight source =
   let size = Array.length source
   if n < 0 || size < n then
      outOfRange "n" "`n' must be between 0 and the number of elements in `source`."
   elif Array.length weight <> size then
      invalidArg "weight" "`weight' must have the same length of `source'."
   else
      // Efraimidis and Spirakis (2006) Weighted random sampling with a reservoir (DOI: 10.1016/j.ipl.2005.11.003)
      GeneratorFunction (fun s0 ->
         let s = ref s0
         let result = ref BinarySearchTree.empty
         for index = 0 to n - 1 do
            let u, s' = Random.next ``[0, 1)`` !s
            s := s'
            let key = u ** (1.0 / weight.[index])
            result := BinarySearchTree.insert key source.[index] !result
         let index = ref n
         while !index < size do
            let threshold = BinarySearchTree.min !result |> fst
            let u, s' = Random.next ``[0, 1)`` !s
            s := s'
            let x = log u / log threshold
            let weightSum = ref 0.0
            while !index < size && !weightSum < x do
               weightSum := !weightSum + weight.[!index]
               incr index
            if !weightSum >= x then
               let index = !index - 1
               let w = weight.[index]
               let u, s' = Random.next ``[0, 1)`` !s
               s := s'
               let r = let t = threshold ** w in t + u * (1.0 - t)
               let key = r ** (1.0 / w)
               result := BinarySearchTree.removeMinimum !result |> BinarySearchTree.insert key source.[index]
         !result |> (BinarySearchTree.toList >> List.map snd >> List.toArray), !s
      )

[<CompiledName("SampleWithReplacement")>]
let sampleWithReplacement n source =
   let size = Array.length source
   if n < 0 then
      outOfRange "n" "`n' must not be negative."
   elif size = 0 then
      invalidArg "source" "empty array."
   else
      GeneratorFunction (fun s0 ->
         let result = Array.zeroCreate n
         let size = float <| Array.length source
         let mutable s0 = s0
         for index = 0 to n - 1 do
            let u, s' = Random.next ``[0, 1)`` s0
            s0 <- s'
            result.[index] <- source.[int (u * size)]
         result, s0
      )

[<CompiledName("WeightedSampleWithReplacement")>]
let weightedSampleWithReplacement n weight source =
   let size = Array.length source
   if n < 0 then
      outOfRange "n" "`n' must not be negative."
   elif size = 0 then
      invalidArg "source" "empty array."
   elif Array.length weight <> size then
      invalidArg "weight" "`weight' must have the same length of `source'."
   else
      GeneratorFunction (fun s0 ->
         let result = Array.zeroCreate n
         let cdf = Array.accumulate (+) weight
         let sum = cdf.[size - 1]
         let mutable s0 = s0
         for index = 0 to n - 1 do
            let u, s' = Random.next ``[0, 1)`` s0
            s0 <- s'
            let p = sum * u
            let resultIndex = Array.findIndex (fun x -> p < x) cdf
            result.[index] <- source.[resultIndex]
         result, s0
      )
