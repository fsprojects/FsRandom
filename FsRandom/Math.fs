[<AutoOpen>]
module internal FsRandom.Math

[<Literal>]
let pi = 3.1415926535897932384626433832795
[<Literal>]
let ``2pi`` = 6.283185307179586476925286766559
[<Literal>]
let log2pi = 1.8378770664093454835606594728112

let inline polynomial (coefficient : float list) = fun x -> List.reduceBack (fun c acc -> c + x * acc) coefficient

// Coefficients for the loggamma function.
[<Literal>]
let a0 = 0.08333333333333333333333333333333
[<Literal>]
let a1 = -0.00277777777777777777777777777778
[<Literal>]
let a2 = 7.9365079365079365079365079365079e-4
[<Literal>]
let a3 = -5.952380952380952380952380952381e-4
[<Literal>]
let a4 = 8.4175084175084175084175084175084e-4
[<Literal>]
let a5 = -0.00191752691752691752691752691753
[<Literal>]
let a6 = 0.00641025641025641025641025641026
[<Literal>]
let a7 = -0.02955065359477124183006535947712
[<Literal>]
let N = 8.0

let loggamma x =
   let mutable v = 1.0
   let mutable x = x
   while x < N do
      v <- v * x
      x <- x + 1.0
   let s = polynomial [a0; a1; a2; a3; a4; a5; a6; a7] (1.0 / (x * x))
   s / x + 0.5 * log2pi - log v - x + (x - 0.5) * log x

let gamma x =
   if x < 0.0 then
      pi / (sin (pi * x) * exp (loggamma (1.0 - x)))
   else
      exp (loggamma x)
