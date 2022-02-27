# Parameters


## Parameter class
The parameter class contains the definition for the collection of parameters that govern the behaviour of both the island cells and the animals that inhabit these cells. These parameters can be set by either altering the default values in the subclasses. The parameters can be overridden in the instanced animal classes by calling 
```cs 
// Overload 1
Param.OverloadParameters(Dictionary<string, double> newParameters);
// Overload 2
Param.OverloadParameters(IAnimalParams parameters); 
// Overload 3
Param.OverloadParameters(double bWeight, double bSigma, double beta, double eta, double aHalf,
            double phiAge, double wHalf, double phiWeight, double mu, double gamma, double zeta,
            double xi, double omega, double f, double deltaPhiMax);
```

### Returning the parameters as a Dictionary
The parameters can also be written out as a dictionary to be used by other methods in the program.  
```cs
public Dictionary<string, double> CopyParameters()
```

### Save Parameters to file
To write out all parameters contained in a Parameter object, use the `SaveValues()` method.  
```cs
public void SaveValue(string folder, string name);
```
The output filename is defined using the parameter `name`.  
The output directory is defined using the parameter `folder`.  
This method will then write all parameters is has into a .csv file.  

---

## Position class
To keep track of cell and animal position, a two dimentional integer "vector" object is used called "Position". A position object can be defined in two ways:  
```cs
int xCor = 10;
int yCor = 15;
Position a = new Position(xCor, yCor);
Position b = new Position {
    x = xCor,
    y = yCor
}
```