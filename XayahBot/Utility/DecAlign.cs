using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XayahBot.Utility
{
    public class DecAlign
    {
        public int IntDigits { get; set; }
        public int DecDigits { get; set; }

        //

        public DecAlign(List<decimal> numbers)
        {
            int intDigits = 1;
            int decDigits = 1;
            foreach (decimal number in numbers)
            {
                string value = number.ToString("G0", CultureInfo.InvariantCulture);
                int decPosition = value.IndexOf(".") < 0 ? value.Length : value.IndexOf(".");
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
            this.IntDigits = intDigits;
            this.DecDigits = decDigits;
        }

        //

        public int GetFieldLength()
        {
            return this.IntDigits + 1 + this.DecDigits;
        }

        public string Align(decimal number)
        {
            string value = number.ToString("G0", CultureInfo.InvariantCulture);
            int decPosition = value.IndexOf(".") < 0 ? value.Length : value.IndexOf(".");
            return value.PadLeft((this.IntDigits - decPosition) + value.Length).PadRight(this.GetFieldLength());
        }
    }
}
