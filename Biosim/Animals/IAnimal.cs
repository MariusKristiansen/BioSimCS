using Biosim.Parameters;
using System.Collections.Generic;
using Biosim.Models;

namespace Biosim.Animals
{
    public interface IAnimal
    {
        public int ID { get; set; } // MUST BE UNIQUE
        int Age { get; set; }
        Position Pos { get; set; }
        Position GoingToMoveTo { get; set; } // The cell the animal is going to move to under simulation.migration
        double Weight { get; set; }
        double Qplus { get; set; }
        double Qneg { get; set; }
        double Fitness { get; }
        IAnimalParams Params { get; set; }
        bool IsAlive { get; set; }
        bool Tracked { get; set; }
        bool InheritTracking { get; set; }
        bool GivenBirth { get; set; }
        bool Migrated { get; set; }
        int NumberOfBirths { get; set; }
        List<int> Parents { get; set; } // List of IDs of any "parents" of this animal. 

        //Directions Migrate(Directions[] dir);
        IAnimal Birth(int sameSpeciesInCell);
        void Death();
        void UpdateWeight(); //Lose weight
        void GrowOlder(); // Increment Age
        bool Kill();
        void UpdateParameters(IAnimalParams newParameters);
        void RandomParameterChange();
        void SelectivePressure();
        bool Track();
        bool Untrack();
        AnimalModel LogTrackedAnimal();
    }
}
