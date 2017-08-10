using System;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Database.DAO;
using XayahBot.Error;

namespace XayahBot.Utility
{
    public sealed class Property
    {
        public static readonly Property FilePath = new Property("file_path", AppContext.BaseDirectory + "/", false);
        public static readonly Property FileDiscordToken = new Property("file__discord_token", "discord.token", false);
        public static readonly Property FileRiotApiKey = new Property("file_riot_api_key", "riot_api.key", false);
        public static readonly Property FileChampionGGApiKey = new Property("file_championgg_api_key", "championgg_api.key", false);

        public static readonly Property IncidentDisabled = new Property("incidents_disabled", "");
        public static readonly Property ChampDisabled = new Property("champ_disabled", "");

        public static readonly Property GameActive = new Property("game_active", "with Rakan");
        public static readonly Property RiotUrlVersion = new Property("rioturl_version", "7.16.1");
        public static readonly Property RemindDayCap = new Property("remind_day_cap", "30");
        public static readonly Property RemindTextCap = new Property("remind_text_cap", "100");
        public static readonly Property IncidentCheckInterval = new Property("incidents_interval", "15");

        public static IEnumerable<Property> UpdatableValues()
        {
            yield return IncidentDisabled;
            yield return ChampDisabled;
            yield return GameActive;
            yield return RiotUrlVersion;
            yield return RemindDayCap;
            yield return RemindTextCap;
            yield return IncidentCheckInterval;
        }

        public static Property GetUpdatableByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                Property match = UpdatableValues().FirstOrDefault(x => x.Name.ToLower().Equals(name));
                if (match != null)
                {
                    return match;
                }
            }
            throw new NotExistingException();
        }

        // ---

        private PropertyDAO _propertyDAO = new PropertyDAO();

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
                        this._value = this._propertyDAO.GetValue(this);
                    }
                    catch (NotExistingException)
                    {
                        this._propertyDAO.SetValue(this);
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
                        this._propertyDAO.SetValue(this);
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
