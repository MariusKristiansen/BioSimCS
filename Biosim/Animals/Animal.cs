using Biosim.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Biosim.Tools;
using Biosim.Simulation;
using Biosim.Models;
//using MathNet.Numerics;

namespace Biosim.Animals
{
    public class Animal : IAnimal
    {
        // Fields
        private double weight;
        private Position pos = new Position(); // Should replace with faster way of holding key-value pairs
        internal Random rng;
        // Properties
        public double Weight
        {
            get { return weight; }
            set { weight = (value < 0) ? 0 : value; }
        }

        public int Age { get; set; }
        public int ID { get; set; }
        public Position Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public double Fitness => Qneg * Qplus;
        public virtual double Qplus { get; set; }
        public virtual double Qneg { get; set; }
        public virtual IAnimalParams Params { get; set; }
        public bool IsAlive { get; set; } = true;
        public bool GivenBirth { get; set; } = false;
        public bool Migrated { get; set; } = false;
        public bool Tracked { get; set; } = false;
        public Position GoingToMoveTo { get; set; }
        public int NumberOfBirths { get; set; } = 0;
        public bool InheritTracking { get; set; } = false;
        public List<int> Parents { get; set; }

        // Constructors & Overloads

        public Animal(Random _rng, Position pos)
        {
            rng = _rng;
            ID = Sim.BaseID + 1;
            Sim.BaseID++;
            //ID = Program.IDbase + 1;
            //Program.IDbase++;
            Age = 0;
            Parents = new List<int>();
            Pos = pos is null ? new Position { x = 0, y = 0 } : pos;
        }

        public bool Track()
        {
            if (Tracked) return false;
            Tracked = true;
            return true;
        }

        public bool Untrack()
        {
            if (!Tracked) return false;
            Tracked = false;
            return true;
        }

        public virtual void Migrate(List<Position> dir)
        {
            // The animal gets a set of directions it is allowed to move to (based on the passable attribute passed in from the main controller (island)
            if (Fitness * Params.Mu > rng.NextDouble())
            {
                Migrated = true;
                GoingToMoveTo = dir[rng.Next(dir.Count())];
            }
            else
            {
                GoingToMoveTo = new Position { x = Pos.x, y = Pos.y };
            }
        }


        public virtual IAnimal Birth(int sameSpeciesInCell)
        {
            if (GivenBirth) return null;
            double probability;
            if (Weight < (Params.Zeta * (Params.BirthWeight + Params.BirthSigma)))
            {
                probability = 0.0;
            }
            else
            {
                probability = Params.Gamma * Fitness * (sameSpeciesInCell - 1);
                probability = (probability > 1) ? 1 : probability;
            }

            if (probability >= rng.NextDouble())
            {
                IAnimal newborn = this.GetType().Name == "Herbivore" ? new Herbivore(rng, this.Pos) : new Carnivore(rng, this.Pos);
                newborn.Params.OverloadParameters(Params.CopyParameters()); // Inherit params from parent
                
                if (InheritTracking)
                {
                    newborn.Tracked = true;
                    newborn.InheritTracking = InheritTracking;
                    newborn.Parents.AddRange(Parents);
                    newborn.Parents.Add(ID);
                }

                double bWeight = newborn.Weight;
                if (bWeight >= Weight || bWeight <= 0) return null;
                Weight -= Params.Xi * bWeight;
                NumberOfBirths++;
                GivenBirth = true;
                return newborn;
            }
            return null;
        }

        public void Death()
        {
            IsAlive = (rng.NextDouble() > Params.Omega * (1 - Fitness));
        }

        public void UpdateWeight()
        {
            Weight -= Weight * Params.Eta;
        }

        public void GrowOlder()
        {
            Age += 1;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}\nAlive: {(IsAlive ? "Alive" : "Dead")}\nAge: {Age}\nWeight: {Weight}\nFitness: {Fitness}\n{this.Pos}";
        }

        public bool Kill()
        {
            if (IsAlive)
            {
                IsAlive = false;
                return true;
            }
            return false;
        }

        public void UpdateParameters(IAnimalParams newParameters)
        {
            Params.OverloadParameters(newParameters);
        }

        public void RandomParameterChange()
        {
            // Randomly alter a single parameter.

            var parameter = Enum.GetValues(typeof(ParameterEnum)).Cast<ParameterEnum>().Max();
            Console.WriteLine(parameter);
        }

        public void SelectivePressure()
        {
            // Animal can update it's own parameters based on external data and randomness. 

        }

        public AnimalModel LogTrackedAnimal()
        {
            if (!Tracked) return null;
            AnimalModel animalModel;
            if (this.GetType().Name == "Herbivore")
            {
                animalModel = new HerbivoreModel {
                    HerbivoreID = this.ID
                };
            } else
            {
                animalModel = new CarnivoreModel {
                    CarnivoreID = this.ID
                };
            }
            animalModel.Age = this.Age;
            animalModel.Weight = this.Weight;
            animalModel.NumberOfBirths = this.NumberOfBirths;
            animalModel.Fitness = this.Fitness;
            //animalModel.Parents = this.Parents;

            return animalModel;
        }
    }

    public class Herbivore : Animal
    {

        public override double Qplus => 1 / (1 + Math.Exp(Params.PhiAge * (Age - Params.AHalf)));
        public override double Qneg => 1 / (1 + Math.Exp(-Params.PhiWeight * (Weight - Params.WHalf)));
        public Herbivore(Random rng, Position pos = null, IAnimalParams customParameters = null) : base(rng, pos)
        {
            Params = customParameters is null ? new HerbivoreParams() : customParameters;
            var norm = new MathNet.Numerics.Distributions.Normal(Params.BirthWeight, Params.BirthSigma);
            Weight = norm.Sample();
        }

        public virtual double Feed(double availableFood)
        {
            double willEat = (availableFood >= Params.F) ? Params.F : availableFood;
            double leftOver = availableFood - willEat;
            Weight += Params.Beta * willEat;
            return (leftOver < 0) ? 0 : leftOver;
        }
    }

    public class Rodent : Herbivore
    {
        public override double Qplus => 1 / (1 + Math.Exp(Params.PhiAge * (Age - Params.AHalf)));
        public override double Qneg => 1 / (1 + Math.Exp(-Params.PhiWeight * (Weight - Params.WHalf)));
        public Rodent(Random rng, Position pos = null, IAnimalParams customParameters = null) : base(rng, pos)
        {
            Params = customParameters is null ? new RodentParams() : customParameters;

        }

        public new List<Rodent> Birth(int sameSpeciesInCell)
        {
            if (GivenBirth) return null;
            double probability;
            if (Weight < (Params.Zeta * (Params.BirthWeight + Params.BirthSigma)))
            {
                probability = 0.0;
            }
            else
            {
                probability = Params.Gamma * Fitness * (sameSpeciesInCell - 1);
                probability = (probability > 1) ? 1 : probability;
            }

            if (probability >= rng.NextDouble())
            {
                var norm = new MathNet.Numerics.Distributions.Normal(2, 2);
                int numberOfOffspring =  1 + (int)Math.Floor(norm.Sample());
                numberOfOffspring = (numberOfOffspring < 1) ? 1 : numberOfOffspring;
                List<Rodent> offspring = new List<Rodent>();
                for (int i = 0; i < numberOfOffspring; i++)
                {
                    Rodent newborn = new Rodent(rng, new Position(Pos.x, Pos.y));
                    newborn.Params.OverloadParameters(Params.CopyParameters()); // Inherit params from parent
                    if (InheritTracking)
                    {
                        newborn.Tracked = true;
                        newborn.InheritTracking = InheritTracking;
                        newborn.Parents.AddRange(Parents);
                        newborn.Parents.Add(ID);
                    }

                    double bWeight = newborn.Weight;
                    if (bWeight >= Weight || bWeight <= 0) break;
                    Weight -= Params.Xi * bWeight;
                    offspring.Add(newborn);
                }
                NumberOfBirths++;
                GivenBirth = true;
                return offspring;
            }
            return null;
        }
        
    }

    public class Carnivore : Animal
    {

        public override double Qplus => 1 / (1 + Math.Exp(Params.PhiAge * (Age - Params.AHalf)));
        public override double Qneg => 1 / (1 + Math.Exp(-Params.PhiWeight * (Weight - Params.WHalf)));
        public Carnivore(Random rng, Position pos = null, IAnimalParams customParameters = null) : base(rng, pos)
        {
            Params = customParameters is null ? new CarnivoreParams() : customParameters;
            var norm = new MathNet.Numerics.Distributions.Normal(Params.BirthWeight, Params.BirthSigma);
            Weight = norm.Sample();
        }

        public void FeedOld(List<Herbivore> herbivores)
        {
            // Go through all Herbivores one by one, killing to reach Params.F. 
            // Check if the herbivore is alive with herb.IsAlive
            double eaten = 0;
            foreach (var herb in herbivores)
            {
                if (eaten >= Params.F)
                {
                    Console.WriteLine("Carnivore has reached F eaten of H.Weight");
                    break; // Animal is full
                }
                if (!herb.IsAlive) continue; // Animal is already dead
                if (Fitness <= herb.Fitness) continue; // Cannot kill animal, try the next one
                if (0 < Fitness - herb.Fitness && Fitness - herb.Fitness < Params.DeltaPhiMax)
                { // Try to kill and eat
                    if ((Fitness - herb.Fitness) / Params.DeltaPhiMax > rng.NextDouble())
                    {
                        eaten += herb.Weight;
                        Weight += herb.Weight * Params.Beta;
                        herb.Kill();
                    }
                    else
                    {
                        continue; // Fails to kill animal, try the next one
                    }
                }
                else
                { // Kill it and eat
                    eaten += herb.Weight;
                    Weight += herb.Weight * Params.Beta;
                    herb.Kill();
                }


            }
        }

        public int Feed(List<Herbivore> herbs)
        {
            int killed = 0;
            double eaten = 0.0;
            herbs = herbs.OrderBy(i => i.Fitness).ToList();
            foreach (var h in herbs)
            {
                if (eaten >= Params.F)
                {
                    //Carnivore is full, stop hunting
                    break;
                }
                if (!h.IsAlive) continue; // Animal is already dead
                if (Fitness < h.Fitness)
                {
                    //Herbivore has too high fitness to kill, go to next H
                    continue;
                }
                else if (0 < Fitness - h.Fitness && Fitness - h.Fitness < Params.DeltaPhiMax)
                {
                    // try to kill
                    if (rng.NextDouble() < (Fitness - h.Fitness) / Params.DeltaPhiMax)
                    {
                        // Animal is killed
                        eaten += KillHerbivore(h);
                        killed++;
                    }
                    else
                    {
                        // Kill fails
                        continue; // Redundant continue...
                    }
                }
                else
                {
                    // Carnivore will kill
                    eaten += KillHerbivore(h);
                    killed++;
                }
            }
            return killed;
        }

        public double KillHerbivore(Herbivore herb)
        {
            Weight += herb.Weight * Params.Beta;
            herb.Kill();
            return herb.Weight;
        }
    }

}
