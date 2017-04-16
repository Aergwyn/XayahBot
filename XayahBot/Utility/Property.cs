using System;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Database.Service;

namespace XayahBot.Utility
{
    public sealed class Property
    {
        // Static "Enums" with set values
        public static readonly Property Author = new Property("author", "Aergwyn#8786", false);
        public static readonly Property CmdPrefix = new Property("cmd_prefix", ".", false);
        public static readonly Property DbName = new Property("db_name", "xayah.db", false);
        public static readonly Property FilePath = new Property("file_path", AppContext.BaseDirectory + @"\", false);
        public static readonly Property FileRiotApiKey = new Property("file_riotapikey", "riotapikey.txt", false);
        public static readonly Property FileToken = new Property("file_token", "token.txt", false);
        // Changeable "Enums" with default values
        public static readonly Property ConfigMods = new Property("cfg_mods", "");
        public static readonly Property DataLongevity = new Property("data_longevity", "24"); // Hours
        public static readonly Property GameActive = new Property("game_active", "with Rakan");
        public static readonly Property GameShutdown = new Property("game_shutdown", "shutting down...");
        public static readonly Property QuizLastReset = new Property("quiz_lastreset", "1/2017");
        public static readonly Property QuizMatch = new Property("quiz_match", "70"); // Percentage
        public static readonly Property QuizMaxTries = new Property("quiz_maxtries", "10");
        public static readonly Property QuizTimeout = new Property("quiz_timeout", "10"); // Minutes

        //

        public static IEnumerable<Property> Values
        {
            get
            {
                yield return Author;
                yield return CmdPrefix;
                yield return DbName;
                yield return FilePath;
                yield return FileRiotApiKey;
                yield return FileToken;
                //
                yield return ConfigMods;
                yield return DataLongevity;
                yield return GameActive;
                yield return GameShutdown;
                yield return QuizLastReset;
                yield return QuizMatch;
                yield return QuizMaxTries;
                yield return QuizTimeout;
            }
        }

        public static Property GetByName(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                return Values.Where(x => x.Updatable).FirstOrDefault(x => x.Name.ToLower().Equals(text.ToLower()));
            }
            return null;
        }

        //

        public string Name { get; private set; }
        private string _value { get; set; }
        public string Value
        {
            get
            {
                if (this.NeedInit())
                {
                    string newValue = PropertyService.GetValue(this);
                    if (newValue != null)
                    {
                        this._value = newValue;
                    }
                    this.Loaded = true;
                }
                return this._value;
            }
            set
            {
                string realValue = value != null ? value.ToString() : string.Empty;
                if (!realValue.Equals(this._value))
                {
                    this._value = realValue;
                    if (this.Updatable)
                    {
                        PropertyService.SetValueAsync(this);
                    }
                }
            }
        }
        public bool Updatable { get; private set; }
        public bool Loaded { get; private set; }

        //

        private Property(string name, string value) : this(name, value, true) { }

        private Property(string name, string value, bool updatable)
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

        //

        #region Overrides

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

        #endregion
    }
}
