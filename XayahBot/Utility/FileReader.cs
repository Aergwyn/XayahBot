using System.IO;
using System.Linq;

namespace XayahBot.Utility
{
    public static class FileReader
    {
        public static string GetFirstLine(string path)
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
