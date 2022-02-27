using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Biosim.Animals;
using Biosim.Parameters;

namespace BiosimTests
{
    public class TestRodent
    {

        [Fact]
        public void RodentFeedingTest()
        {
            /*Description*/
            var rng = new Random();
            var pos = new Position(1, 2);
            double food = 100;
           
            var rodent = new Rodent(rng, pos);
            var F = rodent.Params.F;
            food = rodent.Feed(food);
            Assert.True(food < 100);
            Assert.Equal(100 - F, food);
        }

        [Fact]
        public void RodentBreedingTest()
        {
            /*Description*/
            var rng = new Random();
            var pos = new Position(1, 3);
            var rodent = new Rodent(rng, pos);
            Assert.False(rodent.GivenBirth);
            rodent.Age = 3;
            rodent.Weight = 10;
            rodent.Params.Gamma = 10;
            Assert.Equal(10, rodent.Params.Gamma);
            var offspring = rodent.Birth(100)[0];
            Assert.NotNull(offspring);
            Assert.Equal("Rodent", offspring.GetType().Name);
        }


    }
}
