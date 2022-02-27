using System;
using System.Collections.Generic;
using System.Text;

using Biosim.Parameters;

namespace Biosim.Tools
{
    public enum Command
    {
        InsertAnimals,
        RemoveAnimals,
        HerbivoreCap,
        CarnivoreCap,
        CellFood,
        CellFoodMax,
        FoodMaxAll,
        Passable,
        UpdateAnimalParameters,
        KillHerbivores,
        KillCarnivores,
        GlobalHerbivoreCap,
        GlobalCarnivoreCap,
        GlobalKillHerbivores,
        GlobalKillCarnivores,
        GlobalKillAllHerbivores,
        GlobalKillAllCarnivores,
        GlobalKillAll,
        Kill,
        KillAll,
        KillAllHerbivores,
        KillAllCarnivores
    }

    public enum GlobalCommands
    {
        // Commands that run "island wide" and does not require a cell position parameter
        FoodMaxAll,
        GlobalKillAllHerbivores,
        GlobalKillAllCarnivores,
        GlobalKillHerbivores,
        GlobalKillCarnivores,
        GlobalKillAll
    }

    public class CommandData : ICommandData
    {
        public bool Global { get; set; } = false;
        public Command Command { get; set; }
        public string Parameters { get; set; }
        public Position CellPosition { get; set; } = null; // Null assignment not needed, but just as a sanity check
        public int ActivationYear { get; set; }

        public override string ToString()
        {
            return $"Command: {Command}\nActiviation Year: {ActivationYear}\nCellPosition: {CellPosition}\nParameters: {Parameters}";
        }
    }
}
