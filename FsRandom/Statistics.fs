module FsRandom.Statistics

open System
open FsRandom.StateMonad

[<Literal>]
let pi = 3.1415926535897932384626433832795
[<Literal>]
let ``2pi`` = 6.283185307179586476925286766559

let uniform (min, max) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   if min > max
   then
      ArgumentOutOfRangeException ("min", "Invalid range.") |> raise
   else
      if isInfinity (max - min)
      then
         fun s0 ->
            let middle = (min + max) / 2.0
            let halfLength = middle - min
            let u, s' = ``[0, 1]`` s0
            if u < 0.5
            then
               min + 2.0 * u * halfLength, s'
            else
               middle + (2.0 * u - 1.0) * halfLength, s'
      else
         fun s0 ->
            let length = max - min
            let u, s' = ``[0, 1]`` s0
            min + u * length, s'

let loguniform (min, max) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   if min <= 0.0 || min > max
   then
      ArgumentOutOfRangeException ("min", "Invalid range.") |> raise
   else
      fun s0 ->
         let u, s' = uniform (log min, log max) s0
         exp u, s'

let triangular (min, max, mode) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   ensuresFiniteValue mode "mode"
   if mode < min || max < mode
   then
      ArgumentOutOfRangeException ("mode", "Invalid range.") |> raise
   else
      fun s0 ->
         let u, s' = uniform (min, max) s0
         if u < mode
         then
            min + sqrt ((u - min) * (mode - min)), s'
         else
            max - sqrt ((max - u) * (max - mode)), s'

// Box-Muller's transformation.
let normal (mean, sd) =
   ensuresFiniteValue mean "mean"
   ensuresFiniteValue sd "sd"
   if sd <= 0.0
   then
      ArgumentOutOfRangeException ("sd", "`sd' must be positive.") |> raise
   else
      fun s0 ->
         let u1, s1 = ``(0, 1)`` s0
         let u2, s' = ``(0, 1)`` s1
         let r = sqrt <| -2.0 * log u1
         let theta = ``2pi`` * u2
         let z = r * cos theta
         mean + z * sd, s'

let lognormal (mu, sigma) =
   fun s0 ->
      let z, s' = normal (mu, sigma) s0
      exp z, s'
      
// random number distributed gamma for alpha < 1 (Best 1983).
let gammaSmall alpha s0 =
   let c1 = 0.07 + sqrt (1.0 - alpha)
   let c2 = 1.0 + alpha * exp (-c1) / c1
   let c3 = 1.0 / alpha
   let mutable state = s0
   let mutable result = 0.0
   let incomplete = ref true
   while !incomplete do
      let u1, s1 = ``(0, 1)`` state
      let u2, s' = ``(0, 1)`` s1
      state <- s'
      let v = c2 * u1
      if v <= 1.0
      then
         let x = c1 * v ** c3
         if u2 <= (2.0 - x) / (2.0 + x) || u2 <= exp (-x)
         then
            result <- x
            incomplete := false
      else
         let x = -log (c1 * c3 * (c2 - v));
         let y = x / c1;
         if u2 * (alpha + y - alpha * y) <= 1.0 || u2 <= y ** (alpha - 1.0)
         then
            result <- x
            incomplete := false
   result, state

// random number distributed gamma for alpha < 1 (Marsaglia & Tsang 2001).
let gammaLarge alpha s0 =
   let c1 = alpha - 1.0 / 3.0
   let c2 = 1.0 / sqrt (9.0 * c1)
   let mutable state = s0
   let mutable result = 0.0
   let incomplete = ref true
   while !incomplete do
      let z, s' = normal (0.0, 1.0) state
      state <- s'
      let t = 1.0 + c2 * z;
      if t > 0.0
      then
         let v = pown t 3;
         let u, s' = ``(0, 1)`` state
         state <- s'
         if u < 1.0 - 0.0331 * pown z 4 || log u < 0.5 * z * z + c1 * (1.0 - v + log v)
         then
            result <- c1 * v
            incomplete := false
   result, state
// random number distributed gamma for alpha is integer.
let gammaInt alpha s0 =
   let rec loop n ((sum, s) as result) =
      match n with
         | 0 -> result
         | _ ->
            let u, s' = ``(0, 1)`` s
            loop (n - 1) (sum - log u, s')
   loop alpha (0.0, s0)

let gamma (shape, scale) =
   ensuresFiniteValue shape "shape"
   ensuresFiniteValue scale "scale"
   if shape <= 0.0
   then
      ArgumentOutOfRangeException ("shape", "`scale' must be positive.") |> raise
   elif scale <= 0.0
   then
      ArgumentOutOfRangeException ("scale", "`scale' must be positive.") |> raise
   else
      if isInt shape
      then
         fun s0 ->
            let r, s' = gammaInt (int shape) s0
            in scale * r, s'
      elif shape < 1.0
      then
         fun s0 ->
            let r, s' = gammaSmall shape s0
            in scale * r, s'
      else
         fun s0 ->
            let r, s' = gammaLarge shape s0
            in scale * r, s'

let beta (alpha, beta) =
   ensuresFiniteValue alpha "alpha"
   ensuresFiniteValue beta "beta"
   if alpha <= 0.0
   then
      ArgumentOutOfRangeException ("alpha", "`alpha' must be positive.") |> raise
   elif beta <= 0.0
   then
      ArgumentOutOfRangeException ("beta", "`beta' must be positive.") |> raise
   else
      fun s0 ->
         let y1, s1 = gamma (alpha, 1.0) s0
         let y2, s' = gamma (beta, 1.0) s1
         y1 / (y1 + y2), s'

let exponential rate =
   ensuresFiniteValue rate "rate"
   if rate <= 0.0
   then
      ArgumentOutOfRangeException ("rate", "`rate' must be positive.") |> raise
   else
      fun s0 ->
         let u, s' = ``(0, 1)`` s0
         in -log u / rate, s'

let weibull (shape, scale) =
   ensuresFiniteValue shape "shape"
   ensuresFiniteValue scale "scale"
   if shape <= 0.0 then
      ArgumentOutOfRangeException ("shape", "`shape' must be positive.") |> raise
   elif scale <= 0.0 then
      ArgumentOutOfRangeException ("scale", "`scale' must be positive.") |> raise
   else
      random {
         let! u = ``(0, 1)``
         let r = (-log u) ** (1.0 / shape)
         return r * scale
      }

let cauchy (location, scale) =
   ensuresFiniteValue location "location"
   ensuresFiniteValue scale "scale"
   if scale <= 0.0
   then
      ArgumentOutOfRangeException ("scale", "`scale' must be positive.") |> raise
   else
      fun s0 ->
         let u, s' = ``(0, 1)`` s0
         location + scale * tan (pi * (u - 0.5)), s'

let chisquare df =
   if df <= 0
   then
      ArgumentOutOfRangeException ("degreeOfFreedom", "`degreeOfFreedom' must be positive.") |> raise
   else
      if df = 1
      then
         fun s0 ->
            let u, s1 = ``[0, 1)`` s0
            let y, s' = gamma (1.5, 2.0) s1
            in 2.0 * y * u * u, s'
      else
         gamma (float df / 2.0, 2.0)

let t df =
   if df <= 0
   then
      ArgumentOutOfRangeException ("degreeOfFreedom", "`degreeOfFreedom' must be positive.") |> raise
   else
      if df = 1
      then
         cauchy (0.0, 1.0)
      elif df = 2
      then
         fun s0 ->
            let z, s1 = normal (0.0, 1.0) s0
            let w, s' = exponential 1.0 s1
            z / sqrt w, s'
      else
         fun s0 ->
            let r = float df / 2.0
            let d = sqrt r
            let z, s1 = normal (0.0, 1.0) s0
            let w, s' = gamma (r, 1.0) s1
            d * z / sqrt w, s'

let uniformDiscrete (min, max) =
   if min > max
   then
      ArgumentOutOfRangeException ("min", "Invalid range.") |> raise
   else
      fun s0 ->
         let u, s' = ``[0, 1)`` s0
         let range = int64 max - int64 min + 1L
         min + int (u * float range), s'

let poisson lambda =
   ensuresFiniteValue lambda "lambda"
   if lambda <= 0.0
   then
      ArgumentOutOfRangeException ("lambda", "`lambda' must be positive.") |> raise
   else
      let expLambda = exp lambda
      if isInfinity expLambda
      then
         fun s0 ->
            let count = ref (-1)
            let mutable state = s0
            let mutable t = lambda
            while t > 0.0 do
               incr count
               let u, s' = ``(0, 1)`` state
               state <- s'
               t <- t + log u
            !count, state
      else
         fun s0 ->
            let count = ref (-1)
            let mutable state = s0
            let mutable t = expLambda
            while t > 1.0 do
               incr count
               let u, s' = ``(0, 1)`` state
               state <- s'
               t <- t * u
            !count, state

let geometric probability =
   ensuresFiniteValue probability "probability"
   if probability <= 0.0 || 1.0 < probability
   then
      ArgumentOutOfRangeException ("probability", "`probability' must be in the range of (0, 1].") |> raise
   else
      fun s0 ->
         let u, s' = ``(0, 1)`` s0
         in -u / log (1.0 - probability) |> ceil |> int, s'

let bernoulli probability =
   ensuresFiniteValue probability "probability"
   if probability <= 0.0 || 1.0 <= probability
   then
      ArgumentOutOfRangeException ("probability", "`probability' must be in the range of (0, 1).") |> raise
   else
      fun s0 ->
         let u, s' = ``[0, 1]`` s0
         (if u <= probability then 1 else 0), s'

let binomial (n, probability) =
   ensuresFiniteValue probability "probability"
   if n <= 0
   then
      ArgumentOutOfRangeException ("n", "`n' must be positive.") |> raise
   elif probability <= 0.0 || 1.0 <= probability
   then
      ArgumentOutOfRangeException ("probability", "`probability' must be in the range of (0, 1).") |> raise
   else
      fun s0 ->
         let count = ref 0
         let mutable s = s0
         for i = 1 to n do
            let u, s' = ``[0, 1]`` s
            if u <= probability then incr count
            s <- s'
         !count, s

let dirichlet alpha =
   let k = List.length alpha
   if k < 2
   then
      invalidArg "alpha" "`alpha' must contain two or more values."
   elif List.exists (fun x -> isNaN x || isInfinity x || x <= 0.0) alpha
   then
      ArgumentOutOfRangeException ("alpha", "All elements in `alpha' must be positive.") |> raise
   else
      fun s0 ->
         let y, sum, s' = List.foldBack (fun a (xs, sum, s) -> let x, s' = gamma (a, 1.0) s in x :: xs, sum + x, s') alpha ([], 0.0, s0)
         List.map (fun y' -> y' / sum) y, s'

let multinomial (n, weight) =
   if n <= 0
   then
      ArgumentOutOfRangeException ("n", "`n' must be positive.") |> raise
   let k = List.length weight
   if k < 2
   then
      invalidArg "weight" "`weight' must contain two or more values."
   elif List.exists (fun x -> isNaN x || isInfinity x || x <= 0.0) weight
   then
      ArgumentOutOfRangeException ("probability", "All elements in `weight' must be positive.") |> raise
   else
      fun s0 ->
         let cdf, sum = List.fold (fun (xs, sum) x -> let s = sum + x in xs @ [s], s) ([], 0.0) weight
         let result = Array.zeroCreate k
         let mutable s = s0
         for loop = 1 to n do
            let u, s' = ``[0, 1)`` s
            let p = sum * u
            let index = List.findIndex (fun x -> p < x) cdf
            result.[index] <- result.[index] + 1
            s <- s'
         Array.toList result, s
         
module Seq =
   let markovChain (generator:'a -> State<PrngState<'s>, 'a>) (builder:RandomBuilder<'s>) =
      let rec loop seed previous = seq {
         let r, next = builder { return! generator previous } <| seed
         yield r
         yield! loop next r
      }
      loop