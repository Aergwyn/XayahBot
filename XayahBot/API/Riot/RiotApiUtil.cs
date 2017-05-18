using System.Text.RegularExpressions;

namespace XayahBot.API.Riot
{
    public static class RiotApiUtil
    {
        public static bool IsValidName(string name)
        {
            if (Regex.IsMatch(name, "^[0-9\\p{L} _\\.]+$"))
            {
                return true;
            }
            return false;
        }
    }
}
