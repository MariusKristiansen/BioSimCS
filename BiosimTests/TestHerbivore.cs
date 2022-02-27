using Biosim.Animals;
using Biosim.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BiosimTests
{
    public class TestHerbivore
    {
        public static Position testCoor = new Position { x = 1, y = 1 };

        [Fact]
        public void HerbivoreFeedReducesAmountTest()
        {
            double initialAmount = 100.0;
            Herbivore testHerb = new Herbivore(new Random());
            double remainder = testHerb.Feed(initialAmount);
            Assert.NotEqual(initialAmount, remainder);
            Assert.Equal(initialAmount, remainder + testHerb.Params.F);
        }

        [Fact]
        public void HerbivoreFeedIncreasesWeightTest()
        {
            Herbivore testHerb = new Herbivore(new Random());
            double initialWeight = testHerb.Weight;
            testHerb.Feed(100.0);
            double endWeight = testHerb.Weight;
            Assert.True(initialWeight < endWeight);
        }


        [Fact]
        public void HerbivoreFeedingIncreasesFitnessTest()
        {
            /*food => Phat*/
            var testHerbivore = new Herbivore(new Random(), testCoor);
            var food = 100;
            testHerbivore.Params.F = 25;
            var initialFitness = testHerbivore.Fitness;
            var remainder = testHerbivore.Feed(food);
            var endFitness = testHerbivore.Fitness;
        }


        [Fact]
        public void HerbivoreFeedWhenLessThanFTest()
        {
            Herbivore testHerb = new Herbivore(new Random());
            double F = testHerb.Params.F;


            double available = F - 5.0;
            double remainder = testHerb.Feed(available);
            Assert.Equal(0, remainder);
        }

        [Fact]
        public void HerbivoreFeedZeroAvailableTest()
        {
            Herbivore testHerb = new Herbivore(new Random());
            double initialWeight = testHerb.Weight;
            double initalAmount = 0.0;
            double remainder = testHerb.Feed(initalAmount);
            double endWeight = testHerb.Weight;
            Assert.Equal(0, remainder);
            Assert.Equal(initialWeight, endWeight);
        }

        [Fact] //fct shortcut
        public void HerbivoreGiveBirthTest()
        {
            /*Set Weight and param gamme sufficiently high such that birth will succeed
             Check if Given birth is set to true
             Check that the offspring is of the right type
            Assert that the weight of mother reduces after birth
            */
            var testHerb = new Herbivore(new Random()) { Weight = 150 };
            Assert.False(testHerb.GivenBirth);
            testHerb.Params.Gamma = 10;
            var result = testHerb.Birth(10);
            Assert.NotNull(result);
            Assert.True(result.GetType().Name == "Herbivore");
            Assert.True(testHerb.GivenBirth);
            Assert.True(testHerb.Weight < 150);
            Assert.True(result != testHerb);
        }


        /* NEEDS REWRITE
        [Fact]
        public void HerbivoreMoveNotNullTest()
        {
            // Set Mu parameter so that berbiovre must move. 
            Directions[] allDir = new Directions[] { Directions.Up, Directions.Left, Directions.Right, Directions.Down};
            Directions[] upDown = new Directions[] { Directions.Up, Directions.Down };
            Directions[] leftRight = new Directions[] { Directions.Left, Directions.Right };
            Directions[] onlyOne = new Directions[] { Directions.Left };
            Herbivore testHerb = new Herbivore(new Random()) { Weight=150, Age=5 };
            testHerb.Params.Mu = 100; // Effective value of Fitness will be ~0.99, setting Mu to a stupid high value will ensure that the rng pull always succeeds.
            Directions? result = testHerb.Migrate(allDir);
            Directions? left = testHerb.Migrate(onlyOne);
            Assert.NotNull(result);
            Assert.NotNull(left);
            Assert.Equal(Directions.Left, left);
        }
        */
        [Fact]
        public void HerbivoreSetWeightTest()
        {
            Herbivore testHerb = new Herbivore(new Random()) { Weight = 10 };
            Assert.Equal(10, testHerb.Weight);
        }


        [Fact]
        public void HerbivoreGrowsOlderTest()
        {
            /*Description*/
            var testHerb = new Herbivore(new Random());
            Assert.Equal(0, testHerb.Age);
            testHerb.GrowOlder();
            Assert.Equal(1, testHerb.Age);
        }


        [Fact]
        public void HerbivoreDiesTest()
        {
            /*Check to confirm death when fitness is low*/
            var rng = new Random();
            List<bool> results = new List<bool>();
            for (int i = 0; i < 10000; i++)
            {
                var testHerbivore = new Herbivore(rng) { Age = 60, Weight = 1 }; // Herbivore with low fitness
                testHerbivore.Params.Omega = 1; // Omega = 1 removes the preset amount of survivals
                testHerbivore.Death();
                results.Add(testHerbivore.IsAlive);
            }
            var dead = results.Where(i => !i).Count() / (double)results.Count() * 100.0;
            Assert.True(dead > 99); // More than 99% of all individuals die
        }


        [Fact]
        public void HerbivoreSurvivesTest()
        {
            var rng = new Random();
            List<bool> results = new List<bool>();
            for (int i = 0; i < 10000; i++)
            {
                var testHerbivore = new Herbivore(rng) { Age = 5, Weight = 100 }; // Herbivore with high fitness
                testHerbivore.Params.Omega = 1;
                testHerbivore.Death();
                results.Add(testHerbivore.IsAlive);
            }
            var dead = results.Where(i => !i).Count() / (double)results.Count() * 100.0;
            Assert.True(dead < 1); // Less than 1% of all individuals die
        }


        [Fact]
        public void HerbivoreLosesWeightTest()
        {
            /*Description*/
            var rng = new Random();
            var testHerb = new Herbivore(rng);
            var presetWeight = 100;
            testHerb.Weight = presetWeight;
            testHerb.UpdateWeight();
            var amountToLose = presetWeight * testHerb.Params.Eta;
            Assert.True(presetWeight > testHerb.Weight);
            Assert.True(presetWeight - testHerb.Weight == amountToLose);
        }


        [Fact]
        public void HerbivoreKilledTest()
        {
            var rng = new Random();
            var testHerb = new Herbivore(rng);
            Assert.True(testHerb.IsAlive);
            testHerb.Kill();
            Assert.False(testHerb.IsAlive);
        }


        [Fact]
        public void HerbivoreAlreadyDeadTest()
        {
            // Check if IsAlive is set properly, or if it flips
            var rng = new Random();
            var testHerb = new Herbivore(rng);
            testHerb.IsAlive = false;
            testHerb.Kill();
            Assert.False(testHerb.IsAlive);

        }


        [Fact]
        public void FeedingUpdatesFitnessTest()
        {
            var rng = new Random();
            var testHerb = new Herbivore(rng) { Weight = 5, Age = 50 };
            var initialF = testHerb.Fitness;
            var food = 100.0;
            testHerb.Feed(food);
            var endF = testHerb.Fitness;
            Assert.True(initialF < endF);
        }


        [Fact]
        public void HerbivoreParmeterInheretanceTest()
        {
            /*Assert that the offspring of a herbivore inherits a 
             * parameter object with type HerbivoreParams and has the same values as the mother
               while also not being the same actual object (reference/pointer) */
            var rng = new Random();
            var testHerb = new Herbivore(rng) { Weight = 50, Age = 5 }; // High Fitness
            var defaultParams = testHerb.Params;
            testHerb.Params.Gamma = 10;
            var offspring = testHerb.Birth(20);
            Assert.NotEqual(offspring.Params, defaultParams);
        }

    }
}
