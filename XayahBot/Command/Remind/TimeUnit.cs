using System.Collections.Generic;
using System.Linq;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.Remind
{
    public class TimeUnit
    {
        public static readonly TimeUnit Day = new TimeUnit(1, "days", "day", "d");
        public static readonly TimeUnit Hour = new TimeUnit(2, "hours", "hour", "h");
        public static readonly TimeUnit Minute = new TimeUnit(3, "minutes", "minute", "mins", "min", "m");

        public static IEnumerable<TimeUnit> Values()
        {
            yield return Day;
            yield return Hour;
            yield return Minute;
        }

        public static TimeUnit Get(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToLower();
                TimeUnit match = Values().FirstOrDefault(x => x._matches.Contains(text));
                if (match != null)
                {
                    return match;
                }
            }
            throw new NotExistingException();
        }

        // ---

        private int _id;
        private List<string> _matches = new List<string>();

        private TimeUnit(int id, params string[] matches)
        {
            this._id = id;
            this._matches.AddRange(matches);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is TimeUnit)
            {
                TimeUnit compObj = obj as TimeUnit;
                return this._id.Equals(compObj._id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ListUtil.BuildEnumeration(this._matches);
        }
    }
}
