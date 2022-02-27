using System;
using System.Collections.Generic;
using System.Text;
using Biosim.Animals;
using Biosim.Models;

namespace Biosim.Tools
{
    public class DatabaseHandler
    {
        //private string[] cache;
        private IAnimal[] animalCache;
        private ResultModel[] resultCache;
        private int cachePos = 0;
        private int resultCachePos = 0;
        private int cacheSize;
        public DatabaseHandler(int cacheSizeinput = 100)
        {
            cacheSize = cacheSizeinput;
            //cache = new string[cacheSize];
            animalCache = new IAnimal[cacheSize];
            resultCache = new ResultModel[cacheSize];
        }

        /// <summary>
        /// Maybe change over to creating a model and saving them in the cache to save memory. 
        /// </summary>
        /// <param name="animal"></param>
        /// <param name="tableName"></param>
        public void AddDeadAnimalToDatabase(IAnimal animal, string tableName)
        {
            if (cachePos == cacheSize) // Cahche is full, write cache to database
            {
                using (var db = new EFContext())
                {
                    foreach (var anim in animalCache)
                    {
                        if (animal.GetType().Name == "Herbivore")
                        {
                            HerbivoreModel model = new HerbivoreModel();
                            model.HerbivoreID = anim.ID;
                            model.Age = anim.Age;
                            model.Fitness = anim.Fitness;
                            model.Weight = anim.Weight;
                            model.NumberOfBirths = anim.NumberOfBirths;
                            db.Herbivores.Add(model);
                        }
                        else
                        {
                            CarnivoreModel model = new CarnivoreModel();
                            model.CarnivoreID = anim.ID;
                            model.Age = anim.Age;
                            model.Fitness = anim.Fitness;
                            model.Weight = anim.Weight;
                            model.NumberOfBirths = anim.NumberOfBirths;
                            db.Carnivores.Add(model);
                        }
                    }
                    db.SaveChanges();
                    animalCache = new IAnimal[cacheSize];
                    animalCache[0] = animal;
                    cachePos = 1;
                }
            } else
            {
                animalCache[cachePos] = animal;
                cachePos++;
            }
            
        }

        public void SaveAnimalCache()
        {
            using (var db = new EFContext())
            {
                foreach (var animal in animalCache)
                {
                    if (animal is null) break;
                    if (animal.GetType().Name == "Herbivore")
                    {
                        HerbivoreModel model = new HerbivoreModel();
                        model.HerbivoreID = animal.ID;
                        model.Age = animal.Age;
                        model.Fitness = animal.Fitness;
                        model.Weight = animal.Weight;
                        model.NumberOfBirths = animal.NumberOfBirths;
                        db.Herbivores.Add(model);
                    }
                    else
                    {
                        CarnivoreModel model = new CarnivoreModel();
                        model.CarnivoreID = animal.ID;
                        model.Age = animal.Age;
                        model.Fitness = animal.Fitness;
                        model.Weight = animal.Weight;
                        model.NumberOfBirths = animal.NumberOfBirths;
                        db.Carnivores.Add(model);
                    }
                }
                db.SaveChanges();
            }
            cachePos = 0;
            animalCache = new IAnimal[cacheSize];
            // Saves the remaining cache to database
            

        }

        public static void Add(IAnimal animal, EFContext context)
        {
            if (animal.GetType().Name == "Herbivore")
            {
                HerbivoreModel model = new HerbivoreModel();
                model.HerbivoreID = animal.ID;
                model.Age = animal.Age;
                model.Fitness = animal.Fitness;
                model.Weight = animal.Weight;
                model.NumberOfBirths = animal.NumberOfBirths;
                context.Herbivores.Add(model);
            }
            else
            {
                CarnivoreModel model = new CarnivoreModel();
                model.CarnivoreID = animal.ID;
                model.Age = animal.Age;
                model.Fitness = animal.Fitness;
                model.Weight = animal.Weight;
                model.NumberOfBirths = animal.NumberOfBirths;
                context.Carnivores.Add(model);
            }
        }

        public void UpdateAnimalInDatabase(int ID, string tableName)
        {

        }

        public void RemoveAnimalInDatabase(int ID, string tableName)
        {

        }

        public void AddResult(int year, int herbivores, int carnivores, double hFitness, double cFitness, double avgHerbAge, double avgCarnAge, double avgHerbWeight, double avgCarnWeight, int herbBirths, int carnBirths)
        {
            var model = new ResultModel
            {
                Year = year,
                Herbivores = herbivores,
                Carnivores = carnivores,
                HerbivoreAvgFitness = hFitness,
                CarnivoreAvgFitness = cFitness,
                HerbivoreAvgAge = avgHerbAge,
                CarnivoreAvgAge = avgCarnAge,
                HerbivoreAvgWeight = avgHerbWeight,
                CarnivoreAvgWeight = avgCarnWeight,
                HerbivoreBirths = herbBirths,
                CarnivoreBirths = carnBirths
            };

            if (resultCachePos == cacheSize) // Cache is full, write to database
            {
                using (var db = new EFContext())
                {
                    foreach (var result in resultCache)
                    {
                        db.Results.Add(result);
                    }
                    db.SaveChanges();
                }
                resultCache[0] = model;
                resultCachePos = 1;
            } else
            {
                resultCache[resultCachePos] = model;
                resultCachePos++;
            }
        }

        public void SaveResultCache()
        {
            using (var db = new EFContext())
            {
                foreach (var result in resultCache)
                {
                    if (result is null) break;
                    db.Results.Add(result);
                }
                db.SaveChanges();
            }
        }

        public void WipeResultDatabase()
        {
            using (var db = new EFContext())
            {
                if (db.Results is null) return;
                db.Results.RemoveRange(db.Results);
                db.SaveChanges();
            }
        }
    }
}
