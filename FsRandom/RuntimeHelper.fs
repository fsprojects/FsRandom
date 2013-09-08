[<AutoOpen>]
module internal FsRandom.RuntimeHelper

open System
open Microsoft.FSharp.Core.LanguagePrimitives

let inline curry f x y = f (x, y)
let inline uncurry f (x, y) = f x y

let inline outOfRange (paramName:string) (message:string) =
   ArgumentOutOfRangeException (paramName, message) |> raise

let inline isNaN (value : ^a when ^a : (static member IsNaN : ^a -> bool)) =
   (^a : (static member IsNaN : ^a -> bool) value)
   
let inline isInfinity (value : ^a when ^a : (static member IsInfinity : ^a -> bool)) =
   (^a : (static member IsInfinity : ^a -> bool) value)

let inline isInt x = x % GenericOne = GenericZero

let inline ensuresFiniteValue argument argumentName =
   if isNaN argument || isInfinity argument then
      invalidArg argumentName (sprintf "`%s' must be a finite number." argumentName) |> raise

let inline to64bit (lower:uint32) (upper:uint32) = (uint64 upper <<< 32) ||| uint64 lower

module List =
   let accumulate accumulation = function
      | [] -> invalidArg "list" "Empty list."
      | x :: xs -> List.scan accumulation x xs

module Array =
   let accumulate accumulation array =
      if Array.length array = 0 then
         Array.empty
      else
         let size = Array.length array
         let result = Array.zeroCreate size
         result.[0] <- array.[0]
         for index = 1 to size - 1 do
            result.[index] <- accumulation result.[index - 1] array.[index]
         result

type Tree<'a> =
   | Empty
   | Node of 'a * Tree<'a> * Tree<'a>

module BinarySearchTree =
   let empty<'a> = Tree<'a>.Empty
   let singleton key value = Tree.Node ((key, value), empty, empty)
   let rec insert key value = function
      | Node ((key', _) as y, left, right) when key < key' -> Node (y, insert key value left, right)
      | Node ((key', _) as y, left, right) -> Node (y, left, insert key value right)
      | Empty -> singleton key value
   let rec removeMinimum = function
      | Node (_, Empty, right) -> right
      | Node (x, left, right) -> Node (x, removeMinimum left, right)
      | Empty -> Empty
   let rec min = function
      | Node (x, Empty, _) -> x
      | Node (_, left, _) -> min left
      | Empty -> failwith "Empty."
   let rec toSeq = function
      | Node (x, left, right) ->
         seq {
            yield! toSeq left
            yield x
            yield! toSeq right
         }
      | Empty -> Seq.empty
