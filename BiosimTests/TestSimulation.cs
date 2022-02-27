using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Biosim.Parameters;
using Biosim.Simulation;
using Biosim.Tools;


namespace BiosimTests
{
    public static class testData
    {
        public static string simpleTestTemplate = "JJJJ\nJSSJ\nJSSJ\nJJJJ";
        public static Position TestJunglePosition = new Position(0, 1);
        public static Position TestSavannahPosition = new Position(1, 1);
    }

    [Collection("sync")]
    public class TestSimulation
    {
        

        [Fact]
        public void AnimalTrackingAddedTest()
        {
            var sim = new Sim(10000, testData.simpleTestTemplate);
            var middle = new Position(1, 1);
            sim.AddHerbivore(middle);
            sim.AddHerbivore(middle);
            sim.AddHerbivore(middle);
            sim.AddHerbivore(middle);
            sim.GetCell(middle).Herbivores.ForEach(i => i.Tracked = true);
            sim.GetTrackedAnimals();
            Assert.Equal(4, sim.TrackedAnimals.Count); 
        }


        [Fact]
        public void AddingAnimalsToCellTest()
        {
            /*Description*/
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var middle = new Position(1, 1);
            var cell = sim.GetCell(middle);
            Assert.Equal(0, cell.NumberOfHerbivores);
            Assert.Equal(0, cell.NumberOfCarnivores);
            sim.AddHerbivore(middle);
            sim.AddCarnivore(middle);
            sim.AddCarnivore(middle);
            Assert.Equal(1, cell.NumberOfHerbivores);
            Assert.Equal(2, cell.NumberOfCarnivores);
        }



        [Fact]
        public void AddingCustomAnimalsToCellTest()
        {
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var middle = new Position(1, 1);
            var cell = sim.GetCell(middle);
            Assert.Equal(0, cell.NumberOfHerbivores);
            Assert.Equal(0, cell.NumberOfCarnivores);
            sim.AddHerbivore(42, 123.0, middle);
            sim.AddCarnivore(10, 99.2, middle);
            sim.AddCarnivore(10, 99.2, middle);
            Assert.Equal(1, cell.NumberOfHerbivores);
            Assert.Equal(2, cell.NumberOfCarnivores);
            Assert.Equal(2, cell.Carnivores.Where(i => i.Age == 10).Count());
            Assert.Equal(99.2+99.2, cell.Carnivores.Select(i => i.Weight).Sum());
            Assert.Equal(42, cell.HerbivoreAvgAge);
            Assert.Equal(10, cell.CarnivoreAvgAge);
        }



        [Fact]
        public void GetCellTest()
        {
            /*Description*/
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var savannahCell = sim.GetCell(testData.TestSavannahPosition);
            var jungleCell = sim.GetCell(testData.TestJunglePosition);
            Assert.Equal("Savannah", savannahCell.GetType().Name);
            Assert.Equal("Jungle", jungleCell.GetType().Name);
        }


    }

    [Collection("sync")]
    public class TestCommands
    {

        [Fact]
        public void FoodmaxCommandDoesNothingOnDesertCellTest()
        {
            var sim = new Sim(10000, "DDDD\nDDDD\nDDDD\nDDDD");
            var desertPos = new Position(1, 1);
            var fMaxCommand = new CommandData() {
                ActivationYear = 10,
                CellPosition = desertPos,
                Command = Command.CellFoodMax,
                Parameters = "1000"
            };
            sim.Build();
            sim.ExecuteCommand(fMaxCommand);
            var cell = sim.GetCell(desertPos);
            Assert.Throws<NullReferenceException>(() => cell.Params.Fmax);
        }


        [Fact]
        public void FoodmaxCommandSetsFmaxOnJungleSavannahTest()
        {
            var sim = new Sim(10000, "JJJJ\nSSSS\nJJJJ\nSSSS");
            var junglePos = new Position(0, 1);
            var savannahPos = new Position(1, 1);
            var fmaxCommandJungle = new CommandData() { 
                ActivationYear = 10,
                CellPosition = junglePos,
                Command = Command.CellFoodMax,
                Parameters = "1000" };
            var fmaxCommandSavannah = new CommandData()
            {
                ActivationYear = 10,
                CellPosition = savannahPos,
                Command = Command.CellFoodMax,
                Parameters = "1000"
            };

            sim.ExecuteCommand(fmaxCommandJungle);
            sim.ExecuteCommand(fmaxCommandSavannah);
            
            var resultJungle = sim.GetCell(junglePos).Params.Fmax;
            var resultSavannah = sim.GetCell(savannahPos).Params.Fmax;
            Assert.Equal(1000.0, resultJungle);
            Assert.Equal(1000.0, resultSavannah);
        }


        [Fact]
        public void GlobalKillHerboivoresCommandTest()
        {
            /*Description*/
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var command = new CommandData() {
                ActivationYear = 10,
                CellPosition = null,
                Command = Command.GlobalKillHerbivores,
                Global = true,
                Parameters = "5"
            };
            var cellPos1 = new Position(1, 1);
            var cellPos2 = new Position(2, 2);
            for (int i = 0; i < 10; i++)
            {
                sim.AddHerbivore(cellPos1);
                sim.AddCarnivore(cellPos2);
            }
            sim.ExecuteCommand(command);
            Assert.Equal(10, sim.LiveCarnivores);
            Assert.Equal(5, sim.LiveHerbivores);
        }

        [Fact]
        public void GlobalKillCarnivoresCommandTest()
        {
            /*Description*/
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var command = new CommandData()
            {
                ActivationYear = 10,
                CellPosition = null,
                Command = Command.GlobalKillCarnivores,
                Global = true,
                Parameters = "5"
            };
            var cellPos1 = new Position(1, 1);
            var cellPos2 = new Position(2, 2);
            for (int i = 0; i < 10; i++)
            {
                sim.AddHerbivore(cellPos1);
                sim.AddCarnivore(cellPos2);
            }
            sim.ExecuteCommand(command);
            Assert.Equal(5, sim.LiveCarnivores);
            Assert.Equal(10, sim.LiveHerbivores);
        }


        [Fact]
        public void PassableCommandTest()
        {
            /*Description*/
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var cell = sim.GetCell(testData.TestJunglePosition);
            var command = new CommandData() { 
                Command = Command.Passable,
                Parameters = "false",
                ActivationYear = 10,
                CellPosition = testData.TestJunglePosition
                };
            Assert.True(cell.Passable);
            sim.ExecuteCommand(command);
            Assert.False(cell.Passable);
        }


        [Fact]
        public void KillAllTest()
        {
            /*Description*/
            var command = new CommandData() { 
                ActivationYear=10,
                Command = Command.KillAll,
                CellPosition = testData.TestJunglePosition,
                Parameters = ""
            };
            var sim = new Sim(1000, testData.simpleTestTemplate);
            for (int i = 0; i < 100; i++)
            {
                sim.AddHerbivore(testData.TestJunglePosition);
                sim.AddCarnivore(testData.TestJunglePosition);
            }
            Assert.Equal(100, sim.LiveHerbivores);
            Assert.Equal(100, sim.LiveCarnivores);
            sim.ExecuteCommand(command);
            Assert.Equal(0, sim.LiveHerbivores);
            Assert.Equal(0, sim.LiveCarnivores);
        }


        [Fact]
        public void KillAllHerbivoresTest()
        {
            /*Description*/
            var command = new CommandData()
            {
                ActivationYear = 10,
                Command = Command.KillAllHerbivores,
                CellPosition = testData.TestJunglePosition,
                Parameters = ""
            };
            var sim = new Sim(1000, testData.simpleTestTemplate);
            for (int i = 0; i < 100; i++)
            {
                sim.AddHerbivore(testData.TestJunglePosition);
                sim.AddCarnivore(testData.TestJunglePosition);
            }
            Assert.Equal(100, sim.LiveHerbivores);
            Assert.Equal(100, sim.LiveCarnivores);
            sim.ExecuteCommand(command);
            Assert.Equal(0, sim.LiveHerbivores);
            Assert.Equal(100, sim.LiveCarnivores);
        }


        [Fact]
        public void KillAllCarnivoresTest()
        {
            /*Description*/
            var command = new CommandData()
            {
                ActivationYear = 10,
                Command = Command.KillAllCarnivores,
                CellPosition = testData.TestJunglePosition,
                Parameters = ""
            };
            var sim = new Sim(1000, testData.simpleTestTemplate);
            for (int i = 0; i < 100; i++)
            {
                sim.AddHerbivore(testData.TestJunglePosition);
                sim.AddCarnivore(testData.TestJunglePosition);
            }
            Assert.Equal(100, sim.LiveHerbivores);
            Assert.Equal(100, sim.LiveCarnivores);
            sim.ExecuteCommand(command);
            Assert.Equal(100, sim.LiveHerbivores);
            Assert.Equal(0, sim.LiveCarnivores);
        }


        [Fact]
        public void GlobalKillAllTest()
        {
            /*Description*/
            var command = new CommandData()
            {
                ActivationYear = 10,
                Command = Command.GlobalKillAll,
                CellPosition = null,
                Global = true,
                Parameters = ""
            };
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var positions = new List<Position> {
                new Position(1, 1),
                new Position(0, 0),
                new Position(2, 2),
                new Position(3, 3)
            };
            foreach (var pos in positions)
            {
                for (int i = 0; i < 10; i++)
                {
                    sim.AddHerbivore(pos);
                    sim.AddCarnivore(pos);
                }
            }
            Assert.Equal(40, sim.LiveHerbivores);
            Assert.Equal(40, sim.LiveCarnivores);
            sim.ExecuteCommand(command);
            Assert.Equal(0, sim.LiveCarnivores);
            Assert.Equal(0, sim.LiveHerbivores);
        }


        [Fact]
        public void GlobalKillAllHerbivoresTest()
        {
            /*Description*/
            var command = new CommandData()
            {
                ActivationYear = 10,
                Command = Command.GlobalKillAllHerbivores,
                CellPosition = null,
                Parameters = "",
                Global = true
            };
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var positions = new List<Position> {
                new Position(1, 1),
                new Position(0, 0),
                new Position(2, 2),
                new Position(3, 3)
            };
            foreach (var pos in positions)
            {
                for (int i = 0; i < 10; i++)
                {
                    sim.AddHerbivore(pos);
                    sim.AddCarnivore(pos);
                }
            }
            Assert.Equal(40, sim.LiveHerbivores);
            Assert.Equal(40, sim.LiveCarnivores);
            sim.ExecuteCommand(command);
            Assert.Equal(40, sim.LiveCarnivores);
            Assert.Equal(0, sim.LiveHerbivores);
        }


        [Fact]
        public void GlobalKillAllCarnivoresTest()
        {
            /*Description*/
            var command = new CommandData()
            {
                ActivationYear = 10,
                Command = Command.GlobalKillAllCarnivores,
                CellPosition = null,
                Parameters = "",
                Global = true
            };
            var sim = new Sim(1000, testData.simpleTestTemplate);
            var positions = new List<Position> {
                new Position(1, 1),
                new Position(0, 0),
                new Position(2, 2),
                new Position(3, 3)
            };
            foreach (var pos in positions)
            {
                for (int i = 0; i < 10; i++)
                {
                    sim.AddHerbivore(pos);
                    sim.AddCarnivore(pos);
                }
            }
            Assert.Equal(40, sim.LiveHerbivores);
            Assert.Equal(40, sim.LiveCarnivores);
            sim.ExecuteCommand(command);
            Assert.Equal(0, sim.LiveCarnivores);
            Assert.Equal(40, sim.LiveHerbivores);
        }


    }
}
