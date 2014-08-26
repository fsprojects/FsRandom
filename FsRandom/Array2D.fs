[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Array2D

open System

[<CompiledName("RandomCreate")>]
let randomCreate rowCount columnCount generator =
   if rowCount < 0 then
      outOfRange "rowCount" "`rowCount' must not be negative."
   elif columnCount < 0 then
      outOfRange "columnCount" "`columnCount' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let result = Array2D.zeroCreate rowCount columnCount
         let mutable s0 = s0
         for i = 0 to rowCount - 1 do
            for j = 0 to columnCount - 1 do
               let r, s' = Random.next generator s0
               result.[i, j] <- r
               s0 <- s'
         result, s0
      )

[<CompiledName("RandomCreateBased")>]
let randomCreateBased rowBase columnBase rowCount columnCount generator =
   if rowCount < 0 then
      outOfRange "rowCount" "`rowCount' must not be negative."
   elif columnCount < 0 then
      outOfRange "columnCount" "`columnCount' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let result = Array2D.zeroCreateBased rowBase columnBase rowCount columnCount
         let mutable s0 = s0
         for i = rowBase to rowBase + rowCount - 1 do
            for j = columnBase to columnBase + columnCount - 1 do
               let r, s' = Random.next generator s0
               result.[i, j] <- r
               s0 <- s'
         result, s0
      )

[<CompiledName("RandomInitialize")>]
let randomInit rowCount columnCount initializer =
   if rowCount < 0 then
      outOfRange "rowCount" "`rowCount' must not be negative."
   elif columnCount < 0 then
      outOfRange "columnCount" "`columnCount' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let result = Array2D.zeroCreate rowCount columnCount
         let mutable s0 = s0
         for i = 0 to rowCount - 1 do
            let init = initializer i
            for j = 0 to columnCount - 1 do
               let r, s' = Random.next (init j) s0
               result.[i, j] <- r
               s0 <- s'
         result, s0
      )

[<CompiledName("RandomInitializeBased")>]
let randomInitBased rowBase columnBase rowCount columnCount initializer =
   if rowCount < 0 then
      outOfRange "rowCount" "`rowCount' must not be negative."
   elif columnCount < 0 then
      outOfRange "columnCount" "`columnCount' must not be negative."
   else
      GeneratorFunction (fun s0 ->
         let result = Array2D.zeroCreateBased rowBase columnBase rowCount columnCount
         let mutable s0 = s0
         for i = rowBase to rowBase + rowCount - 1 do
            let init = initializer i
            for j = columnBase to columnBase + columnCount - 1 do
               let r, s' = Random.next (init j) s0
               result.[i, j] <- r
               s0 <- s'
         result, s0
      )
