FsRandom From C#
================

From version 1.3, FsRandom supports C#'s query syntax.

    [lang=csharp]
    using FsRandom;
    using RNG = FsRandom.RandomNumberGenerator;

    var generator = from x in RNG.Standard  // [0, 1)
                    from y in RNG.Standard
                    let z = x + y
                    select z / 2;
    var result = RandomModule.Get(generator, UtilityModule.DefaultState);
