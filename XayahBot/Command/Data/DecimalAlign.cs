using System.Globalization;

namespace XayahBot.Command.Data
{
    public class DecimalAlign
    {
        private int _intDigits;
        private int _decDigits;

        public DecimalAlign(params decimal[] numbers)
        {
            int intDigits = 1;
            int decDigits = 1;
            foreach (decimal number in numbers)
            {
                string value = number.ToString("G0", CultureInfo.InvariantCulture);
                int decPosition = this.GetDecPosition(value);
                if (decPosition > intDigits)
                {
                    intDigits = decPosition;
                }
                int newDecDigits = value.Length - (decPosition + 1);
                if (newDecDigits > decDigits)
                {
                    decDigits = newDecDigits;
                }
            }
            this._intDigits = intDigits;
            this._decDigits = decDigits;
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
