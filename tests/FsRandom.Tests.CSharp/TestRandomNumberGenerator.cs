using System;
using NUnit.Framework;
using RNG = FsRandom.RandomNumberGenerator;

namespace FsRandom
{
   [TestFixture]
   public class TestRandomNumberGenerator
   {
      [Test]
      public void CanGenerateRandom()
      {
         var seed = Tuple.Create(123456789u, 362436069u, 521288629u, 88675123u);
         var state = RNG.CreateState(RNG.XorshiftPrng, seed);
         var r1 = RandomModule.Next(RNG.RawBits, state);
         var r2 = RandomModule.Next(RNG.RawBits, r1.Item2);
         Assert.That(r2.Item1, Is.Not.EqualTo(r1.Item1));
      }

      [Test]
      public void CanUseSingleSelectQueryExpression()
      {
         var g = from x in RNG.RawBits
                 select x + 1uL;
         var actual = RandomModule.Get(g, UtilityModule.DefaultState);
         var expected = RandomModule.Get(RNG.RawBits, UtilityModule.DefaultState) + 1uL;
         Assert.That(actual, Is.EqualTo(expected));
      }

      [Test]
      public void CanUseSelectManyQueryExpression()
      {
         var g = from x in RNG.RawBits
                 from y in RNG.RawBits
                 let z = x ^ y
                 select z + 1uL;
         var actual = RandomModule.Get(g, UtilityModule.DefaultState);
         var expectedX = RandomModule.Next(RNG.RawBits, UtilityModule.DefaultState);
         var expectedY = RandomModule.Next(RNG.RawBits, expectedX.Item2);
         Assert.That(actual, Is.EqualTo((expectedX.Item1 ^ expectedY.Item1) + 1uL));
      }
   }
}
