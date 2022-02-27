using Biosim.Animals;
using Biosim.Land;
using Biosim.Parameters;
using System;
using System.Collections.Generic;
using Biosim.Tools;

namespace Biosim.Simulation
{
    internal interface ISimulation
    {
        string TemplateString { get; set; }
        bool NoMigration { get; set; }
        List<List<IEnviroment>> Land { get; set; } // Nested List
        Position DefaultDim { get; set; }
        int YearsToSimulate { get; set; }
        Random Rng { get; set; }
        LogWriter Logger { get; set; }
        Position Dimentions { get; set; }
        int TotalDeadHerbivores { get; set; }
        int TotalDeadCarnivores { get; set; }
        int DeadHerbivoresThisYear { get; set; }
        int DeadCarnivoresThisYear { get; set; }
        int HerbivoresBornThisYear { get; }
        int CarnivoresBornThisYear { get; }
        int TotalHerbivoresCreated { get; set; }
        int TotalCarnivoresCreated { get; set; }
        double AverageHerbivoreFitness { get; }
        double AverageCarnivoreFitness { get; }
        double AverageHerbivoreAge { get; }
        double AverageCarnivoreAge { get; }
        double AverageHerbivoreWeight { get; }
        double AverageCarnivoreWeight { get; }
        double PeakHerbiovreFitness { get; }
        double PeakCarnivoreFitness { get; }
        double PeakHerbivoreWeight { get; }
        double PeakCarnivoreWeight { get; }
        int CurrentYear { get; set; }
        int LiveHerbivores { get; }
        int LiveCarnivores { get; }
        bool Follow { get; set; }
        List<IAnimal> TrackedAnimals { get; set; }
        List<int> AnimalsToTrack { get; set; }
        ScriptInterpreter ScriptParser { get; set; }
        Dictionary<int, CommandData> Commands {get;set;}


        void ExecuteCommand(CommandData command);
        Position Build();
        void Migrate(IEnviroment cell);
        void AddAnimals(List<Animal> animals, Position cellPosition);
        void AddHerbivore(int age, double w, Position cellPosition, IAnimalParams par);
        void AddCarnivore(int age, double w, Position cellPosition, IAnimalParams par);
        void AddHerbivore(Position cellPosition, IAnimalParams par);
        void AddCarnivore(Position cellPosition, IAnimalParams par);
        string GetCellInformation(IEnviroment cell);
        IEnviroment GetCell(Position pos);
        IEnviroment GetCell(int x, int y);
        void AddTrackedAnimals(List<int> ids); // Adds the animals with the given IDs to tracking. (sets animal.Tracked)
        void TrackAnimals();
        void GetTrackedAnimals(); // Fetches all tracked animals in the simulation and adds them to the local list of trakced animals.
        void Simulate();
        void OneCellYearFirstHalf(IEnviroment cell);
        void OneCellYearSecondHalf(IEnviroment cell);
        void OneYear(); // Runs the simulation for one year and returns a string of data
        void LoadCustomOnCellParameters(Position cellPos, IAnimalParams parameters); // Parameters for all animals of a type in cell
        bool LoadCustomParametersOnAnimal(IAnimal animal, IAnimalParams parameters);
        List<Position> GetSurroundingCells(Position cellPos);
        void MoveAnimals(); // Tests must ensure that the animal is actually moved, not copied or replaced with age 0 animal
        void Plot();
        void ChangeCellParameters(Position cellPos, EnvParams newParams);
        void SaveCSV();
        void ManualRemoveDead();
        
    }
}