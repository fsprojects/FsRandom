[<AutoOpen>]
module internal FsRandom.RuntimeHelper

open System
open Microsoft.FSharp.Core.LanguagePrimitives

let inline curry f x y = f (x, y)
let inline uncurry f (x, y) = f x y
let inline flip f x y = f y x
let inline cons x xs = x :: xs
let inline tuple x y = x, y
let inline tuple3 x y z = x, y, z

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
