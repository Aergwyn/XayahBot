using System.IO;
using System.Linq;

namespace XayahBot.Utility
{
    public class FileReader
    {
        public string ReadFirstLine(string path)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(path))
            {
                result = File.ReadAllLines(path).ElementAt(0);
            }
            return result;
        }
    }
}
