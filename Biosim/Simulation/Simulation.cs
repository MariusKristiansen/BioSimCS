using Biosim.Animals;
using Biosim.Land;
using Biosim.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Biosim.Tools;
using Bitmapper;
using System.IO;
using System.Diagnostics;

namespace Biosim.Simulation
{
    public class Sim : ISimulation

    {
        public LogWriter FoodLog;
        internal static int BaseID = 0;  // HOW TO REFFER TO THIS AND UPDATE IT???
        public DatabaseHandler DBHandler = new();

        public Sim(int yearsToSimulate = 100, string template = null, bool noMigration = false)
        {
            Rng = new Random();
            ScriptParser = new ScriptInterpreter("script.biosim");
            Commands = ScriptParser.Parse();
            HerbivorePopulationMap = new PopulationMapper("HerbivorePopulation", "../../../Results");
            CarnivorePopulationMap = new PopulationMapper("CarnivorePopulation", "../../../Results");
            Logger = new LogWriter(ResultsDirectory, "SimResult.csv", "Year,Herbivores,Carnivores,HerbivoreAvgFitness,CarnivoreAvgFitness,HerbivoreAvgAge,CarnivoreAvgAge,HerbivoreAvgWeight,CarnivoreAvgWeight,HerbivoresBornThisYear,CarnivoresBornThisYear");
            NoMigration = noMigration;
            YearsToSimulate = yearsToSimulate;
            if (template is null)
            {
                TemplateString = DefaultParameters.DefaultIsland;
            }
            else
            {
                if (template.Length < 19) throw new Exception("Template string must be at least 4x4 cells");
                TemplateString = template;
            }

            if (!(Commands is null) && Commands.Keys.Max() > YearsToSimulate)
            {
                throw new Exception("Invalid year value in .biosim script file! Activiation year cannot exceed Years to simulate.");
            }

            Dimentions = Build(); // Builds the "Land" prop and returns it's dimentions as object.
            GenerateIslandImage();
            if (DBsave) DBHandler.WipeResultDatabase();
        }

        private void GenerateIslandImage()
        {
            var image = new BitmapGenerator(1000, "../../../Results/island");
            var island = TemplateString.Replace("\n", "");
            var resolution = 1000;
            var xDim = Dimentions.x;

            var factor = resolution / xDim;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    var Celltype = island[x / factor * xDim + y / factor];
                    switch (Celltype)
                    {
                        case 'D':
                            image.InsertPixel(y, x, 153, 51, 0, 255);
                            break;
                        case 'O':
                            image.InsertPixel(y, x, 0, 0, 204, 255);
                            break;
                        case 'M':
                            image.InsertPixel(y, x, 100, 100, 152, 255);
                            break;
                        case 'J':
                            image.InsertPixel(y, x, 0, 51, 0, 255);
                            break;
                        case 'S':
                            image.InsertPixel(y, x, 204, 204, 0, 255);
                            break;
                        default:
                            break;
                    }
                }
            }
            image.Save();
        }

        #region Properties
        public bool DBsave { get; set; } = false;
        public bool Built { get; set; } = false; // Set to true when build has been run. 
        public string ResultsDirectory { get; set; } = "../../../Results";
        public bool Follow { get; set; } = false;
        public PopulationMapper HerbivorePopulationMap { get; set; }
        public PopulationMapper CarnivorePopulationMap { get; set; }
        public int YearsToSimulate { get; set; }
        public bool NoMigration { get; set; }
        public string TemplateString { get; set; }
        public List<List<IEnvironment>> Land { get; set; }
        public Position DefaultDim { get; set; } = new Position { x = 10, y = 10 };
        public Random Rng { get; set; }
        public LogWriter Logger { get; set; }
        public Position Dimentions { get; set; }
        public int TotalDeadHerbivores { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TotalDeadCarnivores { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DeadHerbivoresThisYear { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DeadCarnivoresThisYear { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double AverageHerbivoreFitness => Land.Select(i => i.Select(j => j.HerbivoreAvgFitness).Average()).Average();
        public double AverageCarnivoreFitness => Land.Select(i => i.Select(j => j.CarnivoreAvgFitness).Average()).Average();
        public double AverageHerbivoreAge => Land.Select(i => i.Select(j => j.HerbivoreAvgAge).Average()).Average();
        public double AverageCarnivoreAge => Land.Select(i => i.Select(j => j.CarnivoreAvgAge).Average()).Average();
        public double AverageHerbivoreWeight => Land.Select(i => i.Select(j => j.HerbivoreAvgWeight).Average()).Average();
        public double AverageCarnivoreWeight => Land.Select(i => i.Select(j => j.CarnivoreAvgWeight).Average()).Average();
        public double PeakHerbiovreFitness => Land.Select(i => i.Select(j => j.PeakHerbivoreFitness).Max()).Max();
        public double PeakCarnivoreFitness => Land.Select(i => i.Select(j => j.PeakCarnivoreFitness).Max()).Max();
        public double PeakHerbivoreWeight => Land.Select(i => i.Select(j => j.PeakHerbivoreWeight).Max()).Max();
        public double PeakCarnivoreWeight => Land.Select(i => i.Select(j => j.PeakCarnivoreWeight).Max()).Max();
        public int HerbivoresBornThisYear => Land.Select(i => i.Select(j => j.NewHerbivores).Sum()).Sum();
        public int CarnivoresBornThisYear => Land.Select(i => i.Select(j => j.NewCarnivores).Sum()).Sum();
        public int TotalHerbivoresCreated { get; set; }
        public int TotalCarnivoresCreated { get; set; }
        public int CurrentYear { get; set; } = 0;
        public int LiveHerbivores => Land.Select(i => i.Select(j => j.NumberOfHerbivores).Sum()).Sum();
        public int LiveCarnivores => Land.Select(i => i.Select(j => j.NumberOfCarnivores).Sum()).Sum();
        public ScriptInterpreter ScriptParser { get; set; }
        public Dictionary<int, CommandData> Commands { get; set; }
        public List<IAnimal> TrackedAnimals { get; set; }
        public List<int> AnimalsToTrack { get; set; }
        #endregion

        #region AddAnimal
        public void AddAnimals(List<Animal> animals, Position cellPosition)
        {
            if (animals.Count() <= 0 || animals is null) throw new Exception("You must provide animals to insert");

            foreach (var animal in animals)
            {
                if (animal.GetType().Name == "Herbivore")
                {
                    Land[cellPosition.x][cellPosition.y].Herbivores.Add((Herbivore)animal);
                }
                else
                {
                    Land[cellPosition.x][cellPosition.y].Carnivores.Add((Carnivore)animal);
                }
            }
        }

        public void AddCarnivore(int age, double w, Position cellPosition, IAnimalParams par = null)
        {
            Land[cellPosition.x][cellPosition.y].Carnivores.Add(
                new Carnivore(
                    Rng,
                    new Position { x = cellPosition.x, y = cellPosition.y },
                    par)
                { Weight = w, Age = age });
        }

        public void AddHerbivore(int age, double w, Position cellPosition, IAnimalParams par = null)
        {
            Land[cellPosition.x][cellPosition.y].Herbivores.Add(
                new Herbivore(
                    Rng,
                    new Position { x = cellPosition.x, y = cellPosition.y },
                    par)
                { Weight = w, Age = age });
        }

        public void AddCarnivore(Position cellPosition, IAnimalParams par = null)
        {
            Land[cellPosition.x][cellPosition.y].Carnivores.Add(
                new Carnivore(
                    Rng,
                    new Position { x = cellPosition.x, y = cellPosition.y },
                    par));
        }

        public void AddHerbivore(Position cellPosition, IAnimalParams par = null)
        {
            Land[cellPosition.x][cellPosition.y].Herbivores.Add(
                new Herbivore(
                    Rng,
                    new Position { x = cellPosition.x, y = cellPosition.y },
                    par));
        }
        #endregion

        public Position Build()
        {
            Land = new List<List<IEnvironment>>();
            var logstring = "year";
            if (TemplateString.Contains('/')) throw new Exception("The input template string has incorrect formatting, use backslash \"\\\" to annotate newline");
            var lines = TemplateString.Split('\n');
            var xDim = lines.Length;
            var yDim = lines[0].Length;
            for (int i = 0; i < lines.Length; i++)
            {
                List<IEnvironment> islandLine = new List<IEnvironment>();
                for (int j = 0; j < lines[i].Length; j++)
                {
                    logstring += $",{lines[i][j]}";
                    switch (lines[i][j])
                    {
                        case 'D':
                            islandLine.Add(new Desert(new Position { x = i, y = j }, Rng, DBHandler));
                            break;
                        case 'M':
                            islandLine.Add(new Mountain(new Position { x = i, y = j }, Rng, DBHandler));
                            break;
                        case 'S':
                            islandLine.Add(new Savannah(new Position { x = i, y = j }, Rng, DBHandler));
                            break;
                        case 'J':
                            islandLine.Add(new Jungle(new Position { x = i, y = j }, Rng, DBHandler));
                            break;
                        case 'O':
                            islandLine.Add(new Ocean(new Position { x = i, y = j }, Rng, DBHandler));
                            break;
                        default:
                            break;
                    }
                }
                Land.Add(islandLine);
            }
            
            FoodLog = new LogWriter(ResultsDirectory, "FoodOverview.csv", logstring);
            Built = true;
            return new Position() { x = xDim, y = yDim };
        }

        public void ChangeCellParameters(Position cellPos, EnvParams newParams)
        {
            throw new NotImplementedException();
        }

        public List<Position> GetSurroundingCells(Position cellPos)
        {

            var cellList = new List<Position>();
            for (var j = -1; j < 2; j++)
            {
                for (var i = -1; i < 2; i++)
                {

                    if (i != 0 || j != 0)
                    {
                        if (cellPos.x + i >= 0 && cellPos.y + j >= 0 && cellPos.x + i < Land.Count() && cellPos.y + j < Land[0].Count())
                        {
                            if (Land[cellPos.x + i][cellPos.y + j].Passable)
                            {
                                cellList.Add(new Position
                                {
                                    x = cellPos.x + i,
                                    y = cellPos.y + j
                                });
                            }

                        }

                    }

                }
            }
            return cellList;
        }

        public void LoadCustomOnCellParameters(Position cellPos, IAnimalParams parameters)
        {
            if (parameters.GetType().Name == "HerbivoreParams")
            {
                Land[cellPos.x][cellPos.y].Herbivores.ForEach(i => i.Params = parameters);
                return;
            }
            if (parameters.GetType().Name == "CarnivoreParams")
            {
                Land[cellPos.x][cellPos.y].Carnivores.ForEach(i => i.Params = parameters);
            }
        }

        public bool LoadCustomParametersOnAnimal(IAnimal animal, IAnimalParams parameters)
        {
            /*Find the animal*/
            foreach (var line in Land)
            {
                foreach (var cell in line)
                {
                    foreach (var herb in cell.Herbivores)
                    {
                        if (herb == animal)
                        {
                            herb.Params = parameters;
                            return true;
                        }
                    }
                    foreach (var carn in cell.Carnivores)
                    {
                        if (carn == animal)
                        {
                            carn.Params = parameters;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Migrate(IEnvironment cell)
        {
            /*
            Gets surrounding cells and feeds them to the individual animals in a cell, the animal then sets a personal parameter indicating what cell it wants to move to*/
            if (NoMigration) return;
            var surrounding = GetSurroundingCells(cell.Pos);
            cell.Herbivores.ForEach(i => i.Migrate(surrounding));
            cell.Carnivores.ForEach(i => i.Migrate(surrounding));

        }

        public void OneYear()
        {
            var feedString = $"{CurrentYear}";
            var popStringHerb = $"{CurrentYear}";
            var popStringCarn = $"{CurrentYear}";
            for (int i = 0; i < Dimentions.x; i++)
            {
                for (int j = 0; j < Dimentions.y; j++)
                {
                    var cell = Land[i][j];
                    OneCellYearFirstHalf(cell);
                    Migrate(cell);

                }
            }

            MoveAnimals(); // Do the migration!

            for (int i = 0; i < Dimentions.x; i++)
            {
                for (int j = 0; j < Dimentions.y; j++)
                {
                    var cell = Land[i][j];
                    OneCellYearSecondHalf(cell);
                    //Save Log info here.
                    feedString += $",{cell.Food}";
                    popStringHerb += $",{cell.NumberOfHerbivores}";
                    popStringCarn += $",{cell.NumberOfCarnivores}";
                }
            }

            HerbivorePopulationMap.LogPop(popStringHerb);
            CarnivorePopulationMap.LogPop(popStringCarn);
            FoodLog.Log(feedString);
            Logger.Log($"{CurrentYear.ToString().Replace(',', '.')},{LiveHerbivores.ToString().Replace(',', '.')},{LiveCarnivores.ToString().Replace(',', '.')},{AverageHerbivoreFitness.ToString().Replace(',', '.')},{AverageCarnivoreFitness.ToString().Replace(',', '.')},{AverageHerbivoreAge.ToString().Replace(',', '.')},{AverageCarnivoreAge.ToString().Replace(',', '.')},{AverageHerbivoreWeight.ToString().Replace(',', '.')},{AverageCarnivoreWeight.ToString().Replace(',', '.')},{HerbivoresBornThisYear.ToString().Replace(',', '.')},{CarnivoresBornThisYear.ToString().Replace(',', '.')}");
            if (DBsave) DBHandler.AddResult(CurrentYear, LiveHerbivores, LiveCarnivores, AverageHerbivoreFitness, AverageCarnivoreFitness, AverageHerbivoreAge, AverageCarnivoreAge, AverageHerbivoreWeight, AverageCarnivoreWeight, HerbivoresBornThisYear, CarnivoresBornThisYear);
            CurrentYear++;
        }

        public string GetCellInformation(IEnvironment cell)
        {
            return cell.ToString();
        }

        public void OneCellYearFirstHalf(IEnvironment cell)
        {
            cell.ResetCurrentYearParameters();
            cell.GrowFood();
            cell.HerbivoreFeedingCycle();
            cell.CarnivoreFeedingCycle();
            cell.BirthCycle();
            TotalHerbivoresCreated += cell.NewHerbivores;
            TotalCarnivoresCreated += cell.NewCarnivores;
        }
        public void OneCellYearSecondHalf(IEnvironment cell)
        {
            cell.AgeCycle();
            cell.WeightLossCycle();
            cell.DeathCycle();
            cell.RemoveDeadIndividuals();
            cell.ResetGivenBirthParameter();
            cell.ResetMigrationParameter();
        }

        public void Plot()
        {
            Process.Start("CMD.exe", "/C python ../../../Scripts/plot.py");
        }

        public void Simulate()
        { //Runs the simulation for x years
            var watch = new Stopwatch();
            watch.Start();
            Console.WriteLine($"Running Simulation for {YearsToSimulate} years\nIsland Layout:");
            Console.WriteLine(TemplateString);
            //HerbivoreParams.SaveValues("../../../Results");
            //CarnivoreParams.SaveValues("../../../Results");
            //RodentParams.SaveValues("../../../Results");
            var herb = new HerbivoreParams();
            herb.SaveValues("../../../Results", "herbivoreparams");
            var carn = new CarnivoreParams();
            carn.SaveValues("../../../Results", "carnivoreparams");

            int simulated = 0;
            for (int i = 0; i < YearsToSimulate; i++)
            {
                if (LiveHerbivores + LiveCarnivores <= 0) break;
                if (!(Commands is null) && Commands.Keys.Contains(i)) { // Found a command to execute
                    ExecuteCommand(Commands[i]);
                }
                OneYear();
                GetTrackedAnimals();
                simulated++;
            }
            watch.Stop();
            if (DBsave)DBHandler.SaveResultCache();
            SaveCSV();
            Plot();
            Console.WriteLine($"Simulation finished after {simulated} years. \nElapsed time: {watch.ElapsedMilliseconds/1000} Seconds");
            Console.Write("Generate png image sets? (Takes a while...) [N/y]: ");
            var result = Console.ReadLine();
            if (result.ToLower().Contains("y"))
            {
                GeneratePopulationImages();
                Console.Write("Run FFMPEG? [N/y]: ");
                result = Console.ReadLine();
                if (result.ToLower().Contains("y")) RunFFMPEG();
            }
        }

        public void RunFFMPEG()
        {
            //RunScript(@"../../../Scripts/deleteVideofiles.bat");
            RunScript(@"../../../Scripts/GenerateHerbivoremp4.bat");
            RunScript(@"../../../Scripts/GenerateCarnivoremp4.bat");
            RunScript(@"../../../Scripts/Combine4.bat");

            //System.Diagnostics.Process.Start("CMD.exe", "/C ffmpeg -framerate 24 -i ../../../Results/carnivoresequence/carnivoreimg%08d.png ../../../Results/mp4/carnivores.mp4");
            //System.Diagnostics.Process.Start("CMD.exe", "/C ffmpeg -framerate 24 -i ../../../Results/herbivoresequence/herbivoreimg%08d.png ../../../Results/mp4/herbivores.mp4");
            //System.Diagnostics.Process.Start("CMD.exe", "/C del ../../../Results/mp4/populations.mp4 && ffmpeg -i ../../../Results/mp4/carnivores.mp4 -i ../../../Results/mp4/herbivores.mp4 -filter_complex hstack ../../../Results/mp4/populations.mp4");
            Console.WriteLine("Video files Generated. Press enter to continue.");
        }

        public void RunScript(string command)
        {
            var processInfo = new ProcessStartInfo(command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => Console.WriteLine("output>> " + e.Data);
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => Console.WriteLine(e.Data);
                
            process.BeginErrorReadLine();

            process.WaitForExit();

            Console.WriteLine("ExitCode: {0}", process.ExitCode);
            process.Close();
        }

        public void GeneratePopulationImages()
        {
            BitmapGenerator.FileStream("../../../Results/HerbivorePopulation.csv", "../../../Results/herbivoresequence/", "herbivoreimg", 20, 1000, false);
            BitmapGenerator.FileStream("../../../Results/CarnivorePopulation.csv", "../../../Results/carnivoresequence/", "carnivoreimg", 20, 1000, false);
        }

        public void ExecuteCommand(CommandData command)
        {
            Console.WriteLine($"\n==>\nRunning command: \n{command}\n");
            //throw new NotImplementedException();

            switch (command.Command)
            {
                case Command.CellFood:
                    // Include tests to ensure parameter argument is int and valid
                    {
                        Land[command.CellPosition.x][command.CellPosition.y].Food = int.Parse(command.Parameters);
                    }
                    break;
                case Command.CellFoodMax:
                    {
                        var cell = GetCell(command.CellPosition);
                        if (cell.GetType().Name == "Jungle" ||
                            cell.GetType().Name == "Savannah")
                        {
                            cell.Params.Fmax = double.Parse(command.Parameters);
                        }
                    }

                    break;
                case Command.FoodMaxAll:
                    { // Have to limit scope!!!!
                        foreach (var row in Land)
                        {
                            foreach (var cell in row)
                            {
                                if (cell.GetType().Name == "Jungle" ||
                                    cell.GetType().Name == "Savannah")
                                {
                                    cell.Params.Fmax = double.Parse(command.Parameters);
                                }
                            }
                        }
                    }
                    break;
                case Command.CarnivoreCap:
                    break;
                case Command.HerbivoreCap:
                    {
                        if (!int.TryParse(command.Parameters, out int number)) throw new Exception("Unable to parse input parameter");
                        while (LiveHerbivores > number)
                        {
                            //Pick a random cell and a random herbivore and kill it. 
                        }
                    }
                    break;
                case Command.KillCarnivores:
                    {
                        var number = int.Parse(command.Parameters);
                        var cell = GetCell(command.CellPosition);
                        number = (number > cell.Carnivores.Count) ? cell.Carnivores.Count : number;
                        for (int i = 0; i < number; i++)
                        {
                            cell.Carnivores[i].Kill();
                        }
                        cell.RemoveDeadIndividuals();
                    }
                    break;
                case Command.KillHerbivores:
                    {
                        var number = int.Parse(command.Parameters);
                        var cell = GetCell(command.CellPosition);
                        number = (number > cell.Herbivores.Count) ? cell.Herbivores.Count : number;
                        for (int i = 0; i < number; i++)
                        {
                            cell.Herbivores[i].Kill();
                        }
                        cell.RemoveDeadIndividuals();
                    }
                    break;
                case Command.InsertAnimals:
                    {
                        //var cell = GetCell(command.CellPosition);
                        var arguments = command.Parameters.Split(' ');
                        if (arguments.Length > 2)
                        {

                        } else
                        {
                            var type = arguments[2].Trim().ToLower();
                            if (!int.TryParse(arguments[1].Trim(), out int amount)) throw new Exception($"Unable to parse additional argument: {command.Parameters}");
                            for (int i = 0; i < amount; i++)
                            {
                                if (type == "herbivore")
                                {
                                    this.AddHerbivore(command.CellPosition);
                                } else if (type == "carnivore")
                                {
                                    this.AddCarnivore(command.CellPosition);
                                } else
                                {
                                    throw new Exception($"Unable to parse animal type parameter: {type}");
                                }
                            }
                        }
                    }
                    break;
                case Command.RemoveAnimals:
                    break;
                case Command.Passable:
                    Land[command.CellPosition.x][command.CellPosition.y].Passable = bool.Parse(command.Parameters);
                    break;
                case Command.UpdateAnimalParameters:
                    break;
                case Command.Kill:
                    break;
                case Command.KillAll:
                    {
                        var cell = GetCell(command.CellPosition);
                        cell.Herbivores.ForEach(i => i.Kill());
                        cell.Carnivores.ForEach(i => i.Kill());
                        cell.RemoveDeadIndividuals();
                    }
                    break;
                case Command.GlobalKillHerbivores:
                    {   // Randomly select a carnivore and kill it until the number has been reached.
                        var amountToKill = int.Parse(command.Parameters);
                        var killed = 0;
                        while (LiveHerbivores > 0)
                        {
                            var AllHerbivores = Land.SelectMany(i => i.SelectMany(j => j.Herbivores).ToList()).ToList();
                            var rand = Rng.Next(AllHerbivores.Count);
                            if (AllHerbivores[rand].Kill()) killed++;
                            if (killed == amountToKill) break;
                        }
                        ManualRemoveDead();
                    }
                    break;
                case Command.GlobalKillCarnivores:
                    {
                        var amountToKill = int.Parse(command.Parameters);
                        var killed = 0;
                        while (LiveCarnivores > 0)
                        {
                            var AllCarnivores = Land.SelectMany(i => i.SelectMany(j => j.Carnivores).ToList()).ToList();
                            var rand = Rng.Next(AllCarnivores.Count);
                            if (AllCarnivores[rand].Kill()) killed++;
                            if (killed == amountToKill) break;
                        }
                        ManualRemoveDead();
                    }
                    break;
                case Command.GlobalKillAllCarnivores:
                    {
                        foreach (var row in Land)
                        {
                            foreach (var cell in row)
                            {
                                cell.Carnivores.ForEach(i => i.Kill());
                                cell.RemoveDeadIndividuals();
                            }
                        }
                    }
                    break;
                case Command.GlobalKillAllHerbivores:
                    {
                        foreach (var row in Land)
                        {
                            foreach (var cell in row)
                            {
                                cell.Herbivores.ForEach(i => i.Kill());
                                cell.RemoveDeadIndividuals();
                            }
                        }
                    }
                    break;
                case Command.KillAllHerbivores:
                    {
                        var cell = GetCell(command.CellPosition);
                        cell.Herbivores.ForEach(i => i.Kill());
                        cell.RemoveDeadIndividuals();
                    }
                    break;
                case Command.KillAllCarnivores:
                    { 
                        var cell = GetCell(command.CellPosition);
                        cell.Carnivores.ForEach(i => i.Kill());
                        cell.RemoveDeadIndividuals();
                    }
                    break;

                case Command.GlobalKillAll:
                    {
                        foreach (var row in Land)
                        {
                            foreach (var cell in row)
                            {
                                cell.Herbivores.ForEach(i => i.Kill());
                                cell.Carnivores.ForEach(i => i.Kill());
                                cell.RemoveDeadIndividuals();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        public void ManualRemoveDead()
        {
            foreach (var row in Land)
            {
                foreach (var cell in row)
                {
                    cell.RemoveDeadIndividuals();
                }
            }
        }

        public override string ToString()
        {
            return TemplateString;
        }

        public void MoveAnimals()
        {
            /*
             Create a layout of animals and positions they wish to move to. 
             */
            //Extract all animals into a list
            List<IAnimal> animals = new List<IAnimal>();
            foreach (var row in Land)
            {
                foreach (var cell in row)
                {
                    foreach (var herb in cell.Herbivores)
                    {
                        animals.Add(herb);
                    }
                    foreach (var carn in cell.Carnivores)
                    {
                        animals.Add(carn);
                    }
                }
            }

            // Remove all animals from cells
            foreach (var row in Land)
            {
                foreach (var cell in row)
                {
                    cell.Herbivores = new List<Herbivore>();
                    cell.Carnivores = new List<Carnivore>();
                }
            }

            // Place each animal in the cells. This can and should be improved to only moving the animals who's "want to move to" -parameter is not the same as it's position parameter
            foreach (var animal in animals)
            {
                animal.Pos = new Position { x = animal.GoingToMoveTo.x, y = animal.GoingToMoveTo.y };
                if (animal.GetType().Name == "Herbivore")
                {
                    Land[animal.GoingToMoveTo.x][animal.GoingToMoveTo.y].Herbivores.Add((Herbivore)animal);
                }
                else
                {
                    Land[animal.GoingToMoveTo.x][animal.GoingToMoveTo.y].Carnivores.Add((Carnivore)animal);
                }
            }
        }

        /// <summary>
        /// Writes all loggers data to csv files
        /// </summary>
        public void SaveCSV()
        {
            Logger.LogCSV();
            FoodLog.LogCSV(); // Remvove or refactor later
            HerbivorePopulationMap.WriteCSV();
            CarnivorePopulationMap.WriteCSV();
        }

        public IEnvironment GetCell(Position pos)
        {
            if (pos.x > Dimentions.x) return null;
            if (pos.y > Dimentions.y) return null;
            return Land[pos.x][pos.y];
        }

        public IEnvironment GetCell(int x, int y)
        {
            if (x > Dimentions.x) return null;
            if (y > Dimentions.y) return null;
            return Land[x][y];
        }

        public void TrackAnimals()
        {
            if (!(AnimalsToTrack is null))
            {
                foreach (var id in AnimalsToTrack)
                {
                    // THIS IS A DIRTY HACK...
                    Land.ForEach(i => i.ForEach(j => j.Herbivores.Where(a => a.ID == id).ToList().ForEach(k => k.Tracked = true)));
                    Land.ForEach(i => i.ForEach(j => j.Carnivores.Where(a => a.ID == id).ToList().ForEach(k => k.Tracked = true)));
                    //Land.ForEach(i => i.ForEach(j => j.Herbivores.FirstOrDefault(a => a.ID == id).Tracked = true));
                    //Land.ForEach(i => i.ForEach(j => j.Carnivores.FirstOrDefault(a => a.ID == id).Tracked = true));
                }
                GetTrackedAnimals();
            } else
            {
                throw new Exception("AnimalsToTrack List is null!!!");
            }
        }

        public void GetTrackedAnimals()
        {
            TrackedAnimals = new List<IAnimal>(); // Wipe the old list... 
            foreach (var row in Land)
            {
                foreach (var cell in row)
                {
                    var trackedHerb = cell.Herbivores.Where(i => i.Tracked).ToList();
                    var trakcedCarn = cell.Carnivores.Where(i => i.Tracked).ToList();
                    TrackedAnimals.AddRange(trackedHerb);
                    TrackedAnimals.AddRange(trakcedCarn);
                }
            }
        }

        public void AddTrackedAnimals(List<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
