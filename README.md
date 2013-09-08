FsRandom
========

FsRandom (formerly RecycleBin.Random) is a (purely-functional) random number generator framework designed for F# language.
It helps you to obtain a variety of random numbers to use more than ten predefined generators,
and to define a new function to generate random numbers you want.

Randoms Functions
-----------------

* **RandomNumberGenerator module** provides standard random number generators: <code>\`\`(0, 1)\`\`</code>, <code>\`\`[0, 1)\`\`</code>, <code>\`\`(0, 1]\`\`</code>, and <code>\`\`[0, 1]\`\`</code>.
* **Random module** manipulates random numbers.
* **Statistics module** provides a variety of statistical distributions such like `uniform`, `normal` and `gamma`.
* **Seq module** provides functions for generating random number sequences.
* **Array module** provides functions for array operations like `shuffle`.
* **Utility module** provides utility functions.

Also, user-defined functions can be implemented easily (see below).

How to Install
--------------

### Install from NuGet

FsRandom is [available on the NuGet Gallery](https://nuget.org/packages/FsRandom/).
Use the following command to install FsRandom via NuGet.

```
Install-Package FsRandom
```

[Visit the site](https://nuget.org/) for more information.

### Build from source code

Run:

```
fsi tools\build.fsx --debug --no-deploy
```

How to Use
----------

### First random number

Let's try to get a first random number `z` ~ N(0, 1).
It is easy to do with `normal` random number generator in the Statistics module.
To give the specific parameter, say the mean of 0 and the variance of 1, do:

```fsharp
let generator<'s> : GeneratorFunction<'s, _> = Statistics.normal (0.0, 1.0)
```

`'s` is a random state type described below.
The generator function is only able to use with a pseudo-random number generator (PRNG).
The PRNG constructs a computation expression to generate random numbers.
The computation expression is a function which takes a random seed and returns a random number and a new seed for the next call.
It is important to keep the new state because it is used when we generate a new random number.

Here for example, we choose xorshift PRNG, which is implemented in FsRandom.
We need to define an initial random seed first for xorshift algorithm
(of course, another algorithm is available rather than xorshift, as described later).
It is a tuple composed of four 32-bit unsigned integers.
And then, we should combine the PRNG and the seed using `createState` function.

```fsharp
let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed
```

Now we can retrieve a random number using `Random.get` function.

```
let z = Random.get generator state
printfn "%f" z
```

Since `Random.get` returns a stateless function,
if you do the code above, the same thing occurs.
To generate a new random number,
we need to get next state using `Random.next` instead of `Random.get`:

```fsharp
let _, nextState = Random.next generator state
let z2 = Random.get generator nextState
printfn "%f" z2
```

### Transforming random numbers

Transformation of random numbers is a regular work.
FsRandom defines `Random.transformBy` function for the purpose.
The following code shows how to use it.

```fsharp
let plusOne x = x + 1.0
Random.transformBy plusOne <| Statistics.uniform (0.0, 1.0)
|> Random.get
<| state
```

`plusOne` is a function that takes an argument and adds one to it.
`uniform` is a uniform random number generator between its two arguments.
So `x` finally becomes a uniform random number between 1 and 2.

The both following codes return the same results as above.

``` fsharp
Random.identity <| Statistics.uniform (0.0, 1.0)
|> Random.get
<| state
|> plusOne
```

```fsharp
Statistics.uniform (0.0, 1.0)
|> Random.get
<| state
|> plusOne
```

### Random number sequence

Usually, we use a lot of random numbers for our needs.
The following code defines a function generating an infinite binary sequence
using Bernoulli random number generator,
and it illustrates how we can generate a number of random numbers.

```fsharp
let rec binaries initialState = seq {
   let binary, nextState = Random.next (Statistics.bernoulli 0.5) initialState
   yield binary
   yield! binaries nextState // recursively generating binaries.
}
```

Or, more precisely like the following:

```fsharp
let binaries state = Seq.ofRandom (Statistics.bernoulli<'a> 0.5) state
```

### Using `System.Random`

The examples above uses `xorshift` to generate random numbers.
The familliar `System.Random` (and its subclasses) is available in the workflow.
Just use `systemrandom` instead of `xorshift`.

```fsharp
let r0 = System.Random ()
let s = createState systemrandom r0
```

Because `System.Random` is a stateful object,
unlike `xorshift`, we will get different result on each call.

```fsharp
let u1 = Random.get generator s
let u2 = Random.get generator s
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
// Coefficients are cited from Wikipedi
let linear x = x, 6364136223846793005uL * x + 1442695040888963407uL
```

The first returned value is a random number and the second returned value is a next state.
Note that modulus is not defined because `Prng` is required to return random numbers
in 64-bit resolution.

Hereafter we can use the `linear` builder to generate random numbers.

```fsharp
let linearState = createState linear 0x123456789ABCDEFuL
Random.get generator linearState
```

#### Generator function

This section explains how to construct generator functions such like `normal` and `uniform`.

The type of generator function is `GeneratorFunction<'s, 'a>`,
where `'s` is a type of random seed and `'a` is a type of random number.
`'s` should be generic whenever we define new generator functions,
since it is determined by random number generators.

As an example of user-defined generator function,
let's construct a random number generator to produce an *approximate*
standard normal random number (approximately ~ N(0, 1)).
Theorem says that the mean of 12 standard random numbers,
namely, 12 random numbers between 0 and 1, approximates a normal random number
with mean of 1/2 and variance of 1/12.
Therefore, if we subtract 6 from the sum of 12 standard random numbers, the result
approximates a standard normal random number.

```fsharp
let approximatelyStandardNormal<'s> : GeneratorFunction<'s, _> = random {
   let! values = Array.randomCreate 12 ``(0, 1)``  // ``(0, 1)`` is a standard random number generator in (0, 1)
   return Array.sum values - 6.0
}
```

The `approximatelyStandardNormal` can be used in the generating process as the following.

```fsharp
Random.get approximatelyStandardNormal state
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
// Weight of a point
// If the distance from (0, 0) is equal to or less than 1 (in the unit circle),
// the weight is 4 (because random points are distributed on [-1, 1] x [-1, 1]).
let weight (x, y) = if x * x + y * y <= 1.0 then 4.0 else 0.0
// Function to generate a sequence
let values = Seq.ofRandom (Random.transformBy weight randomPointGenerator)

// Monte Carlo integration
// Generates 1,000,000 random values and the average becomes estimator of pi
values state
|> Seq.take 1000000
|> Seq.average
|> printfn "%f"
```

### Generating bivariate normal random numbers using Gibbs sampler

To sample from bivariate normal distribution
![n2](http://chart.apis.google.com/chart?cht=tx&chl=N_{2}%5cleft%28%5cbegin{bmatrix}%5cmu_{X}%5c%5c%5cmu_{Y}%5cend{bmatrix},%5c,%5cbegin{bmatrix}%5csigma_{X}^{2}%26%5csigma_{XY}%5c%5c%5csigma_{XY}%26%5csigma_{Y}^{2}%5cend{bmatrix}%5cright%29),
we will construct a Gibbs sample.
Because the density function f(x, y) is propotional to f(x | y) * f(y) and
f(x | y) is propotinal to
![p](http://chart.apis.google.com/chart?cht=tx&chl=p%5cleft%28%5cmu_{X}%2b%5cfrac{%5csigma_{XY}}{%5csigma_{Y}^{2}}%28y-%5cmu_{Y}%29,%5c,%5csigma_{X}^{2}-%5cfrac{%5csigma_{XY}^{2}}{%5csigma_{Y}^{2}}%5cright%29)
where p is a univariate normal density function,
the Gibbs sampler for bivariate normal distribution consists of iterating as the following:

1. Draw ![draw_x](http://chart.apis.google.com/chart?cht=tx&chl=x_{t%2b1}%5csim+N%5cleft%28%5cmu_{X}%2b%5cfrac{%5csigma_{XY}}{%5csigma_{Y}^{2}}%28y_{t}-%5cmu_{Y}%29,%5c,%5csigma_{X}^{2}-%5cfrac{%5csigma_{XY}^{2}}{%5csigma_{Y}^{2}}%5cright%29)
2. Draw ![draw_y](http://chart.apis.google.com/chart?cht=tx&chl=y_{t%2b1}%5csim+N%5cleft%28%5cmu_{Y}%2b%5cfrac{%5csigma_{XY}}{%5csigma_{X}^{2}}%28x_{t%2b1}-%5cmu_{X}%29,%5c,%5csigma_{Y}^{2}-%5cfrac{%5csigma_{XY}^{2}}{%5csigma_{X}^{2}}%5cright%29)

And it can be naturally translated into F# code as the following.

```fsharp
open FsRandom.Statistics
let gibbsBinormal (meanX, meanY, varX, varY, cov) (_ : float, y : float) = random {
   let! x' = normal (meanX + cov * (y - meanY) / varY, sqrt <| varX - cov ** 2.0 / varY)
   let! y' = normal (meanY + cov * (x' - meanX) / varX, sqrt <| varY - cov ** 2.0 / varX)
   return (x', y')
}
let binormal parameter = Seq.markovChain (gibbsBinormal parameter)
```

Note that the generating bivariate normal random number sequence is [autocorrelated](http://en.wikipedia.org/wiki/Autocorrelation).

