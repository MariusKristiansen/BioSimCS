using System.IO;

namespace Biosim.Simulation
{
    public class LogWriter
    {
        /*
         Collects all data and writes it to csv */
        public string FilePath { get; set; }
        public string FileName { get; set; }
        private string _data;
        public LogWriter(string filepath, string filename, string header)
        {
            FilePath = filepath;
            FileName = filename;
            _data += $"{header}\n";
        }

        public void Log(string line)
        {
            _data += $"{line}\n";
        }

        public void LogCSV()
        {
            using TextWriter sw = new StreamWriter(Path.Combine(FilePath, FileName));
            foreach (var line in _data.Split('\n'))
            {
                sw.WriteLine(line);
            }
        }
    }
}
