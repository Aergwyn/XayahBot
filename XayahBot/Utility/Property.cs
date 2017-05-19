using System;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Database.DAO;
using XayahBot.Database.Error;
using XayahBot.Error;

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
        public static readonly Property HelpServerLink = new Property("help_serverlink", "https://discord.gg/YhQYAFW");
        public static readonly Property ReminderCap = new Property("reminder_cap", "6");
        public static readonly Property ReminderTextCap = new Property("reminder_textcap", "60");

        public static IEnumerable<Property> UpdatableValues
        {
            get
            {
                yield return GameActive;
                yield return GameShutdown;
                yield return HelpServerLink;
                yield return ReminderCap;
                yield return ReminderTextCap;
            }
        }

        public static Property GetUpdatableByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Property match = UpdatableValues.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
                if (match != null)
                {
                    return match;
                }
            }
            throw new UnknownTypeException();
        }

        // ---

        private PropertiesDAO _propertiesDao = new PropertiesDAO();

        public string Name { get; private set; }
        private string _value { get; set; }
        public string Value
        {
            get
            {
                if (this.NeedInit())
                {
                    this.Loaded = true;
                    try
                    {
                        this._value = this._propertiesDao.GetValue(this);
                    }
                    catch (NotExistingException)
                    {
                        this._propertiesDao.SetValueAsync(this);
                    }
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
                        this._propertiesDao.SetValueAsync(this);
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
            return this.ToString().Equals(obj?.ToString());
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
