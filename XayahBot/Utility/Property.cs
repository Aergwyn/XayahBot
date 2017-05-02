using System;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Database.Service;

namespace XayahBot.Utility
{
    public sealed class Property
    {
        public static readonly Property Author = new Property("author", "Aergwyn#8786", false);
        public static readonly Property CmdPrefix = new Property("cmd_prefix", ".", false);
        public static readonly Property DbName = new Property("db_name", "xayah.db", false);
        public static readonly Property FilePath = new Property("file_path", AppContext.BaseDirectory + "/", false);
        public static readonly Property FileRiotApiKey = new Property("file_riotapikey", "riotapikey.txt", false);
        public static readonly Property FileToken = new Property("file_token", "token.txt", false);

        public static readonly Property GameActive = new Property("game_active", "with Rakan");
        public static readonly Property GameShutdown = new Property("game_shutdown", "shutting down...");
        public static readonly Property QuizLastReset = new Property("quiz_lastreset", "1/2017");
        public static readonly Property QuizLeaderboardCd = new Property("quiz_leaderboardcd", "5"); // Minutes
        public static readonly Property QuizLeaderboardMax = new Property("quiz_leaderboardmax", "10");
        public static readonly Property QuizMatch = new Property("quiz_match", "70"); // Percentage
        public static readonly Property QuizMaxTries = new Property("quiz_maxtries", "10");
        public static readonly Property QuizTimeout = new Property("quiz_timeout", "10"); // Minutes

        public static IEnumerable<Property> UpdatableValues
        {
            get
            {
                yield return GameActive;
                yield return GameShutdown;
                yield return QuizLastReset;
                yield return QuizLeaderboardCd;
                yield return QuizLeaderboardMax;
                yield return QuizMatch;
                yield return QuizMaxTries;
                yield return QuizTimeout;
            }
        }

        public static Property GetUpdatableByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return UpdatableValues.Where(x => x.Updatable).FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
            }
            return null;
        }

        //

        private PropertyDAO _propertyDao = new PropertyDAO();

        public string Name { get; private set; }
        private string _value { get; set; }
        public string Value
        {
            get
            {
                if (this.NeedInit())
                {
                    this._value = this._propertyDao.GetValue(this) ?? this._value;
                    this.Loaded = true;
                }
                return this._value;
            }
            set
            {
                string realValue = value?.ToString() ?? string.Empty;
                if (!realValue.Equals(this._value))
                {
                    this._value = realValue;
                    if (this.Updatable)
                    {
                        this._propertyDao.SetValueAsync(this);
                    }
                }
            }
        }
        public bool Updatable { get; set; }
        private bool Loaded { get; set; }

        private Property(string name, string value, bool updatable = true)
        {
            this.Name = name;
            this._value = value;
            this.Updatable = updatable;
            this.Loaded = !updatable;
        }

        //

        private bool NeedInit()
        {
            if (this.Loaded)
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                string comp = obj.ToString();
                if (this.ToString().Equals(comp))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
