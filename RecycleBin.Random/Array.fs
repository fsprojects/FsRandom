module RecycleBin.Random.Array

open System

let swap i j (array:'a []) =
   let temp = array.[i]
   array.[i] <- array.[j]
   array.[j] <- temp

let randomCreate count (generator:State<PrngState<'s>, 'a>) =
   if count < 0
   then
      ArgumentOutOfRangeException ("count", "`count' must not be negative.") |> raise
   else
      fun s0 ->
         let result = Array.zeroCreate count
         let mutable s0 = s0
         for index = 0 to count - 1 do
            let r, s' = generator s0
            result.[index] <- r
            s0 <- s'
         result, s0

let randomInit count (initializer:int -> State<PrngState<'s>, 'a>) =
   if count < 0
   then
      ArgumentOutOfRangeException ("count", "`count' must not be negative.") |> raise
   else
      fun s0 ->
         let result = Array.zeroCreate count
         let mutable s0 = s0
         for index = 0 to count - 1 do
            let r, s' = initializer index s0
            result.[index] <- r
            s0 <- s'
         result, s0

let shuffle array =
   fun s0 ->
      let copiedArray = Array.copy array
      let mutable s0 = s0
      for index = Array.length copiedArray - 1 downto 1 do
         let u, s' = ``[0, 1)`` s0
         s0 <- s'
         let randomIndex = int <| u * float (index + 1)
         swap index randomIndex copiedArray
      copiedArray, s0

let shuffleInPlace array =
   fun s0 ->
      let mutable s0 = s0
      for index = Array.length array - 1 downto 1 do
         let u, s' = ``[0, 1)`` s0
         s0 <- s'
         let randomIndex = int <| u * float (index + 1)
         swap index randomIndex array
      (), s0

let sample n source =
   let size = Array.length source
   if n < 0 || size < n
   then
      ArgumentOutOfRangeException ("n", "`n' must be between 0 and the number of elements in `source`.") |> raise
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
   if n < 0 || size < n
   then
      ArgumentOutOfRangeException ("n", "`n' must be between 0 and the number of elements in `source`.") |> raise
   elif Array.length weight <> size
   then
      invalidArg "weight" "`weight' must have the same length of `source'."
   else
      // Efraimidis and Spirakis (2006) Weighted random sampling with a reservoir (DOI: 10.1016/j.ipl.2005.11.003)
      state {
         let result = Array.zeroCreate n
         Array.blit source 0 result 0 n
         let! key = randomInit n (fun index ->
            state {
               let! u = ``(0, 1)``
               return u ** (1.0 / weight.[index])
            }
         )
         let index = ref n
         while !index < size do
            let thresholdIndex, threshold = Seq.zip (Seq.initInfinite id) key |> Seq.minBy snd
            let! u = ``(0, 1)``
            let x = log u / log threshold
            let weightSum = ref 0.0
            while !index < size && !weightSum < x do
               weightSum := !weightSum + weight.[!index]
               incr index
            if !weightSum >= x
            then
               let index = !index - 1
               let w = weight.[index]
               let! r = Statistics.uniform (threshold ** w, 1.0)
               key.[thresholdIndex] <- r ** (1.0 / w)
               result.[thresholdIndex] <- source.[index]
         return result
      }

let sampleWithReplacement n source =
   if n < 0
   then
      ArgumentOutOfRangeException ("n", "`n' must not be negative.") |> raise
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
   if n < 0
   then
      ArgumentOutOfRangeException ("n", "`n' must not be negative.") |> raise
   elif Array.length weight <> size
   then
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
