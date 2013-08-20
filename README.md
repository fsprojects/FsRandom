FsRandom
========

FsRandom (formerly RecycleBin.Random) is a random number generator framework designed for F# language.
It helps you to obtain a variety of random numbers to use more than ten predefined generators,
and to define a new function to generate random numbers you want.

Randoms Functions
-----------------

* **RandomNumberGenerator module** provides standard random number generators: <code>\`\`(0, 1)\`\`</code>, <code>\`\`[0, 1)\`\`</code>, <code>\`\`(0, 1]\`\`</code>, and <code>\`\`[0, 1]\`\`</code>.
* **Statistics module** provides a variety of statistical distributions such like `uniform`, `normal` and `gamma`.
* **Seq module** provides functions for generating random number sequences.
* **Array module** provides functions for array operations like `shuffle`.
* **Utility module** provides utility functions.

Also, user-defined functions can be implemented easily (see below).

How to Install
--------------

FsRandom is [available on the NuGet Gallery](https://nuget.org/packages/FsRandom/).
Use the following command to install FsRandom via NuGet.

```
Install-Package FsRandom
```

[Visit the site](https://nuget.org/) for more information.

How to Build
------------

Run:

```
fsi tools\build.fsx --no-deploy
```

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
we should use `normal` random number generator in the Statistics module with parameters `(0.0, 1.0)`.

```fsharp
let generator = Statistics.normal (0.0, 1.0)
```

Then, we can use the generator and get the result.

```fsharp
let z, nextSeed = xorshift { return! generator } <| initialSeed
printfn "%f" z
```

The random number generating computation expression (`xorshift` here) takes a random seed
and returns a tuple with a random number that we request (~ N(0, 1) here) and a new state after the generation of the random number.
It is important to keep the new state because it is used when we generate a new random number.

So we should use `nextSeed` to generate a new random number ~ N(0, 1) as the following.

```fsharp
let z2, _ = xorshift { return! generator } <| nextSeed
printfn "%f" z2
```

### Transforming random numbers

Transformation of random numbers is a regular work.
FsRandom defines `getRandomBy` function for the purpose.
The following code shows how to use it.

```fsharp
let plusOne x = x + 1.0
xorshift {
   return! getRandomBy plusOne <| Statistics.uniform (0.0, 1.0)
}
```

`plusOne` is a function that takes an argument and adds one to it.
`uniform` is a uniform random number generator between its two arguments.
So `x` finally becomes a uniform random number between 1 and 2.

The both following codes return the same results as above.

``` fsharp
xorshift {
   let! u = getRandom <| Statistics.uniform (0.0, 1.0)
   return plusOne u
}
```

```fsharp
xorshift {
   let! u = Statistics.uniform (0.0, 1.0)
   return plusOne u
}
```

### Random number sequence

Usually, we use a lot of random numbers for our needs.
The following code defines a function generating an infinite binary sequence
using Bernoulli random number generator,
and it illustrates how we can generate a number of random numbers.

```fsharp
let generator = xorshift { return! Statistics.bernoulli 0.5 }
let rec binaries initialSeed =
   seq {
      let binary, nextSeed = generator initialSeed
      yield binary
      yield! binaries nextSeed  // recursively generating binaries.
   }
```

Or, more precisely like the following:

```fsharp
let binaries = Seq.ofRandom <| random { return! Statistics.bernoulli 0.5 } <| xorshift
```

### Using `System.Random`

The examples above uses `xorshift` expression to generate random numbers.
The familliar `System.Random` (and its subclasses) is available in the workflow.
Just use `systemrandom` instead of `xorshift`.

```fsharp
let r0 = System.Random ()
let u, r1 = systemrandom { return! Statistics.gamma (2.0, 1.0) } <| r0
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

FsRandom has a random computation expression builder named `random`,
which enables users to construct a user-defined random computation expression builder.
The `random` uses a pseudorandom number generating function, that is, a function
which has a type of `Prng<'s> = 's -> uint64 * 's` where `'s` is a type of state seed.
`Prng` is an actual random number generator which receives a random seed (`: 's`) and returns
a random number in 64-bit resolution (`: uint64`) and a next state (`: 's`) in the random computation expression.
As we saw above, FsRandom currently supports xorshift algorithm and `System.Random`.

As an example of user-defined `Prng`,
let's implement [linear congruential generator](http://en.wikipedia.org/wiki/Linear_congruential_generator).
First, we make a function of `Prng`.

```fsharp
// linearPrng : uint64 * uint64 -> uint64 -> uint64 * uint64
let linearPrng (a, c) x = x, a * x + c
```

The first returned value is a random number and the second returned value is a next state.
Note that modulus is not defined because `Prng` is required to return random numbers
in 64-bit resolution.

Then, a new computation expression builder can be defined as the following.

```fsharp
let linear (a, c) = createRandomBuilder (linearPrng (a, c))
```

Hereafter we can use the `linear` builder to generate random numbers.

```fsharp
let seed = uint64 Environment.TickCount
let myLinear = linear (6364136223846793005uL, 1442695040888963407uL)  // from Wikipedia
let generator = myLinear { return! Statistics.gamma (3.0, 1.0) }
let u, nextSeed = generator seed
```

#### Generator function

This section explains how to construct generator functions such like `normal` and `uniform`.

Random number generator functions are just required to return (`PrngState<'s> -> 'a * PrngState<'s>`)
where `'s` is a type of random seed and `'a` is a type of random number.
`PrngState<'s>` is defined as `Prng<'s> * 's` and `Prng<'s>` is `'s -> uint64 * 's`.
The type looks too complex, but implementation is not difficult as you see soon.

As an example of user-defined generator function,
let's construct a random number generator to produce an *approximate*
standard normal random number (approximately ~ N(0, 1)).
Theorem says that the mean of 12 standard random numbers,
namely, 12 random numbers between 0 and 1, approximates a normal random number
with mean of 1/2 and variance of 1/12.
Therefore, if we subtract 6 from the sum of 12 standard random numbers, the result
approximates a standard normal random number.

```fsharp
let approximatelyStandardNormal =
   random {
      let! values = Array.randomCreate 12 ``(0, 1)``  // ``(0, 1)`` is a standard random number generator in (0, 1)
      return Array.sum values - 6.0
   }
```

The `approximatelyStandardNormal` can be used in the generating process as the following.

```fsharp
let generator = xorshift { return! approximatelyStandardNormal }
let z = fst <| generator seed
printnf "%f" z
```

Don't forget that FsRandom has a normal random number generator `normal`.

Numerical Examples
------------------

### Estimating pi, the ratio of a circle's circumference to its diameter.

```fsharp
// Generates random points on [-1, 1] x [-1, 1].
let randomPointGenerator = random {
   let! x = Statistics.uniform (-1.0, 1.0)
   let! y = Statistics.uniform (-1.0, 1.0)
   return (x, y)
}
// Function to generate a sequence
let randomPoints = Seq.ofRandom randomPointGenerator
// Weight of a point
// If the distance from (0, 0) is equal to or less than 1 (in the unit circle),
// the weight is 4 (because random points are distributed on [-1, 1] x [-1, 1]).
let weight (x, y) = if x * x + y * y <= 1.0 then 4.0 else 0.0

// Generates 1,000,000 random points and estimates pi
randomPoints xorshift (123456789u, 362436069u, 521288629u, 88675123u)
|> Seq.take 1000000
|> Seq.averageBy weight
|> printfn "%f"
```

### Generating bivariate normal random numbers using Gibbs sampler

To sample from bivariate normal distribution N2([meanX, meanY]^t, [[varX, cov]^t, [cov, varY]^t]) (^t denotes transposition),
we will construct a Gibbs sample.
Because the density function f(x, y) is propotional to f(x | y) * f(y) and
f(x | y) is propotinal to p(meanX + (cov / varY) * (y - meanY), varX - cov^2 / varY)
where p is a univariate normal density function,
the Gibbs sampler for bivariate normal distribution consists of iterating as the following:

1. Draw x[t + 1] ~ N(meanX + (cov / varY) * (y[t] - meanY), varX - cov^2 / varY)
2. Draw y[t + 1] ~ N(meanY + (cov / varX) * (x[t + 1] - meanX), varY - cov^2 / varX)

And it can be naturally translated into F# code as the following.

```fsharp
open FsRandom.Statistics
let gibbsBinormal (meanX, meanY, varX, varY, cov) (_ : float, y : float) =
   random {
      let! x' = normal (meanX + cov * (y - meanY) / varY, sqrt <| varX - cov ** 2.0 / varY)
      let! y' = normal (meanY + cov * (x' - meanX) / varX, sqrt <| varY - cov ** 2.0 / varX)
      return (x', y')
   }
let binormal parameter = Seq.markovChain (gibbsBinormal parameter)
```

Note that the generating bivariate normal random number sequence is [autocorrelated](http://en.wikipedia.org/wiki/Autocorrelation).

