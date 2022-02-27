using System;
using System.Collections.Generic;
using System.Text;

namespace Biosim.Tools
{
    public interface IScriptInterpreter
    {
        string ScriptPath { get; set; }
        char Delimiter { get; set; }
        char Linebreak { get; set; }
        Dictionary<int, CommandData> ParsedCommands { get; set; }

        CommandData ValidateLine(string line);
        List<string> SplitLines(string lines, char delimiter);
        bool GetFile();
    }
}
