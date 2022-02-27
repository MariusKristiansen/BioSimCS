using Biosim.Animals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biosim.Parameters
{
    public static class DefaultParameters
    {
        //Hard coded default parameters for testing
        public static Random rng = new Random();
        public static int Herbivores = 10;
        public static int Carnivores = 5;
        public static string LargeIsland = "OOOOOOOOOOOOOOOOOOOO\nOSSSSSSJJJMMJJJJJJOO\nOSSSSSSJJJMMJJJJJJOO\nOSSSSSSJJJMMMJJJJJJO\nOOSSSSSSJJJMMJJJJJJO\nOOSSSSSSJJMMMJJJJJJO\nOOOOOOOOSMMMMJJJJJOO\nOSSSSSJJJJMMJJJJJOOO\nOSSSSSSSSSMMJJJOOOOO\nOSSSSDDDDDJJJJJOOOOO\nOSSSSDDDDDDJJJJOOOOO\nOSSSDDDDDDJJJJOOOOOO\nOOSSDDDDDMMMJOOOOOOO\nOOSDDDDDDMMSJJJJJOOO\nOSSDDDDDDDSSJJOOOOOO\nOSSSDDDDDDSSJOOOOOOO\nOOSSSSSDDDSJOOOOOOOO\nOOOSSSSJJJJJJOOOOOOO\nOOOSSSSSOOOOOOOOOOOO\nOOOOOOOOOOOOOOOOOOOO";
        public static string DefaultIsland = "OOOOOOOOOO\nOSSDDDSSMO\nOSJDDDSSMO\nOSJJJJJMMO\nOSSJJJMMMO\nOSSSJJJMMO\nOOSSJJJMMO\nOOSJJJJJMO\nOSJJJJJJMO\nOOOOOOOOOO";
        public static Position[] DefaultHerbPositions => Enumerable.Range(0, DefaultParameters.Herbivores).Select(i => new Position { x = 5, y = 5 }).ToArray();
        public static Position[] DefaultCarnPositions => Enumerable.Range(0, DefaultParameters.Carnivores).Select(i => new Position { x = 6, y = 5 }).ToArray();
        public static List<Herbivore> HerbivorePopulation => Enumerable.Range(0, DefaultParameters.Herbivores).Select(i => new Herbivore(rng) { Pos = new Position { x = 5, y = 5 } }).ToList();
        public static List<Carnivore> CarnivorePopulation => Enumerable.Range(0, DefaultParameters.Carnivores).Select(i => new Carnivore(rng) { Pos = new Position { x = 5, y = 5 } }).ToList();
    }
}
