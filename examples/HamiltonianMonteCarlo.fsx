#I "../Build"
#r "FsRandom.dll"

open FsRandom
open FsRandom.Statistics

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed

let inline updateWith f (ys:float []) (xs:float []) =
   for index = 0 to Array.length xs - 1 do
      xs.[index] <- f xs.[index] ys.[index]

/// Hamiltonian Monte Carlo
let hmc minusLogDensity gradMinusLogDensity epsilon step =
   /// Leapfrog integration
   let leapfrog q p =
      updateWith (fun x y -> x + 0.5 * epsilon * y) (gradMinusLogDensity q) p
      for i = 1 to step - 1 do
         updateWith (fun x y -> x + epsilon * y) p q
         updateWith (fun x y -> x - epsilon * y) (gradMinusLogDensity q) p
      updateWith (fun x y -> x + epsilon * y) p q
      updateWith (fun x y -> -x + 0.5 * epsilon * y) (gradMinusLogDensity q) p
   /// Hamiltonian
   let hamiltonian q p =
      let potential = minusLogDensity q
      let kinetic = Array.fold2 (fun acc x y -> acc + x * y) 0.0 p p / 2.0
      potential + kinetic
   fun currentQ -> random {
      let q = Array.copy currentQ
      // resampling of particles
      let! currentP = Array.randomCreate (Array.length currentQ) (normal (0.0, 1.0))
      let p = Array.copy currentP
      leapfrog q p
      let currentH = hamiltonian currentQ currentP
      let proposedH = hamiltonian q p
      let! r = ``[0, 1)``
      return if r < exp (currentH - proposedH) then q else currentQ
   }
   
// Sampling from N2 with correlation coefficient of 0.95.
let r = 0.95
let detSigma = 1.0 * 1.0 - r * r  // determinant of variance-covariance matrix
let initialPoint = [|0.0; 0.0|]
let minusLogF2 (xy:float []) =
   let x = xy.[0]
   let y = xy.[1]
   (x * x - 2.0 * r * x * y + y * y) / (2.0 * detSigma)
let gradMinusLogF2 (xy:float []) =
   let x = xy.[0]
   let y = xy.[1]
   [|(x - r * y) / detSigma; (-r * x + y) / detSigma|]
let sampler = Seq.markovChain (hmc minusLogF2 gradMinusLogF2 1.0e-2 2000) initialPoint state

sampler
|> Seq.take 1000
|> Seq.iter (fun xy -> printfn "%6.3f\t%6.3f" xy.[0] xy.[1])
