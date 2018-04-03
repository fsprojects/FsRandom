(*** hide ***)
#I "../Build/lib/net45"
#r "FsRandom.dll"

(**
FsRandom
========

FsRandom is a purely-functional random number generator framework designed for F# language.
It helps you to obtain a variety of random numbers to use more than ten predefined generators,
and to define a new function to generate random numbers you want.

How to Install
--------------

### Install from NuGet

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      FsRandom is <a href="https://nuget.org/packages/FsRandom/">available on the NuGet Gallery</a>.
      Run in the Package Manager Console:
      <pre>PM> Install-Package FsRandom</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

### Build from source code

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      <a href="http://fsharp.github.io/FAKE/">FAKE</a> script is included in the source code.
      To make a debug build, run:
      <pre>> fsi tools\build.fsx --debug</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Short Example
-------------
*)

(*** define-output:randomPoint ***)
open FsRandom

// Random state
let state = createState xorshift (123456789u, 362436069u, 521288629u, 88675123u)

// Random point generator
let randomPointGenerator = random {
   let! x = ``[0, 1)``  // generates a random number between 0 and 1
   let! y = Statistics.normal (0.0, 1.0)  // generates a normal random number
   return (x, y)
}

// Get a random point
let randomPoint = Random.get randomPointGenerator state
printf "(x, y) = (%f, %f)" <|| randomPoint

(**
The script yields:
*)
(*** include-output:randomPoint ***)

(**
Features
--------
### Random Functions

FsRandom provides a variety of random number generator functions:

* **RandomNumberGenerator module** provides standard random number generators:
  <code>\`\`(0, 1)\`\`</code>, <code>\`\`[0, 1)\`\`</code>, <code>\`\`(0, 1]\`\`</code>, and <code>\`\`[0, 1]\`\`</code>.
* **Random module** manipulates random numbers.
* **Statistics module** provides a variety of statistical distributions such like `uniform`, `normal` and `gamma`.
* **Seq module** provides functions for generating random number sequences.
* **Array module** and **Array2D module** provide functions for array operations like `createRandom`, `sample`, `sampleWithReplacement`, and `shuffle`.
* **List module** provides functions for generating random lists.
* **String module** provides functions for generating random strings.
* **Utility module** provides utility functions.

### Pseudo-Random Number Generators

You can choose an algorithm of pseudo-random numbger generator:

* **xorshift** implements [Xorshift](http://en.wikipedia.org/wiki/Xorshift) algorithm.
* **systemrandom** leverages System.Random and its subclasses as a random number generators.
  This PRNG is not purely functional because the state of the PRNG is controled in the classes.
* **mersenne** in MersenneTwister module implements 64-bit version of [Mersenne Twister](http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html) algorithm.
* **sfmt** in SimdOrientedFastMersenneTwister module implements [SFMT](http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/SFMT/index.html) algorithm.
*)
