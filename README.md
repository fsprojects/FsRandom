RecycleBin.Random
=================

RecycleBin.Random is a random number generator framework designed for F# language.
It helps you to obtain a variety of random numbers to use more than ten predefined generators,
and to define a new function to generate random numbers you want.

Supported Randoms
-----------------

* Standard random
  * (0, 1)
  * [0, 1)
  * (0, 1]
  * [0, 1]
* Continuous (univariate)
  * uniform
  * normal
  * gamma
  * beta
  * exponential
  * Cauchy
  * chi-square
  * Student's t
* Discrete (univariate)
  * uniform
  * poisson
  * geometric
  * Bernoulli
  * binomial
* Continuous (multivariate)
  * Dirichlet
* User-defined (see below)

How to Install
--------------

RecycleBin.Random is [available on the NuGet Gallery](https://nuget.org/packages/RecycleBin.Random/).
Use the following command to install RecycleBin.Random via NuGet.

```
Install-Package RecycleBin.Random
```

[Visit the site](https://nuget.org/) for more information.

How to Use
----------

### First random number

Let's try to get a first random number `z` ~ N(0, 1).

We need to define an initial random seed first.
It is a tuple composed of four 32-bit unsigned integers (`uint32`)
because we're going to use xorshift algorithm to generate random numbers here.

```fsharp
let initialSeed = 123456789u, 362436069u, 521288629u, 88675123u
```

Because we want a random number `z` ~ N(0, 1),
we should use `normal` random number generator with parameters `(0.0, 1.0)`.

```fsharp
let generator = normal (0.0, 1.0)
```

Then, we can use the generator and get the result.

```fsharp 
let z, nextSeed = xorshift initialSeed { return! generator }
printfn "%f" z
```

The random number generating computation expression (`xorshift` here) takes a random seed
and returns a tuple with a random number that we request (~ N(0, 1) here) and a new state after the generation of the random number.
It is important to keep the new state because it is used when we generate a new random number.

So we should use `nextSeed` to generate a new random number ~ N(0, 1) as the following.

```fsharp
let z2, _ = xorshift nextSeed { return! generator }
printfn "%f" z2
```

### Random number sequence

Usually, we use a lot of random numbers for our needs.
The following code defines a function generating an infinite binary sequence
using Bernoulli random number generator,
and it illustrates how we can generate a number of random numbers.

```fsharp
let rec binaries initialSeed =
   seq {
      let binary, nextSeed = xorshift initialSeed { return! bernoulli 0.5 }
      yield binary
      yield! binaries nextSeed  // recursively generating binaries.
   }
```

### Using `System.Random`

The examples above uses `xorshift` expression to generate random numbers.
The familliar `System.Random` (and its subclasses) is available in the workflow.
Just use `systemrandom` instead of `xorshift`.

```fsharp
let r0 = System.Random ()
let u, r1 = systemrandom r0 { return! gamma (2.0, 1.0) }
```

Because an instance of `System.Random` keeps a state by itself,
to tell the truth, the second returned state value (`r1` here) by `systemrandom`
is the same reference as the instance passed in the call (`r0`).

```fsharp
printfn "%b" (r0 = r1)  // true
```

### Constructing a user-defined random number generator

#### Computation expression

This section explains how to construct random computation expressions such as `xorshift` and `systemrandom`.

RecycleBin.Random has a random computation expression builder named `random`,
which enables users to construct a user-defined random computation expression builder.
The `random` uses a pseudorandom number generating function, that is, a function
which has a type of `Prng<'s> = 's -> uint32 * 's` where `'s` is a type of state seed.
`Prng` is an actual random number generator which receives a random seed (`: 's`) and returns
a random number in 32-bit resolution (`: uint32`) and a next state (`: 's`) in the random computation expression.
As we saw above, RecycleBin.Random currently supports xorshift algorithm and `System.Random`.

As an example of user-defined `Prng`,
let's implement [linear congruential generator](http://en.wikipedia.org/wiki/Linear_congruential_generator).
First, we make a function of `Prng`.

```fsharp
// linearPrng : uint32 * uint32 -> uint32 -> uint32 * uint32
let linearPrng (a, c) x = x, a * x + c
```

The first returned value is a random number and the second returned value is a next state.
Note that modulus is not defined because `Prng` is required to return random numbers
in 32-bit resolution (modulus = 2^32).

Then, a new computation expression builder can be defined as the following.

```fsharp
let linear (a, c) seed = random (linearPrng (a, c)) seed
```

Hereafter we can use the `linear` builder to generate random numbers.

```fsharp
let seed = uint32 Environment.TickCount
let myLinear = linear (1664525u, 1013904223u)  // Numerical Recipes
let u, nextSeed = myLinear seed { return! gamma (3.0, 1.0) }
```

#### Generator function

This section explains how to construct generator functions such like `normal` and `uniform`.

Random number generator functions are just required to return (`PrngState<'s> -> 'a * PrngState<'s>`)
where `'s` is a type of random seed and `'a` is a type of random number.
`PrngState<'s>` is defined as `Prng<'s> * 's` and `Prng<'s>` is `'s -> uint32 * 's`.

As an example of user-defined generator function,
let's construct a random number generator to produce an *approximate*
standard normal random number (approximately ~ N(0, 1)).
Theorem says that the mean of 12 standard random numbers,
namely, 12 random numbers between 0 and 1, approximates a normal random number
with mean of 1/2 and variance of 1/12.
Therefore, if we subtract 6 from the sum of 12 standard random numbers, the result
approximates a standard normal random number.

```fsharp
let approximatelyStandardNormal (s0 : PrngState<'s>) =
   let u1, s1 = ``(0, 1)`` s0  // ``(0, 1)`` is a standard random number generator in (0, 1)
   let u2, s2 = ``(0, 1)`` s1
   let u3, s3 = ``(0, 1)`` s2
   let u4, s4 = ``(0, 1)`` s3
   let u5, s5 = ``(0, 1)`` s4
   let u6, s6 = ``(0, 1)`` s5
   let u7, s7 = ``(0, 1)`` s6
   let u8, s8 = ``(0, 1)`` s7
   let u9, s9 = ``(0, 1)`` s8
   let u10, s10 = ``(0, 1)`` s9
   let u11, s11 = ``(0, 1)`` s10
   let u12, s12 = ``(0, 1)`` s11
   let approximation = u1 + u2 + u3 + u4 + u5 + u6 + u7 + u8 + u9 + u10 + u11 + u12 - 6.0
   approximation, s12  // returns the random number and the last state
```

The `approximatelyStandardNormal` can be used in the generating process as the following.

```fsharp
let z = fst <| xorshift seed { return! approximatelyStandardNormal }
printnf "%f" z
```

Don't forget that RecycleBin.Random has a normal random number generator `normal`.

Numerical Examples
------------------

### Estimating pi, the ratio of a circle's circumference to its diameter.

```fsharp
// Generates random points on [-1, 1] x [-1, 1].
let rec randomPoints initialSeed =
   seq {
      let point, nextSeed =
         xorshift initialSeed {
            let! x = uniform (-1.0, 1.0)
            let! y = uniform (-1.0, 1.0)
            return (x, y)
         }
      yield point
      yield! randomPoints nextSeed
   }
// Weight of a point
// If the distance from (0, 0) is equal to or less than 1 (in the unit circle),
// the weight is 4 (because random points are distributed on [-1, 1] x [-1, 1]).
let weight (x, y) = if x * x + y * y <= 1.0 then 4.0 else 0.0

randomPoints (123456789u, 362436069u, 521288629u, 88675123u)
|> Seq.take 1000000
|> Seq.averageBy weight
|> printfn "%f"
```

### Generating bivariate normal random numbers using Gibbs sampler

To sample from bivariate normal distribution N2([meanX, meanY]^t, [[varX, cov]^t, [cov, varY]]),
we will construct a Gibbs sample.
Because the density function f(x, y) is propotional to f(x | y) * f(y) and
f(x | y) is propotinal to p(meanX + (cov / varY) * (y - meanY), varX - cov^2 / varY)
where p is a univariate normal density function,
the Gibbs sampler for bivariate normal distribution consists of iterating as the following:

1. Draw x[t + 1] ~ N(meanX + (cov / varY) * (y[t] - meanY), varX - cov^2 / varY)
2. Draw y[t + 1] ~ N(meanY + (cov / varX) * (x[t + 1] - meanX), varY - cov^2 / varX)

And it can be naturally translated into F# code as the following.

```fsharp
let rec binormal (meanX, meanY, varX, varY, cov as parameter) ((_, y), seed as state) =
   seq {
      let next =
         xorshift seed {
            // Pay attention to that `normal' takes not variance but standard deviation.
            let! x' = normal (meanX + cov * (y - meanY) / varY, sqrt <| varX - cov ** 2.0 / varY)
            let! y' = normal (meanY + cov * (x' - meanX) / varX, sqrt <| varY - cov ** 2.0 / varX)
            return (x', y')
         }
      yield fst next
      yield! binormal parameter next
   }
```

Note that the generating bivariate normal random number sequence is [autocorrelated](http://en.wikipedia.org/wiki/Autocorrelation).

Related Projects
----------------

* [Math.NET Numerics](https://github.com/mathnet/mathnet-numerics/) -- An opensource numerical library for .NET, Silverlight and Mono.

