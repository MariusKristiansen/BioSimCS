using Xunit;
using Biosim.Animals;
using Biosim.Parameters;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BiosimTests
{
    public class TestAnimals
    {
        [Fact]
        public void TrackingVariableIsInheritedTest()
        {
            var rng = new Random();
            var pos = new Position { x = 10, y = 10 };
            var animals = new List<IAnimal>();
            animals.AddRange(Enumerable.Range(0, 10).Select(i => new Herbivore(rng, pos) { InheritTracking = true, Tracked = true, Weight = 50, Age = 5 }).ToList());
            animals.AddRange(Enumerable.Range(0, 10).Select(i => new Carnivore(rng, pos) { InheritTracking = true, Tracked = true, Weight = 50, Age = 5 }).ToList());
            animals.ForEach(i => i.Params.Gamma = 10); // Setting Gamma to guarantee birth
            var offspring = new List<IAnimal>();
            foreach (var animal in animals)
            {
                if (animal.Params.Gamma != 10) break;
                var result = animal.Birth(10);
                if (result is null) continue;
                offspring.Add(result);
            }

            Assert.True(offspring.Count > 0); // If Gamma is not properly set, no animals will be added
            Assert.True(offspring.Where(i => i.Tracked).Count() > 0);
            Assert.True(offspring.Where(i => i.Tracked).Count() == offspring.Count);
        }


        [Fact]
        public void ParentsAreAddedTest()
        {
            /*Description*/
            var rng = new Random();
            var parent1 = new Herbivore(rng) { Tracked = true, InheritTracking = true, Weight = 50, Age = 5 };
            //var parent2 = new Herbivore(rng) { Tracked = true, InheritTracking = true, Weight = 50, Age = 5 };
            parent1.Params.Gamma = 10;
            parent1.ID = 1;
            //parent2.ID = 2;
            var offspring1 = parent1.Birth(10);
            Assert.True(offspring1.Parents[0] == parent1.ID);
        }


        [Fact]
        public void EnsureParametersAreInheritedTest()
        {
            /*Description*/
            var rng = new Random();
            var animal = new Herbivore(rng) { Weight=50, Age=5 };
            animal.Params.F = 42; // Set F to a custom value that is easy to "track"
            animal.Params.Gamma = 10; // Gamma 10 will ensure birth
            var offspring = animal.Birth(10);
            Assert.Equal(42.0, offspring.Params.F);
        }


        [Fact]
        public void TrackingLogDataReturnsModelTest()
        {
            /*Description*/
            var rng = new Random();
            var animal = new Herbivore(rng);
            animal.Track();
            var result = animal.LogTrackedAnimal();
            Assert.False(result is null);
            Assert.True(result.GetType().Name == "HerbivoreModel");
            Assert.Equal(animal.ID, result.HerbivoreID);
            Assert.True(result.Weight == animal.Weight);
            Assert.True(result.NumberOfBirths == animal.NumberOfBirths);
            Assert.True(result.Age == animal.Age);
            Assert.True(result.Fitness == animal.Fitness);
            //Assert.True(result.Parents.Count == animal.Parents.Count);

        }


        [Fact]
        public void TrackingLogDataReturnsModelOfCorrectTypeTest()
        {
            var rng = new Random();
            var carnivore = new Carnivore(rng);
            var herbivore = new Herbivore(rng);
            carnivore.Track();
            herbivore.Track();
            var hModel = herbivore.LogTrackedAnimal();
            var cModel = carnivore.LogTrackedAnimal();
            Assert.Equal("HerbivoreModel", hModel.GetType().Name);
            Assert.Equal("CarnivoreModel", cModel.GetType().Name);
        }


        [Fact]
        public void TrackingLogDataReturnsNullWhenNotTrackedTest()
        {
            var rng = new Random();
            var animal = new Herbivore(rng);
            Assert.False(animal.Tracked);
            var result = animal.LogTrackedAnimal();
            Assert.Null(result);
        }




        [Fact]
        public void AnimalTrackMethodTest()
        {
            /*Description*/
            var rng = new Random();
            var animal = new Herbivore(rng);
            Assert.False(animal.Tracked);
            animal.Track();
            Assert.True(animal.Tracked);
        }


        [Fact]
        public void AnimalUntrackMethodTest()
        {
            var rng = new Random();
            var animal = new Carnivore(rng) { Tracked = true};
            Assert.True(animal.Tracked);
            animal.Untrack();
            Assert.False(animal.Tracked);
        }

    }
}
