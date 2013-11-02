(*** hide ***)
#I "../Build"
#r "FsRandom.dll"
open FsRandom

let state = createState xorshift (123456789u, 362436069u, 521288629u, 88675123u)

(**
<a name="numerical-examples"></a>
Numerical Examples
==================

This page illustrates how FsRandom is applicable to scientific computations.
For statistics, the Statistics module is useful:
*)

open FsRandom.Statistics

(**
<a name="monte-carlo"></a>
Estimating pi, the ratio of a circle's circumference to its diameter
--------------------------------------------------------------------

Suppose there is a circle of radius 1 is inside a square with side length 2.
The area of the circle is \\(\pi\\) and the area of the square is 4.
If you put \\(N\\) random points on the square, roughly \\(\displaystyle\frac{\pi}{4}N\\) points are inside the circle.
In other words, if you find \\(M\\) points are inside the circle, \\(M\\) approximates \\(\displaystyle\frac{\pi}{4}\\).

<div style="text-align:center;margin-right:100px;">
<img src="images/monte-carlo-pi.png" width="512" height="512" alt="random points approximates pi" />
</div>

Random points can be described simply and easily as follows:
*)

let randomPointGenerator = random {
   let! x = uniform (-1.0, 1.0)
   let! y = uniform (-1.0, 1.0)
   return (x, y)
}

(**
To give each point weight 4 if the point is inside the circle and 0 otherwise
adjusts the average of total score to become \\(\pi\\).
To generate the sequence of the scores:
*)

let weight (x, y) = if x * x + y * y <= 1.0 then 4.0 else 0.0
let scores = Seq.ofRandom (Random.transformBy weight randomPointGenerator)

(**
Then, the average of the sequence approximates \\(\pi\\).
To generate 1,000,000 scores and to compute the average, the approximation of \\(\pi\\):
*)

scores state
|> Seq.take 1000000
|> Seq.average
|> printfn "%f"

(**
<a name="gibbs-sampler"></a>
Generating bivariate normal random numbers using Gibbs sampler
--------------------------------------------------------------

To sample from bivariate normal distribution
\\(\displaystyle N\_{2}\left(
\left[\begin{array}{c}
  \mu\_{X} \\\\
  \mu\_{Y}
\end{array}\right],
\left[\begin{array}{cc}
  \sigma\_{X}^{2} & \sigma\_{XY} \\\\
  \sigma\_{XY} & \sigma\_{Y}^{2}
\end{array}\right]
\right) \\),
we will construct a Gibbs sampler.
Let \\(f\_{2}(x, y)\\) be the density function of \\(N\_{2}\\),
and let \\(x'\\) and \\(y'\\) be \\(x-\mu\_{X}\\) and \\(y-\mu\_{Y}\\) respectively.
Then,
$$
\begin{eqnarray}
f\_{2}(x, y) & \propto & \exp\left(
-\frac{1}{2}\left[\begin{array}{c}
x' \\\\
y'
\end{array}\right]^{T}
\left[\begin{array}{cc}
  \sigma\_{X}^{2} & \sigma\_{XY} \\\\
  \sigma\_{XY} & \sigma\_{Y}^{2}
\end{array}\right]^{-1}
\left[\begin{array}{c}
x' \\\\
y'
\end{array}\right]
\right) \\\\
& \propto & \exp\left(
-\frac{\left(x'-\sigma\_{XY}y'/\sigma\_{Y}^{2}\right)^{2}}{2\left(\sigma\_{X}^{2}-\sigma\_{XY}^{2}/\sigma\_{Y}^{2}\right)}
\right)
\end{eqnarray}
$$
This means the conditional probability of \\(x\\) given \\(y\\) is distributed normally,
and its mean is \\(\displaystyle \mu\_{X}+\frac{\sigma\_{XY}}{\sigma\_{Y}^{2}}\left(y-\mu\_{Y}\right)\\)
and its variance is \\(\displaystyle \sigma\_{X}^{2}-\frac{\sigma\_{XY}^{2}}{\sigma\_{Y}^{2}}\\).
Therefore, the Gibbs sampler for bivariate normal distribution consists of iterating as the following:

1. Draw \\(\displaystyle x\_{t+1}\sim N\left(\mu\_{X}+\frac{\sigma\_{XY}}{\sigma\_{Y}^{2}}(y\_{t}-\mu\_{Y}), \sigma\_{X}^{2}-\frac{\sigma\_{XY}^{2}}{\sigma\_{Y}^{2}}\right)\\)
2. Draw \\(\displaystyle y\_{t+1}\sim N\left(\mu\_{Y}+\frac{\sigma\_{XY}}{\sigma\_{X}^{2}}(x\_{t+1}-\mu\_{Y}), \sigma\_{Y}^{2}-\frac{\sigma\_{XY}^{2}}{\sigma\_{X}^{2}}\right)\\)

And it can be naturally translated into F# code as the following.
*)

let gibbsBinormal (meanX, meanY, varX, varY, cov) (_ : float, y : float) =
   let sdx = sqrt <| varX - cov ** 2.0 / varY
   let sdy = sqrt <| varY - cov ** 2.0 / varX
   random {
      let! x' = normal (meanX + cov * (y - meanY) / varY, sdx)
      let! y' = normal (meanY + cov * (x' - meanX) / varX, sdy)
      return (x', y')
   }
let binormal parameter = Seq.markovChain (gibbsBinormal parameter)

(**
Note that the generating bivariate normal random number sequence is [autocorrelated](http://en.wikipedia.org/wiki/Autocorrelation).
*)
