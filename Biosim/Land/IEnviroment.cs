using Biosim.Animals;
using Biosim.Parameters;
using Biosim.Simulation;
using System.Collections.Generic;

namespace Biosim.Land
{
    public interface IEnviroment
    {
        List<Carnivore> Carnivores { get; set; }
        Position Pos { get; set; }
        double Food { get; set; }
        List<Herbivore> Herbivores { get; set; }
        List<Rodent> Rodents { get; set; }
        bool Passable { get; set; }
        int NumberOfHerbivores { get; }
        int NumberOfCarnivores { get; }
        int TotalIndividuals { get; }
        double CarnivoreFood { get; }
        int KilledByCarnivores { get; set; }
        int DeadCarnivores { get; set; }
        int DeadHerbivores { get; set; }
        int NewHerbivores { get; set; }
        int NewCarnivores { get; set; }
        double HerbivoreAvgFitness { get; }
        double CarnivoreAvgFitness { get; }
        double HerbivoreAvgWeight { get; }
        double CarnivoreAvgWeight { get; }
        double HerbivoreAvgAge { get; }
        double CarnivoreAvgAge { get; }
        int TotalCarnivoreLives { get; set; }
        int TotalHerbivoreLives { get; set; }
        double PeakHerbivoreFitness { get; }
        double PeakCarnivoreFitness { get; }
        double PeakHerbivoreAge { get; }
        double PeakCarnivoreAge { get; }
        double PeakHerbivoreWeight { get; }
        double PeakCarnivoreWeight { get; }
        EnvParams Params { get; set; }

        void LogData(LogWriter logger);
        void HerbivoreFeedingCycle();
        void CarnivoreFeedingCycle();
        void ResetCurrentYearParameters();
        void DeathCycle();
        void BirthCycle();
        void GrowFood();
        int ResetGivenBirthParameter(); //Returns the number of births in a given cycle
        int ResetMigrationParameter();
        void AgeCycle();
        void WeightLossCycle();
        void RemoveDeadIndividuals();
        void DEBUG_OneCycle();
        void OverloadParameters(IAnimal animal, IAnimalParams parameters);
        void OverloadParameters(int index, string type, IAnimalParams parameters);
        void OverloadAllHerbivores(HerbivoreParams parameters);
        void OverloadAllCarnivores(CarnivoreParams parameters);
    }
}