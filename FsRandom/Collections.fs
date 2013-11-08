[<AutoOpen>]
module internal FsRandom.Collections

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module List =
   let accumulate accumulation = function
      | [] -> invalidArg "list" "Empty list."
      | x :: xs -> List.scan accumulation x xs

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module BinarySearchTree =
   let empty = Tree.Empty
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
   let toList tree =
      let rec loop acc = function
         | Node (x, left, right) -> loop (x :: loop acc right) left
         | Empty -> acc
      loop [] tree
