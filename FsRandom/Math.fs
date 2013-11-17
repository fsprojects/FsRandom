[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module internal FsRandom.Math

open Microsoft.FSharp.Core.LanguagePrimitives

[<Literal>]
let pi = 3.1415926535897932384626433832795
[<Literal>]
let ``2pi`` = 6.283185307179586476925286766559
[<Literal>]
let log2pi = 1.8378770664093454835606594728112
/// <summary>The minimum number greater than 1 (= 2^(-52)).</summary>
[<Literal>]
let epsilon = 2.22044604925031308084726333618e-16

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

let cdf (p:float list) =
   let sum = List.sum p
   List.accumulate (+) p |> List.map (fun w -> w / sum)

let sqrtsumsq x y =
   if abs x > abs y then
      let r = y / x
      in abs x * sqrt (1.0 + r * r)
   elif y <> 0.0 then
      let r = x / y
      in abs y * sqrt (1.0 + r * r)
   else
      0.0
      
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector =
   let inline normalize vector =
      let d = Array.fold sqrtsumsq 0.0 vector
      Array.map (fun x -> x / d) vector
   let inline add a b =
      let n = Array.length a
      Array.init n (fun i -> Array.get a i + Array.get b i)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Matrix =
   let transpose matrix =
      Array2D.init (Array2D.length2 matrix) (Array2D.length1 matrix) (fun i j -> matrix.[j, i])

   let inline diag n = Array2D.init n n (fun i j -> if i = j then GenericOne else GenericZero)
   let inline diagByVector vector =
      let n = Array.length vector
      Array2D.init n n (fun i j -> if i = j then vector.[i] else GenericZero)

   let private diagSize matrix = min (Array2D.length1 matrix) (Array2D.length2 matrix)
   let getDiag matrix = Array.init (diagSize matrix) (fun i -> matrix.[i, i])
   let forallDiag f matrix = seq { 0 .. diagSize matrix - 1 } |> Seq.forall (fun i -> f matrix.[i, i])
   let existsDiag f matrix = seq { 0 .. diagSize matrix - 1 } |> Seq.exists (fun i -> f matrix.[i, i])

   let inline add a b =
      let m = Array2D.length1 a
      let n = Array2D.length2 b
      Array2D.init m n (fun i j -> a.[i, j] + b.[i, j])

   let inline multiply a b =
      let m = Array2D.length1 a
      let k = Array2D.length2 a
      let n = Array2D.length2 b
      let p = Array2D.zeroCreate m n
      for i = 0 to m - 1 do
         for j = 0 to n - 1 do
            let mutable sum = 0.0
            for t = 0 to k - 1 do
               sum <- sum + a.[i, t] * b.[t, j]
            p.[i, j] <- sum
      p

   let inline multiplyVector a x =
      let m = Array2D.length1 a
      let n = Array.length x
      let p = Array.zeroCreate m
      for i = 0 to m - 1 do
         let mutable sum = 0.0
         for t = 0 to n - 1 do
            sum <- sum +  a.[i, t] * x.[t]
         p.[i] <- sum
      p

   let inline isSymmetric matrix =
      let m = Array2D.length1 matrix
      let n = Array2D.length2 matrix
      m = n && Seq.forall (fun i -> Seq.forall (fun j -> matrix.[i, j] = matrix.[j, i]) <| seq { 0 .. i - 1 }) <| seq { 0 .. n - 1 }

   /// <summary>Computes eigenvalues and eigenvectors of symmetric matrix.</summary>
   /// <seealso cref="isSymmetric" />
   let jacobi matrix =
      let n = Array2D.length1 matrix
      let m = n - 1
      let eigenvalues = Array2D.copy matrix
      let eigenvectors = diag n
      let findMax () =
         seq {
            for i = 0 to m do
               for j = 0 to m do
                  if i <> j then
                     yield (i, j), abs eigenvalues.[i, j]
         }
         |> Seq.maxBy snd
      let loop = ref true
      while !loop do
         let (p, q), max = findMax ()
         if max < epsilon then
            loop := false
         else
            let app = eigenvalues.[p, p]
            let aqq = eigenvalues.[q, q]
            let apq = eigenvalues.[p, q]
            let t = 0.5 * (app - aqq)
            let ss = 0.5 * (1.0 - abs t / sqrtsumsq apq t)  // sin^2
            let cc = 1.0 - ss  // cos^2
            let s = if apq * t > 0.0 then -sqrt ss else sqrt ss  // sin
            let c = sqrt cc  // cos
            let sc = s * c  // sin * cos
            for i = 0 to m do
               let api = eigenvalues.[p, i]
               let aqi = eigenvalues.[q, i]
               eigenvalues.[p, i] <- api * c - aqi * s
               eigenvalues.[q, i] <- api * s + aqi * c
            for i = 0 to m do
               eigenvalues.[i, p] <- eigenvalues.[p, i]
               eigenvalues.[i, q] <- eigenvalues.[q, i]
            eigenvalues.[p, p] <- app * cc - 2.0 * apq * sc + aqq * ss
            eigenvalues.[q, q] <- aqq * cc + 2.0 * apq * sc + app * ss
            eigenvalues.[p, q] <- 0.0
            eigenvalues.[q, p] <- 0.0
            for i = 0 to m do
               let aip = eigenvectors.[i, p]
               let aiq = eigenvectors.[i, q]
               eigenvectors.[i, p] <- aip * c - aiq * s
               eigenvectors.[i, q] <- aip * s + aiq * c
      getDiag eigenvalues, eigenvectors
