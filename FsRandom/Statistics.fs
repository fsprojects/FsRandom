module FsRandom.Statistics

open System

let uniform (min, max) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   if min > max then
      outOfRange "min" "Invalid range."
   else
      if isInfinity (max - min) then
         let middle = (min + max) / 2.0
         let halfLength = middle - min
         fun s0 ->
            let u, s' = ``[0, 1]`` s0
            if u < 0.5 then
               min + 2.0 * u * halfLength, s'
            else
               middle + (2.0 * u - 1.0) * halfLength, s'
      else
         let length = max - min
         let transform u =  min + u * length
         Random.transformBy transform ``[0, 1]``

let loguniform (min, max) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   if min <= 0.0 || min > max then
      outOfRange "min" "Invalid range."
   else
      Random.transformBy exp (uniform (log min, log max))

let triangular (min, max, mode) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   ensuresFiniteValue mode "mode"
   if mode < min || max < mode then
      outOfRange "mode" "Invalid range."
   else
      let left u = min + sqrt ((u - min) * (mode - min))
      let right u = max - sqrt ((max - u) * (max - mode))
      let transform u = if u < mode then left u else right u
      Random.transformBy transform (uniform (min, max))

// Box-Muller's transformation.
let normal (mean, sd) =
   ensuresFiniteValue mean "mean"
   ensuresFiniteValue sd "sd"
   if sd <= 0.0 then
      outOfRange "sd" "`sd' must be positive."
   else
      let transform u1 u2 =
         let r = sqrt <| -2.0 * log u1
         let theta = ``2pi`` * u2
         let z = r * cos theta
         mean + z * sd
      Random.transformBy2 transform ``(0, 1)`` ``(0, 1)``

let lognormal p = Random.transformBy exp (normal p)
      
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
      if v <= 1.0 then
         let x = c1 * v ** c3
         if u2 <= (2.0 - x) / (2.0 + x) || u2 <= exp (-x) then
            result <- x
            incomplete := false
      else
         let x = -log (c1 * c3 * (c2 - v));
         let y = x / c1;
         if u2 * (alpha + y - alpha * y) <= 1.0 || u2 <= y ** (alpha - 1.0) then
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
      if t > 0.0 then
         let v = pown t 3;
         let u, s' = ``(0, 1)`` state
         state <- s'
         if u < 1.0 - 0.0331 * pown z 4 || log u < 0.5 * z * z + c1 * (1.0 - v + log v) then
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
   if shape <= 0.0 then
      outOfRange "shape" "`scale' must be positive."
   elif scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      let get = Random.transformBy ((*) scale)
      if isInt shape then
         get (gammaInt (int shape))
      elif shape < 1.0 then
         get (gammaSmall shape)
      else
         get (gammaLarge shape)

let beta (alpha, beta) =
   ensuresFiniteValue alpha "alpha"
   ensuresFiniteValue beta "beta"
   if alpha <= 0.0 then
      outOfRange "alpha" "`alpha' must be positive."
   elif beta <= 0.0 then
      outOfRange "beta" "`beta' must be positive."
   else
      let transform y1 y2 = y1 / (y1 + y2)
      Random.transformBy2 transform (gamma (alpha, 1.0)) (gamma (beta, 1.0))

let exponential rate =
   ensuresFiniteValue rate "rate"
   if rate <= 0.0 then
      outOfRange "rate" "`rate' must be positive."
   else
      let transform u = -log u / rate
      Random.transformBy transform ``(0, 1)``

let weibull (shape, scale) =
   ensuresFiniteValue shape "shape"
   ensuresFiniteValue scale "scale"
   if shape <= 0.0 then
      outOfRange "shape" "`shape' must be positive."
   elif scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      let transform u = let r = (-log u) ** (1.0 / shape) in r * scale
      Random.transformBy transform ``(0, 1)``

let gumbel (location, scale) =
   ensuresFiniteValue location "location"
   ensuresFiniteValue scale "scale"
   if scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      let transform u = location - scale * log (-log u)
      Random.transformBy transform ``(0, 1)``

let cauchy (location, scale) =
   ensuresFiniteValue location "location"
   ensuresFiniteValue scale "scale"
   if scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      let transform u = location + scale * tan (pi * (u - 0.5))
      Random.transformBy transform ``(0, 1)``

let chisquare df =
   if df <= 0 then
      outOfRange "degreeOfFreedom" "`degreeOfFreedom' must be positive."
   else
      if df = 1 then
         let transform u y = 2.0 * y * u * u
         Random.transformBy2 transform ``[0, 1)`` (gamma (1.5, 2.0))
      else
         gamma (float df / 2.0, 2.0)

let t df =
   if df <= 0 then
      outOfRange "degreeOfFreedom" "`degreeOfFreedom' must be positive."
   else
      if df = 1 then
         cauchy (0.0, 1.0)
      elif df = 2 then
         let transform z w = z / sqrt w
         Random.transformBy2 transform (normal (0.0, 1.0)) (exponential 1.0)
      else
         let r = float df / 2.0
         let d = sqrt r
         let transform z w = d * z / sqrt w
         Random.transformBy2 transform (normal (0.0, 1.0)) (gamma (r, 1.0))

let uniformDiscrete (min, max) =
   if min > max then
      outOfRange "min" "Invalid range."
   else
      let range = float <| int64 max - int64 min + 1L
      let transform u = min + int (u * range)
      Random.transformBy transform ``[0, 1)``

let poisson lambda =
   ensuresFiniteValue lambda "lambda"
   if lambda <= 0.0 then
      outOfRange "lambda" "`lambda' must be positive."
   else
      let c = 1.0 / lambda
      let m = int lambda
      let m' = float m
      let d = exp <| -lambda + m' * log lambda - loggamma (m' + 1.0)
      fun s0 ->
         let xu = ref m
         let xl = ref m
         let mutable pu = d
         let mutable pl = d
         let mutable u, s' = ``[0, 1)`` s0
         let mutable v = u - pu
         let mutable result = if v <= 0.0 then Some (!xu) else None
         while result.IsNone do
            u <- v
            if !xl > 0 then
               pl <- pl * c * float !xl
               decr xl
               v <- u - pl
               if v > 0.0 then
                  u <- v
               else
                  result <- Some (!xl)
            if result.IsNone then
               incr xu
               pu <- pu * lambda / float !xu
               v <- u - pu
               result <- if v <= 0.0 then Some (!xu) else None
         result.Value, s'

let geometric probability =
   ensuresFiniteValue probability "probability"
   if probability <= 0.0 || 1.0 < probability then
      outOfRange "probability" "`probability' must be in the range of (0, 1]."
   else
      let z = log (1.0 - probability)
      let transform u = int <| ceil (-u / z)
      Random.transformBy transform ``(0, 1)``

let bernoulli probability =
   ensuresFiniteValue probability "probability"
   if probability <= 0.0 || 1.0 <= probability then
      outOfRange "probability" "`probability' must be in the range of (0, 1)."
   else
      let transform u = if u <= probability then 1 else 0
      Random.transformBy transform ``[0, 1]``

let binomial (n, probability) =
   ensuresFiniteValue probability "probability"
   if n <= 0 then
      outOfRange "n" "`n' must be positive."
   elif probability <= 0.0 || 1.0 <= probability then
      outOfRange "probability" "`probability' must be in the range of (0, 1)."
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
   if k < 2 then
      invalidArg "alpha" "`alpha' must contain two or more values."
   elif List.exists (fun x -> isNaN x || isInfinity x || x <= 0.0) alpha then
      outOfRange "alpha" "All elements in `alpha' must be positive."
   else
      fun s0 ->
         let y, sum, s' = List.foldBack (fun a (xs, sum, s) -> let x, s' = gamma (a, 1.0) s in x :: xs, sum + x, s') alpha ([], 0.0, s0)
         List.map (fun y' -> y' / sum) y, s'

let multinomial (n, weight) =
   if n <= 0 then
      outOfRange "n" "`n' must be positive."
   let k = List.length weight
   if k < 2 then
      invalidArg "weight" "`weight' must contain two or more values."
   elif List.exists (fun x -> isNaN x || isInfinity x || x <= 0.0) weight then
      outOfRange "probability" "All elements in `weight' must be positive."
   else
      let cdf, sum = List.fold (fun (xs, sum) x -> let s = sum + x in xs @ [s], s) ([], 0.0) weight
      fun s0 ->
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
   let markovChain (generator:_ -> GeneratorFunction<_, _>) =
      let f = uncurry (generator >> Random.next)
      let rec loop current = seq {
         let next = f current
         yield fst next
         yield! loop next
      }
      curry loop