using System.IO;

namespace Biosim.Simulation
{
    public class PopulationMapper
    {
        private string _popMap;
        public string Location { get; set; }
        public string Filename { get; set; }
        public PopulationMapper(string filename, string location)
        {
            Location = location;
            Filename = filename;
        }

        public void WriteCSV()
        {
            using (TextWriter sw = new StreamWriter($"{Location}/{Filename}.csv"))
            {
                foreach (var line in _popMap.Split('\n'))
                {
                    sw.WriteLine(line);
                }
            }
        }

        public void LogPop(string line)
        {
            _popMap += $"{line}\n";
        }
    }
}
