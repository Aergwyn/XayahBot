using System;
using System.Collections.Generic;
using System.Threading;
using XayahBot.API.Riot;
using XayahBot.Utility;

namespace XayahBot.Command.Account
{
    public class RegistrationService
    {
        private static RegistrationService _instance;

        public static RegistrationService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RegistrationService();
            }
            return _instance;
        }

        // ---

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private Dictionary<string, CacheEntry> _openRegistrations = new Dictionary<string, CacheEntry>();

        private RegistrationService()
        {
        }

        public string NewRegistrant(string name, Region region)
        {
            string code = string.Empty;
            RegistrationUser user = new RegistrationUser
            {
                Name = name,
                Region = region
            };
            do
            {
                code = this.GetGuid();
            }
            while (this._openRegistrations.ContainsKey(code));
            this._openRegistrations.Add(code, new CacheEntry(user, DateTime.UtcNow.AddMinutes(20)));
            return code;
        }

        private string GetGuid()
        {
            int length = 16;
            string code = Guid.NewGuid().ToString();
            code = code.Replace("-", string.Empty);
            if (code.Length > length)
            {
                code = code.Substring(0, length);
            }
            return code;
        }

        public bool ValidateCode(string name, Region region)
        {
            return false;
        }
    }
}
