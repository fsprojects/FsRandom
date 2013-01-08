[<AutoOpen>]
module internal RecycleBin.Random.RuntimeHelper

open Microsoft.FSharp.Core.LanguagePrimitives

let inline isNaN (value : ^a when ^a : (static member IsNaN : ^a -> bool)) =
   (^a : (static member IsNaN : ^a -> bool) value)
   
let inline isInfinity (value : ^a when ^a : (static member IsInfinity : ^a -> bool)) =
   (^a : (static member IsInfinity : ^a -> bool) value)

let inline isInt (x : 'a) = x % (GenericOne : 'a) = (GenericZero : 'a)

let inline ensuresFiniteValue argument argumentName =
   if isNaN argument || isInfinity argument
   then
      invalidArg argumentName (sprintf "`%s' must be a finite number." argumentName) |> raise

module List =
   let accumulate accumulation = function
      | [] -> invalidArg "list" "Empty list."
      | x :: xs -> List.scan accumulation x xs

module Array =
   let accumulate accumulation array =
      if Array.length array = 0
      then
         Array.empty
      else
         let size = Array.length array
         let result = Array.zeroCreate size
         result.[0] <- array.[0]
         for index = 1 to size - 1 do
            result.[index] <- accumulation result.[index - 1] array.[index]
         result