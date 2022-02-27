# Simulation class

## Constructor  

```cs
public Sim(int yearsToSimulate = 100, string template = null, bool noMigration = false)
```
### Years to simulate
The amount of years the simulation should target to run unless interrrupted.  

### Template
The template parameter defines the island layout. More info [here](../../README.md#Sim).  

### No migration  
The noMigration parameter tells the simulation if it should allow the animals to migrate or if they should be confined in their own cells.  

## Methods

```cs
void ExecuteCommand(CommandData command);
```
Ececutes the given command on the simulation.  

```cs
Position Build();
```
Builds the island from provided template in constructor.  

```cs
void AddAnimals(List<Animal> animals, Position cellPosition);
```
Adds a population of animals into the given cell based on the Position argument  

```cs
void AddHerbivore(int age, double w, Position cellPosition, IAnimalParams par);
```
Adds a single Herbivore with the provided custom parameters into the given cell.  

```cs
void AddCarnivore(int age, double w, Position cellPosition, IAnimalParams par);
```
Adds a single Carnivore with the provided custom parameters into the given cell.  


```cs
void AddHerbivore(Position cellPosition, IAnimalParams par);
```
Add one starting Herbivore with custom parameters to the cell. 

```cs
void AddCarnivore(Position cellPosition, IAnimalParams par);
```
Add one starting Carnivore with custom parameters to the cell.

```cs
string GetCellInformation(IEnviroment cell);
```
Gets a string representation of the given cell.  

```cs
IEnviroment GetCell(Position pos);
```
Returns the cell at the given position basen on position object.  

```cs
IEnviroment GetCell(int x, int y);
```
Returns the cell at the given x and y position.  

```cs
void AddTrackedAnimals(List<int> ids); 
```
Adds the animals by the given IDs to tracking by setting their tracked parameter and adds them to a List of tracked animals.  

```cs
void TrackAnimals();
```

```cs
void RunScript(string command)
```
Runs the batch script defined in the string command, or cmdlet defined in command and halts execution until the script has exited.    

```cs
void GeneratePopulationImages()
```
Runs the Bitmapper.Filestream() to generate .png heatmap images from populationmap csv files.  

```cs
void GetTrackedAnimals(); // Fetches all tracked animals in the simulation and adds them to the local list of trakced animals.
```
Looks through all living animals in the simulation, if their tracked property is set, but they are not in the tracked animals list, then they are added to the list.  

```cs
void Simulate();
```
Runs the simulation.  

```cs
void OneCellYearFirstHalf(IEnviroment cell);
```
Runs the first individual steps of the cycle per cell until the migration step.  

```cs
void OneCellYearSecondHalf(IEnviroment cell);
```
Runs the remaining steps of the cycle cell per cell.  

```cs
void OneYear(); // Runs the simulation for one year and returns a string of data
```
Calls OneCellYearFirstHalf, Migrate and OneCellYearSecondHalf.  

```cs
void LoadCustomOnCellParameters(Position cellPos, IAnimalParams parameters); // Parameters for all animals of a type in cell
```
Loads the set of custom enviroment parameters on the given cell position.  

```cs
bool LoadCustomParametersOnAnimal(IAnimal animal, IAnimalParams parameters);
```
Loads the set of custom parameters on the given animal.  

```cs
List<Position> GetSurroundingCells(Position cellPos);
```
Gets the surrounding cells around the given cell position.  

```cs
void MoveAnimals(); 
```
Moves the animals that wants to move into their requested cells.  

```cs
void Plot();
```
Runs the plot.py scirpt to generate a collection of graphs representing the run of the simulation.  

```cs
void ChangeCellParameters(Position cellPos, EnvParams newParams);
```
Alters the cell parameters on the given cells according to the newParams object.  

```cs
void SaveCSV();
```
Writes the data from all loggers into csv files.  

```cs
void ManualRemoveDead();
```
Manually enumerate through all cells in the island and remove all dead individuals from the cells.  

### Migration
```cs
void Migration();
```
This method runs the main migration algorithm on the simulation. 
It creates a map of all animals that wish to [migrate](../Animals/README.md#Migrate) by creating a map of neighboring cells that animals can move into (that have the propery `passable`) for each cell in the island. Then if feeds this list of cells to each individual animal in each cell. If the animal wishes to migrate, it will respond with a Position object indicating wich cell it wants to move to by setting its `GoingToMoveTo` property. The simulation then generates list of all animals on the island, then removes the animals in each cell, then places them back into the new requested cell animal for animal.  