[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.Statistics

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Standard =
   let inline minusLog x = -log x

   [<CompiledName("Uniform")>]
   let uniform = ``[0, 1]``

   // Ziggurat algorithm for normal distribution.
   [<Literal>]
   let k0 = 8
   [<Literal>]
   let m = 64
   [<Literal>]
   let n = 256  // 2^8
   [<Literal>]
   let r = 3.6541528853610088
   [<Literal>]
   let v = 0.00492867323399
   let inline f x = exp (-x * x / 2.0)
   let kn, fn, wn =
      let d = pown 2.0 (m - k0 - 1)
      let xn = Array.zeroCreate (n + 1)
      xn.[n] <- v / f r
      xn.[n - 1] <- r
      for i = n - 2 downto 1 do
         xn.[i] <- sqrt <| -2.0 * log (f xn.[i + 1] + v / xn.[i + 1])
      let wn = Array.zeroCreate n
      let kn = Array.zeroCreate n
      let fn = Array.zeroCreate n
      for i = n - 1 downto 1 do
         wn.[i - 1] <- xn.[i] / d
         kn.[i] <- uint64 (xn.[i] / wn.[i])
         fn.[i] <- f xn.[i]
   //   kn.[0] <- 0uL
      fn.[0] <- 1.0
      kn, fn, wn
   [<CompiledName("Normal")>]
   let normal = GeneratorFunction (fun s0 ->
      let s = ref s0
      let mutable result = None
      while result.IsNone do
         let u, s' = Random.next rawBits !s
         s := s'
         let i = int (u &&& 0b11111111uL)
         let u = u >>> k0
         let sign = if u &&& 0x1uL = 0uL then 1.0 else -1.0
         let u = u >>> 1
         if u < kn.[i] then
            let ux = float u * wn.[i]
            result <- Some (sign * ux)
         elif i = n - 1 then
            while result.IsNone do
               let u1, s' = Random.next ``[0, 1)`` !s
               let u2, s' = Random.next ``[0, 1)`` s'
               s := s'
               let y = -log (1.0 - u1) / r
               let z = -log (u2)
               if y * y <= z + z then
                  result <- Some (sign * (r + y))
         else
            let ux = float u * wn.[i]
            let fx = f ux
            let u, s' = Random.next ``[0, 1)`` !s
            s := s'
            if u * (fn.[i] - fn.[i + 1]) <= fx - fn.[i + 1] then
               result <- Some (ux * sign)
      result.Value, !s
   )

   // random number distributed gamma for alpha < 1 (Best 1983).
   let gammaSmall alpha s0 =
      let c1 = 0.07 + sqrt (1.0 - alpha)
      let c2 = 1.0 + alpha * exp (-c1) / c1
      let c3 = 1.0 / alpha
      let mutable state = s0
      let mutable result = 0.0
      let incomplete = ref true
      while !incomplete do
         let u1, s1 = Random.next ``(0, 1)`` state
         let u2, s' = Random.next ``(0, 1)`` s1
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
         let z, s' = Random.next normal state
         state <- s'
         let t = 1.0 + c2 * z;
         if t > 0.0 then
            let v = pown t 3;
            let u, s' = Random.next ``(0, 1)`` state
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
               let u, s' = Random.next ``(0, 1)`` s
               loop (n - 1) (sum - log u, s')
      loop alpha (0.0, s0)

   [<CompiledName("Gamma")>]
   let gamma shape =
      ensuresFiniteValue shape "shape"
      if shape <= 0.0 then
         outOfRange "shape" "`scale' must be positive."
      else
         if isInt shape then
            GeneratorFunction (gammaInt (int shape))
         elif shape < 1.0 then
            GeneratorFunction (gammaSmall shape)
         else
            GeneratorFunction (gammaLarge shape)

   [<CompiledName("Exponential")>]
   let exponential =
      Random.map minusLog ``(0, 1)``

   [<CompiledName("Weibull")>]
   let weibull shape =
      ensuresFiniteValue shape "shape"
      if shape <= 0.0 then
         outOfRange "shape" "`shape' must be positive."
      else
         let transform u = (-log u) ** (1.0 / shape)
         Random.map transform ``(0, 1)``

   [<CompiledName("Gumbel")>]
   let gumbel =
      Random.map (minusLog >> minusLog) ``(0, 1)``

   [<CompiledName("Cauchy")>]
   let cauchy =
      let transform u = tan (pi * (u - 0.5))
      Random.map transform ``(0, 1)``

[<CompiledName("Uniform")>]
let uniform (min, max) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   if min > max then
      outOfRange "min" "Invalid range."
   else
      if isInfinity (max - min) then
         let middle = (min + max) / 2.0
         let halfLength = middle - min
         GeneratorFunction (fun s0 ->
            let u, s' = Random.next ``[0, 1]`` s0
            if u < 0.5 then
               min + 2.0 * u * halfLength, s'
            else
               middle + (2.0 * u - 1.0) * halfLength, s'
         )
      else
         let length = max - min
         let transform u =  min + u * length
         Random.map transform ``[0, 1]``

[<CompiledName("LogUniform")>]
let loguniform (min, max) =
   ensuresFiniteValue min "min"
   ensuresFiniteValue max "max"
   if min <= 0.0 || min > max then
      outOfRange "min" "Invalid range."
   else
      Random.map exp (uniform (log min, log max))

[<CompiledName("Triangular")>]
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
      Random.map transform (uniform (min, max))

[<CompiledName("Normal")>]
let normal (mean, sd) =
   ensuresFiniteValue mean "mean"
   ensuresFiniteValue sd "sd"
   if sd <= 0.0 then
      outOfRange "sd" "`sd' must be positive."
   else
      let transform z = mean + z * sd
      Random.map transform Standard.normal

[<CompiledName("LogNormal")>]
let lognormal p = Random.map exp (normal p)
      
[<CompiledName("Gamma")>]
let gamma (shape, scale) =
   ensuresFiniteValue shape "shape"
   ensuresFiniteValue scale "scale"
   if shape <= 0.0 then
      outOfRange "shape" "`scale' must be positive."
   elif scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      Random.map ((*) scale) (Standard.gamma (shape))

[<CompiledName("Beta")>]
let beta (alpha, beta) =
   ensuresFiniteValue alpha "alpha"
   ensuresFiniteValue beta "beta"
   if alpha <= 0.0 then
      outOfRange "alpha" "`alpha' must be positive."
   elif beta <= 0.0 then
      outOfRange "beta" "`beta' must be positive."
   else
      let transform y1 y2 = y1 / (y1 + y2)
      Random.map2 transform (Standard.gamma (alpha)) (Standard.gamma (beta))

[<CompiledName("Exponential")>]
let exponential rate =
   ensuresFiniteValue rate "rate"
   if rate <= 0.0 then
      outOfRange "rate" "`rate' must be positive."
   else
      let transform t = t / rate
      Random.map transform Standard.exponential

[<CompiledName("Weibull")>]
let weibull (shape, scale) =
   ensuresFiniteValue scale "scale"
   if scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      Random.map ((*) scale) (Standard.weibull (shape))

[<CompiledName("Gumbel")>]
let gumbel (location, scale) =
   ensuresFiniteValue location "location"
   ensuresFiniteValue scale "scale"
   if scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      let transform g = location + scale * g
      Random.map transform Standard.gumbel

[<CompiledName("Cauchy")>]
let cauchy (location, scale) =
   ensuresFiniteValue location "location"
   ensuresFiniteValue scale "scale"
   if scale <= 0.0 then
      outOfRange "scale" "`scale' must be positive."
   else
      let transform c = location + c * scale
      Random.map transform Standard.cauchy

[<CompiledName("ChiSquare")>]
let chisquare df =
   if df <= 0 then
      outOfRange "degreeOfFreedom" "`degreeOfFreedom' must be positive."
   else
      if df = 1 then
         let transform u y = 2.0 * y * u * u
         Random.map2 transform ``[0, 1)`` (gamma (1.5, 2.0))
      else
         gamma (float df / 2.0, 2.0)

[<CompiledName("StudentT")>]
let studentT df =
   if df <= 0 then
      outOfRange "degreeOfFreedom" "`degreeOfFreedom' must be positive."
   else
      if df = 1 then
         Standard.cauchy
      elif df = 2 then
         let transform z w = z / sqrt w
         Random.map2 transform Standard.normal Standard.exponential
      else
         let r = float df / 2.0
         let d = sqrt r
         let transform z w = d * z / sqrt w
         Random.map2 transform Standard.normal (Standard.gamma (r))
[<CompiledName("T")>]
let t df = studentT df

[<CompiledName("VonMises")>]
let vonMises (mu, kappa) =
   if mu < -pi || pi <= mu then
      outOfRange "direction" "`direction' must be in [-pi, pi)."
   elif kappa <= 0.0 then
      outOfRange "concentration" "`concentration' must be positive."
   else
      let p = let k = 2.0 * kappa in (1.0 + sqrt (1.0 + k * k)) / k
      GeneratorFunction (fun s0 ->
         let mutable s = s0
         let mutable r = None
         while r.IsNone do
            let u, s1 = Random.next ``[0, 1)`` s
            let v, s' = Random.next ``[0, 1]`` s1
            s <- s'
            let z = cos (``2pi`` * u)
            let w = (1.0 - p * z) / (p - z)
            let t = kappa * (p - w)
            if v <= t * (2.0 - t) || v <= t * exp (1.0 - t) then
               let y = if u < 0.5 then -acos w else acos w
               let x = y + mu
               r <- Some (if x >= pi then x - ``2pi`` elif x < -pi then x + ``2pi`` else x)
         r.Value, s
      )

[<CompiledName("UniformDiscrete")>]
let uniformDiscrete (min, max) =
   if min > max then
      outOfRange "min" "Invalid range."
   else
      let range = float <| int64 max - int64 min + 1L
      let transform u = min + int (u * range)
      Random.map transform ``[0, 1)``

[<CompiledName("Poisson")>]
let poisson lambda =
   ensuresFiniteValue lambda "lambda"
   if lambda <= 0.0 then
      outOfRange "lambda" "`lambda' must be positive."
   else
      let c = 1.0 / lambda
      let m = int lambda
      let m' = float m
      let d = exp <| -lambda + m' * log lambda - loggamma (m' + 1.0)
      GeneratorFunction (fun s0 ->
         let xu = ref m
         let xl = ref m
         let mutable pu = d
         let mutable pl = d
         let mutable u, s' = Random.next ``[0, 1)`` s0
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
      )

[<CompiledName("GeometricZeroBased")>]
let geometric0 probability =
   ensuresFiniteValue probability "probability"
   if probability <= 0.0 || 1.0 < probability then
      outOfRange "probability" "`probability' must be in the range of (0, 1]."
   else
      let q = -(1.0 - probability) / probability
      GeneratorFunction (fun s0 ->
         let u, s' = Random.next ``(0, 1)`` s0
         Random.next (poisson (log u * q)) s'
      )
[<CompiledName("GeometricOneBased")>]
let geometric1 probability = Random.map ((+) 1) (geometric0 probability)

[<CompiledName("Bernoulli")>]
let bernoulli probability =
   ensuresFiniteValue probability "probability"
   if probability <= 0.0 || 1.0 <= probability then
      outOfRange "probability" "`probability' must be in the range of (0, 1)."
   else
      let transform u = if u <= probability then 1 else 0
      Random.map transform ``[0, 1]``

[<CompiledName("Binomial")>]
let binomial (n, probability) =
   ensuresFiniteValue probability "probability"
   if n <= 0 then
      outOfRange "n" "`n' must be positive."
   elif probability <= 0.0 || 1.0 <= probability then
      outOfRange "probability" "`probability' must be in the range of (0, 1)."
   else
      GeneratorFunction (fun s0 ->
         let count = ref 0
         let mutable s = s0
         for i = 1 to n do
            let u, s' = Random.next ``[0, 1]`` s
            if u <= probability then incr count
            s <- s'
         !count, s
      )

[<CompiledName("Dirichlet")>]
let dirichlet alpha =
   let k = List.length alpha
   if k < 2 then
      invalidArg "alpha" "`alpha' must contain two or more values."
   elif List.exists (fun x -> isNaN x || isInfinity x || x <= 0.0) alpha then
      outOfRange "alpha" "All elements in `alpha' must be positive."
   else
      GeneratorFunction (fun s0 ->
         let y, sum, s' = List.foldBack (fun a (xs, sum, s) -> let x, s' = Random.next (Standard.gamma (a)) s in x :: xs, sum + x, s') alpha ([], 0.0, s0)
         List.map (fun y' -> y' / sum) y, s'
      )

[<CompiledName("Multinomial")>]
let multinomial (n, weight) =
   if n <= 0 then
      outOfRange "n" "`n' must be positive."
   let k = List.length weight
   if k < 2 then
      invalidArg "weight" "`weight' must contain two or more values."
   elif List.exists (fun x -> isNaN x || isInfinity x || x <= 0.0) weight then
      outOfRange "probability" "All elements in `weight' must be positive."
   else
      let cdf = cdf weight
      GeneratorFunction (fun s0 ->
         let result = Array.zeroCreate k
         let mutable s = s0
         for loop = 1 to n do
            let u, s' = Random.next ``[0, 1)`` s
            let p = u
            let index = List.findIndex (fun x -> p < x) cdf
            result.[index] <- result.[index] + 1
            s <- s'
         Array.toList result, s
      )

[<CompiledName("Normal")>]
let multinormal (mu, sigma) =
   let n = Array.length mu
   if Array2D.length1 sigma <> n || Array2D.length2 sigma <> n then
      invalidArg "sigma" "Invalid size of covariance matrix."
   elif Matrix.existsDiag (fun x -> x <= 0.0) sigma then
      invalidArg "sigma" "Found non-positive diagonal element."
   elif not (Matrix.isSymmetric sigma) then
      invalidArg "sigma" "Not a symmetric matrix."
   else
      let eigenvalues, eigenvectors = Matrix.jacobi sigma
      if Array.exists (fun x -> x < 0.0) eigenvalues then
         invalidArg "sigma" "Not a positive semidefinite matrix."
      else
         let d = Array.map sqrt eigenvalues |> Matrix.diagByVector
         let q = Matrix.multiply eigenvectors d
         let mu = Array.copy mu  // Modification of mu outside affects the transformation
         let standard = Array.randomCreate n Standard.normal
         let transform = Matrix.multiplyVector q >> Vector.add mu
         Random.map transform standard

[<CompiledName("Wishart")>]
let wishart (df, sigma) =
   if Array2D.length1 sigma > df || Array2D.length2 sigma > df then
      invalidArg "degreeOfFreedom" "The degree of freedom must be greater than the size of the covariance matrix."
   elif not (Matrix.isSymmetric sigma) then
      invalidArg "sigma" "Not a symmetric matrix."
   elif Matrix.existsDiag (fun x -> x <= 0.0) sigma then
      invalidArg "sigma" "Found non-positive diagonal element."
   else
      let eigenvalues, eigenvectors = Matrix.jacobi sigma
      if Array.exists (fun x -> x < 0.0) eigenvalues then
         invalidArg "sigma" "Not a positive semidefinite matrix."
      else
         let n = Array.length eigenvalues
         let d = Array.map sqrt eigenvalues |> Matrix.diagByVector
         let q = Matrix.multiply eigenvectors d
         let q' = Matrix.transpose q
         let transform =
            List.map Vector.transposeCross
            >> List.reduce Matrix.add
            >> Matrix.multiply q
            >> flip Matrix.multiply q'
         Array.randomCreate n Standard.normal
         |> List.replicate df
         |> Random.mergeWith transform

[<CompiledName("Mix")>]
let mix model =
   let distribution = List.map fst model |> List.toArray
   let cdf = List.map snd model |> cdf
   GeneratorFunction (fun s0 ->
      let u, s' = Random.next ``[0, 1)`` s0
      let p = u
      let index = List.findIndex (fun x -> p < x) cdf
      Random.next distribution.[index] s'
   )

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Seq =
   [<CompiledName("MarkovChain")>]
   let markovChain generator =
      let f = generator >> Random.next
      fun x0 s0 -> seq {
         let x = ref x0
         let s = ref s0
         while true do
            let x', s' = f !x !s
            yield x'
            x := x'
            s := s'
      }
