using Biosim.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BiosimTests
{
    public class TestCarnivore
    {
        [Fact]
        public void CarnivoreFeedingKillTest()
        {
            var rng = new Random();
            var testCarn = new Carnivore(rng) { Age = 5, Weight = 50 }; // Healthy Carnivore
            testCarn.Params.DeltaPhiMax = 0;
            var herbs = new List<Herbivore>();
            for (int i = 0; i < 10; i++)
            {
                herbs.Add(new Herbivore(rng) { Age = 90, Weight = 5 }); // Add ten old herbivores
            }
            var initialW = testCarn.Weight;
            testCarn.Params.F = 10.0; // Set "Hunger" to two herbivores
            testCarn.Feed(herbs);
            Assert.True(herbs.Where(i => !i.IsAlive).Count() > 0); // Check that some herbvores have been killed
        }


        [Fact]
        public void CarnivoreFeedingInceaseWeightTest()
        {
            var rng = new Random();
            var testCarn = new Carnivore(rng) { Age = 5, Weight = 60 }; // Healthy Carnivore
            testCarn.Params.DeltaPhiMax = 0;
            var herbs = new List<Herbivore>();
            for (int i = 0; i < 100; i++)
            {
                herbs.Add(new Herbivore(rng) { Age = 50, Weight = 5 }); // Add ten old herbivores
            }
            var initialW = testCarn.Weight;
            testCarn.Params.F = 10.0; // Set "Hunger" to two herbivores
            testCarn.Feed(herbs);
            Assert.True(initialW < testCarn.Weight);
        }


        [Fact]
        public void CarnivoreFitnessUpdateTest()
        {
            var rng = new Random();
            var testCarn = new Carnivore(rng) { Weight = 30, Age = 5 };
            testCarn.Params.DeltaPhiMax = 0;
            double initialFitness = testCarn.Fitness;
            var testHerb = new Herbivore(rng) { Weight = 2, Age = 70 }; // Low fitness herbivore.
            var kills = testCarn.Feed(new List<Herbivore>() { testHerb });
            Assert.True(testCarn.Fitness > testHerb.Fitness);
            Assert.True(kills == 1);
            Assert.True(initialFitness < testCarn.Fitness);
        }

        /*
        [Theory]
        [InlineData(5)]
        [InlineData(1)]
        [InlineData(2)]
        public void CarnivoreDoesntOvereatTest(int animalsToKill)
        {
            
        }
        */

    }
}
