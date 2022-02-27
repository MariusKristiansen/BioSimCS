using System;
using System.Collections.Generic;
using System.Linq;
using Biosim.Parameters;
using System.Text;

namespace Biosim.Tools
{
    public class ScriptInterpreter : IScriptInterpreter
    {
        public string ScriptPath { get; set; }
        public char Delimiter { get; set; } = ' ';
        public char Linebreak { get; set; } = '\n';
        public Dictionary<int, CommandData> ParsedCommands { get; set; }
        public ScriptInterpreter(string path)
        {
            ScriptPath = path;
            ParsedCommands = new Dictionary<int, CommandData>();
        }

        public Dictionary<int, CommandData> Parse()
        {
            var raw = System.IO.File.ReadAllText(ScriptPath);
            var lines = SplitLines(raw, Linebreak);
            foreach (var line in lines)
            {
                if (line.Contains("//")) continue; // Indicates if a line should be ignored.
                var command = ValidateLine(line);
                ParsedCommands.Add(command.ActivationYear, command);
            }
            return (ParsedCommands.Count == 0) ? null : ParsedCommands;
        }

        public CommandData ValidateLine(string line)
        {
            Position pos = new Position();
            CommandData cmd;
            var elements = line.Split(Delimiter);
            if (!int.TryParse(elements[0], out int year)) throw new Exception("First argument must provide a year");
            var command = (Command)Enum.Parse(typeof(Command), elements[1]);
            if (GlobalCommands.IsDefined(typeof(GlobalCommands), command.ToString()))
            { // Command is a global command, position is not needed and shall be set to null
                var parameter = elements[2].Replace(";", "").Replace("\r", "");
                cmd = new CommandData
                {
                    Global = true,
                    ActivationYear = year,
                    Command = command,
                    Parameters = parameter,
                    CellPosition = null
                };
            } else
            {
                var coordinates = elements[2];
                var x = int.Parse(coordinates.Split(',')[0].Replace("(", "").Trim());
                var y = int.Parse(coordinates.Split(',')[1].Replace(")", "").Trim());
                pos.x = x;
                pos.y = y;

                var parameter = "";// elements[3].Replace(";", "").Replace("\r", "");
                for (int i = 3; i < elements.Length; i++)
                {
                    parameter += elements[i].Replace(";", "").Replace("\r", "");
                    parameter += " ";
                }
                cmd = new CommandData
                {
                    Global = false,
                    ActivationYear = year,
                    Command = command,
                    CellPosition = pos,
                    Parameters = parameter
                };
            }

            return cmd;
        }

        public List<string> SplitLines(string lines, char delimiter)
        {
            if (lines is null) throw new Exception("Input lines string is null!");
            if (lines.Length < 10) throw new Exception("Input lines string looks to be too short!");
            var split = lines.Split(delimiter).ToList();
            return split;
        }

        public bool GetFile()
        {
            throw new NotImplementedException();
        }
    }
}
