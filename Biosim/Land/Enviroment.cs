using Biosim.Animals;
using Biosim.Parameters;
using Biosim.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using Biosim.Tools;

namespace Biosim.Land
{
    public class Enviroment : IEnviroment
    {

        // Fields

        // Properties
        public Random Rng { get; set; }
        public bool Passable { get; set; } = false;
        public Position Pos { get; set; }
        public List<Herbivore> Herbivores { get; set; }
        public List<Carnivore> Carnivores { get; set; }
        public double Food { get; set; }
        public int NumberOfHerbivores => Herbivores.Count();
        public int NumberOfCarnivores => Carnivores.Count();
        public int TotalIndividuals => NumberOfCarnivores + NumberOfHerbivores;
        public double CarnivoreFood => Herbivores.Select(i => i.Weight).Sum();
        public int KilledByCarnivores { get; set; }
        public int DeadCarnivores { get; set; }
        public int DeadHerbivores { get; set; }
        public int NewHerbivores { get; set; }
        public int NewCarnivores { get; set; }
        public double HerbivoreAvgFitness => (Herbivores.Count() > 0) ? Herbivores.Select(i => i.Fitness).Average() : 0;
        public double CarnivoreAvgFitness => (Carnivores.Count() > 0) ? Carnivores.Select(i => i.Fitness).Average() : 0;
        public EnvParams Params { get; set; }
        public double HerbivoreAvgWeight => (Herbivores.Count() > 0) ? Herbivores.Select(i => i.Weight).Average() : 0;
        public double CarnivoreAvgWeight => (Carnivores.Count() > 0) ? Carnivores.Select(i => i.Weight).Average() : 0;
        public double HerbivoreAvgAge => (Herbivores.Count() > 0) ? Herbivores.Select(i => i.Age).Average() : 0;
        public double CarnivoreAvgAge => (Carnivores.Count() > 0) ? Carnivores.Select(i => i.Age).Average() : 0;
        public int TotalCarnivoreLives { get; set; } = 0;
        public int TotalHerbivoreLives { get; set; } = 0;
        public double PeakHerbivoreFitness => (Herbivores.Count() > 0) ? Herbivores.Select(i => i.Fitness).Max() : 0;
        public double PeakCarnivoreFitness => (Carnivores.Count() > 0) ? Carnivores.Select(i => i.Fitness).Max() : 0;
        public double PeakHerbivoreAge => (Herbivores.Count() > 0) ? Herbivores.Select(i => i.Age).Max() : 0;
        public double PeakCarnivoreAge => (Carnivores.Count() > 0) ? Carnivores.Select(i => i.Age).Max() : 0;
        public double PeakHerbivoreWeight => (Herbivores.Count() > 0) ? Herbivores.Select(i => i.Weight).Max() : 0;
        public double PeakCarnivoreWeight => (Carnivores.Count() > 0) ? Carnivores.Select(i => i.Weight).Max() : 0;
        public DatabaseHandler DBhandler { get; set; }
        public List<Rodent> Rodents { get; set; }

        // Constructor & Overloads

        public Enviroment(Position pos, Random rng, DatabaseHandler dbhandler, List<Herbivore> initialHerbivores = null, List<Carnivore> initialCarnivores = null, List<Rodent> intitalRodents = null )
        {
            DBhandler = dbhandler;
            Rng = rng;
            Pos = pos;
            Herbivores = initialHerbivores is null ? new List<Herbivore>() : initialHerbivores;
            Carnivores = initialCarnivores is null ? new List<Carnivore>() : initialCarnivores;
            TotalHerbivoreLives = Herbivores.Count();
            TotalCarnivoreLives = Carnivores.Count();
        }

        public void HerbivoreFeedingCycle()
        {
            Herbivores = Herbivores.OrderBy(i => i.Fitness).ToList();
            foreach (var herb in Herbivores)
            {
                Food = herb.Feed(Food);
            }
        }

        public void CarnivoreFeedingCycle()
        {
            Carnivores = Carnivores.OrderBy(i => i.Fitness).ToList();
            var kills = 0;
            foreach (var carn in Carnivores)
            {
                kills += carn.Feed(Herbivores);
            }
            KilledByCarnivores = kills;
            RemoveDeadIndividuals();
        }

        public void DeathCycle()
        {
            Carnivores.ForEach(i => i.Death());
            Herbivores.ForEach(i => i.Death());
        }

        public void BirthCycle()
        {
            List<Herbivore> newbornHerbivores = new List<Herbivore>();
            List<Carnivore> newbornCarnivores = new List<Carnivore>();
            Herbivores = Herbivores.OrderBy(i => i.Fitness).ToList();
            //string thisCouldBeChanged;
            var numHerb = Herbivores.Count(); // Can also add Herbivores.Select(i => i.Age > 0).Count() to be sure all animals are not newborn
            foreach (var herb in Herbivores)
            {
                var result = herb.Birth(numHerb);
                if (!(result is null))
                {
                    newbornHerbivores.Add((Herbivore)result);
                }
            }
            foreach (var child in newbornHerbivores)
            {
                Herbivores.Add(child);
            }
            NewHerbivores = newbornHerbivores.Count();
            TotalHerbivoreLives += newbornHerbivores.Count();

            Carnivores = Carnivores.OrderBy(i => i.Fitness).ToList();
            var numCarn = Carnivores.Count();
            foreach (var carn in Carnivores)
            {
                var result = carn.Birth(numCarn);
                if (!(result is null))
                {
                    newbornCarnivores.Add((Carnivore)result);
                }
            }
            foreach (var child in newbornCarnivores)
            {
                Carnivores.Add(child);
            }
            NewCarnivores = newbornCarnivores.Count();
            TotalCarnivoreLives += newbornCarnivores.Count();
        }

        public void AgeCycle()
        {
            Herbivores.ForEach(i => i.GrowOlder());
            Carnivores.ForEach(i => i.GrowOlder());
        }

        public void WeightLossCycle()
        {
            Herbivores.ForEach(i => i.UpdateWeight());
            Carnivores.ForEach(i => i.UpdateWeight());
        }

        public void RemoveDeadIndividuals()
        {
            DeadCarnivores += Carnivores.Where(i => !i.IsAlive).Count();
            DeadHerbivores += Herbivores.Where(i => !i.IsAlive).Count();
            List<Herbivore> survivingHerbivores = new List<Herbivore>();
            foreach (var herb in Herbivores)
            {
                if (herb.IsAlive)
                {
                    survivingHerbivores.Add(herb);
                } 
                else
                {
                    // Add the dead individual to corpse database
                    // DBhandler.AddDeadAnimalToDatabase(herb, "DeadHerbivores");
                }
            }

            List<Carnivore> survivingCarnivores = new List<Carnivore>();
            foreach (var carn in Carnivores)
            {
                if (carn.IsAlive)
                {
                    survivingCarnivores.Add(carn);
                }
                else
                {
                    // Add the dead individual to corpse database
                    // DBhandler.AddDeadAnimalToDatabase(carn, "DeadCarnivores");
                }
            }
            Herbivores = survivingHerbivores;
            Carnivores = survivingCarnivores;
        }

        public void ResetCurrentYearParameters()
        {
            DeadCarnivores = 0;
            DeadHerbivores = 0;
            NewCarnivores = 0;
            NewHerbivores = 0;

        }

        public int ResetGivenBirthParameter()
        {
            var births = Carnivores.Where(i => i.GivenBirth).Count() + Herbivores.Where(i => i.GivenBirth).Count();
            Carnivores.ForEach(i => i.GivenBirth = false);
            Herbivores.ForEach(i => i.GivenBirth = false);
            return births;
        }

        public virtual void GrowFood()
        {

        }

        public int ResetMigrationParameter()
        {
            var migrations = Carnivores.Where(i => i.Migrated).Count() + Herbivores.Where(i => i.Migrated).Count();
            Carnivores.ForEach(i => i.Migrated = false);
            Herbivores.ForEach(i => i.Migrated = false);
            return migrations;
        }

        public void DEBUG_OneCycle()
        {// ResetMigration must be done by Simulation Object
            GrowFood();
            HerbivoreFeedingCycle();
            CarnivoreFeedingCycle();
            BirthCycle();
            //Migration
            AgeCycle();
            WeightLossCycle();
            DeathCycle();
            RemoveDeadIndividuals();
            ResetGivenBirthParameter();
        }

        public void OverloadParameters(IAnimal animal, IAnimalParams parameters)
        {

            int writemorecodehere;

            animal.Params = parameters;
        }

        public void OverloadAllHerbivores(HerbivoreParams parameters)
        {
            // Implement parameter cloning!!!
            Herbivores.ForEach(i => i.Params = parameters);
        }

        public void OverloadAllCarnivores(CarnivoreParams parameters)
        {
            // Implement parameter cloning!!!
            Carnivores.ForEach(i => i.Params = parameters);
        }

        public void OverloadParameters(int index, string type, IAnimalParams parameters)
        {
            if (type.ToUpper() == "HERBIVORE" || type.ToUpper() == "H")
            {
                if (parameters.GetType().Name != "HerbivoreParams") throw new Exception($"Wrong Parameter type ({parameters.GetType()}) for Herbivore");
                Herbivores[index].Params = parameters;
                return;
            }
            if (type.ToUpper() == "CARNIVORE" || type.ToUpper() == "C")
            {
                if (parameters.GetType().Name != "CarnivoreParams") throw new Exception($"Wrong Parameter type ({parameters.GetType()}) for Carnivore");
                Carnivores[index].Params = parameters;
                return;
            }
            throw new Exception($"Unable to overload parameters. \nInput was: INDEX - {index} TYPE - {type} PARAMETERS - {parameters}");
        }

        public void LogData(LogWriter logger)
        {
            throw new NotImplementedException();
        }


        // Methods
    }

    public class Desert : Enviroment
    {

        public Desert(Position pos, Random rng, DatabaseHandler databaseHandler, List<Herbivore> initialHerbivores = null, List<Carnivore> initialCarnivores = null, List<Rodent> initialRodents = null) : base(pos, rng, databaseHandler, initialHerbivores, initialCarnivores, initialRodents)
        {
            Passable = true;
        }

    }

    public class Ocean : Enviroment
    {
        public Ocean(Position pos, Random rng, DatabaseHandler databaseHandler) : base(pos, rng, databaseHandler)
        {

        }

    }

    public class Mountain : Enviroment
    {

        public Mountain(Position pos, Random rng, DatabaseHandler databaseHandler) : base(pos, rng, databaseHandler)
        {

        }

    }

    public class Savannah : Enviroment
    {

        public Savannah(Position pos, Random rng, DatabaseHandler databaseHandler, List<Herbivore> initialHerbivores = null, List<Carnivore> initialCarnivores = null, List<Rodent> initialRodents = null) : base(pos, rng, databaseHandler, initialHerbivores, initialCarnivores, initialRodents)
        {
            Passable = true;
            Params = new SavannahParams();
        }

        public override void GrowFood()
        {
            Food += Params.Alpha * (Params.Fmax - Food);
        }
    }

    public class Jungle : Enviroment
    {
        public Jungle(Position pos, Random rng, DatabaseHandler databaseHandler, List<Herbivore> initialHerbivores = null, List<Carnivore> initialCarnivores = null, List<Rodent> initialRodents = null) : base(pos, rng, databaseHandler, initialHerbivores, initialCarnivores, initialRodents)
        {
            Passable = true;
            Params = new JungleParams();
        }

        public override void GrowFood()
        {
            Food = Params.Fmax;
        }
    }

}