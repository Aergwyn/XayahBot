using System;
using System.Globalization;

namespace XayahBot.Command.RiotData
{
    public class NumberAlign
    {
        private int _intDigits;
        private int _decDigits;

        public NumberAlign(int decimals, params decimal[] numbers)
        {
            int intDigits = 1;
            foreach (decimal number in numbers)
            {
                string value = number.ToString("G0", CultureInfo.InvariantCulture);
                int decPosition = this.GetDecPosition(value);
                if (decPosition > intDigits)
                {
                    intDigits = decPosition;
                }
            }
            this._intDigits = intDigits;
            this._decDigits = decimals;
        }

        private int GetDecPosition(string value)
        {
            
            int decPosition = value.IndexOf(".");
            if (decPosition < 0)
            {
                decPosition = value.Length;
            }
            return decPosition;
        }

        public string Align(decimal number)
        {
            number = Math.Round(number, this._decDigits, MidpointRounding.AwayFromZero);
            string value = number.ToString("G0", CultureInfo.InvariantCulture);
            int decPosition = this.GetDecPosition(value);
            return value.PadLeft((this._intDigits - decPosition) + value.Length).PadRight(this._intDigits + 1 + this._decDigits);
        }

        public string TrimmedAlign(decimal number)
        {
            return this.Align(number).TrimEnd();
        }
    }
}
