using Biosim.Animals;
using Biosim.Land;
using Biosim.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BiosimTests
{
    public class TestEnviroment
    {
        public static Random rng = new Random();
        public static List<Carnivore> testCarns = Enumerable.Range(0, 100).Select(i => new Carnivore(rng)).ToList();
        public static List<Herbivore> testHerbs = Enumerable.Range(0, 100).Select(i => new Herbivore(rng)).ToList();

        [Fact]
        public void MigrationResetTest()
        {
            var env = new Jungle(new Position() { x = 5, y = 5 }, rng, new Biosim.Tools.DatabaseHandler());
            env.Herbivores = Enumerable.Range(0, 10).Select(i => new Herbivore(rng)).ToList();
            env.Herbivores[0].Migrated = true;
            env.Herbivores[4].Migrated = true;
            env.Herbivores[6].Migrated = true;
            int migrated = env.Herbivores.Where(i => i.Migrated).Count();
            env.ResetMigrationParameter();
            Assert.NotEqual(migrated, env.Herbivores.Where(i => i.Migrated).Count());
        }

        [Fact]
        public void ParametersAreUniqueObjectsTest()
        {
            bool good = true;
            foreach (var herbA in testHerbs)
            {
                var baseParams = herbA.Params;
                foreach (var herbB in testHerbs)
                {
                    if (herbA == herbB) continue;
                    if (baseParams == herbB.Params)
                    {
                        good = false; // Found two equal param references!!! ISSUE!

                    }
                }
            }

            foreach (var carnA in testHerbs)
            {
                var baseParams = carnA.Params;
                foreach (var carnB in testHerbs)
                {
                    if (carnA == carnB) continue;
                    if (baseParams == carnB.Params)
                    {
                        good = false; // Found two equal param references!!! ISSUE!

                    }
                }
            }
            Assert.True(good);
        }


        [Fact]
        public void CarnivoresKillHerbivoresTest()
        {
            var randomGen = new Random();
            var herbs = Enumerable.Range(0, 100).Select(i => new Herbivore(randomGen)).ToList();
            var carns = Enumerable.Range(0, 20).Select(i => new Carnivore(randomGen)).ToList();
            var cell = new Jungle(new Position { x = 0, y = 0 }, randomGen, new Biosim.Tools.DatabaseHandler(), herbs, carns);
            cell.CarnivoreFeedingCycle();
            Assert.True(cell.Herbivores.Count() < 100);
        }

    }
}
