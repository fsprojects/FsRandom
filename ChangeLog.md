FsRandom ChangeLog
==================

Version 1.3.2
-------------

* Fix List module throws StackOverflowException (#77).
* Add primitive random generators (#75).
* Add negative binomial generator (#50).

Version 1.3.1
-------------

* Fix bug (#74).

Version 1.3
-----------

* C# support (#64).
* Add Random.bind function.
* Add sample functions that take one sample from array (#73).

Version 1.2.3
-------------

* Add utility random state (#61).
* Add List module (#70).
* Add or rename functions (#66, #68, #69).
* Fix a bug (#65).

Version 1.2.2
-------------

* Add functions (multinormal, #46; wishart, #49; vonMises, #62).

Version 1.2.1
-------------

* Fix geometric generates random numbers incorrectly (#58),
  and support geometric on {0, 1, 2, ...} and {1, 2, 3, ...} (#59).
  Buggy geometric is removed and geometric0 and geometric1 on the respective supports are added.
* Add Statistics.Standard module (#55).
* sampleWithReplacement new throws an error before runtime (#45).
* Add functions.

Version 1.2
-----------

* API change (#40). PrngState no longer requires generics.
* Support use binding and error handling in random computation expression (#43).
* Add String module (#23).
* Add several useful functions.

Version 1.1
-----------

* API change (#35).
* Add Array2D module (#38).
* Add useful functions.

Version 1.0.2
-------------

* Add GeneratorFunction type abbreviation.
* Add choose function (#34).

Version 1.0.1
-------------

* Enhance speed of Poisson generator.
* Add rawBits generator function.

Version 1.0.0.0
---------------

* Rename project.
* RandomBuilder.Run returns a function.
* Added Seq module (#27).
* High resolution of random numbers (#31).

Version 1.2.2.0 (RecycleBin.Random)
-----------------------------------

* Enhanced Array module (#22, #25, #26).

Version 1.2.1.0 (RecycleBin.Random)
-----------------------------------

* Bug fix.

Version 1.2.0.0 (RecycleBin.Random)
-----------------------------------

* Functions moved to separated modules (#15).
* Added functions (#16, #18, #20).

Version 1.1.4.0 (RecycleBin.Random)
-----------------------------------
Tagged: 265f8ddfb416f2c8804e4a5af5465e21fbc31908

* Added `coinFlip` function (#2).
* Added `trialgular` function (#11).
* Added `loguniform` function (#12).
* Added `lognormal` function (#13).
* Added `multinomial` function (#14).

Version 1.1.3.0 (RecycleBin.Random)
-----------------------------------
Tagged: 6d91d3df1ddc6aae4d52710fb56856c75f5125d6

* Fixed SimdOrientedFastMersenneTwister implementation.

Version 1.1.2.0 (RecycleBin.Random)
-----------------------------------
Tagged: 1cc5c0474b9048862b849bededd00aa26f9ae1bd

* Added Mersenne Twister implementation (#7).
* Added SIMD-Oriented Fast Mersenne Twister implementation (#8).

Version 1.1.1.0 (RecycleBin.Random)
-----------------------------------
Tagged: 17920ee5ad02b48dd7e4d5888ce3c7343bb67069

* Added `state` builder (#9).

Version 1.1.0.0 (RecycleBin.Random)
-----------------------------------
Tagged: 930858a0b1454057f4461ce5fd788a771d1b2a79

* Supported loops (#4).
* Random number conversion functions (#5).

Version 1.0.0.0 (RecycleBin.Random)
-----------------------------------
Tagged: db812be59c6fafc63a1169ae85887361cbb5bb14

* Initial release.
