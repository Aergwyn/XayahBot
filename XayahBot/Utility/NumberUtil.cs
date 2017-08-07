using System.Text.RegularExpressions;

namespace XayahBot.Utility
{
    public static class NumberUtil
    {
        public static int StripForNumber(string value)
        {
            int number = 0;
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                result = Regex.Replace(value, "[^0-9]+", string.Empty);
                if (int.TryParse(result, out number))
                {
                    return number;
                }
            }
            return -1;
        }
    }
}
