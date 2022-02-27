# Tools

## Commands
This file contains the definition for the `CommandData` object and two `enums` representing all available commands the user can use in the .biosim script file.  
This is an example of a command.  
```cs
CommandData command = new CommandData {
    Global = false, // bool
    Command = Command.Passable, // Command enum for the requested command
    CellPosition = new Position(x, y), // Position object indicating wich cell the command shall be executed on
    ActivationYear = 0, // The given year the command is executed
    Global = false // Indicates if the command is global or local for one cell
};
```

Commands available:  
```cs
InsertAnimals
RemoveAnimals
HerbivoreCap
CarnivoreCap
CellFood
CellFoodMax
FoodMaxAll
Passable
UpdateAnimalParameters
KillHerbivores
KillCarnivores
GlobalKillHerbivores
GlobalKillCarnivores
GlobalKillAllHerbivores
GlobalKillAllCarnivores
GlobalKillAll
Kill
KillAll
KillAllHerbivores
KillAllCarnivores
```

---

## LogWriter
This class collects data over the run of the simulation and saves them as csv upon simulation completion.   
Instancing of the log writer class is done like this:  
```cs
LogWriter log = new LogWriter(string filepath, string filename, string header);
```
Where header is the first line of the generated .csv file. i.e. the coulumn names of the created file.  

### **Methods**
```cs
Log(string line);
```
Saves the given line string to it's local cache.  

```cs
LogCSV();
```
Writes the cached lines to a csv file with the pattern `{filepath}{filename}.csv`.  

---

## DatabaseHandler
Used to interface with the database and performs CRUD operations on the stored data. 

> DATABASEINFO  
> The database implementation is not feature complete. Currently the program can use the database to store basic simulation data mirroring the SimResult.csv file. 
> Optimally the database should store a representation of all individuals by animal ID, a table of populations per cell per year.

### Constructor
The handler is instanced with the following constructor:  
```cs
DatabaseHandler DBh = new DatabaseHandler(int cacheSize=100);
```
The handler will store data locally until the local cache is full, then it will bulk write the data into the database. Handler can be made standalone by adding the posiibility to include a external EFcontext. ?  

### Adding Animal to database  
```cs
DBh.AddDeadAnimalToDatabase(IAnimal animal);
```
The method creates a model representation of the animal and adds it to the local cache. If the cache is full, it writes the cache to database, wipes the cache and adds the given animal to cache.

### Saving the cache
Any animals left in the cache will not be automatically written to the database, therefore to avoid data loss, invoke the method `SaveAnimalCache();`.
```cs
DBh.SaveAnimalCache();
```
This method will enumerate through the cache and write all remaining animals from cache into database.  


---

## ScriptInterpreter
This class is used to convert the commands found in the script.biosim file line by line into commands that the simulation can execute.  

### Constructor
```cs
ScriptInterpreter intr = new ScriptInterpreter(string filename);
```
The interpreter will look for a file from the given filename, and uses this as it's source for commands.  
The interpreter can be customised by defining what symbols are used to define linebreaks and ardument delimiters in the file.  
```cs
intr.Delimiter = ' '; // Default delimiter is space. 
intr.Linebreak = '\n'; // Defualt linebreak is newline
```
Commands are defined line by line in a .biosim file according to pattern defined [here](../../README.md#Commands)  

### Parsing
To parse the commands from the file, use the method `ScriptInterpreter.Parse()`.
```cs
public Dictionary<int, CommandData> Parse()
```
The Parse method returns the parsed commands into a dictionary ordered by the year the [command](#Commands) is supposed to run. 