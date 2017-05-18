using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<RegistrationUser> _openRegistrations = new List<RegistrationUser>();

        private RegistrationService()
        {
        }

        public string NewRegistrant(string name, Region region)
        {
            RegistrationUser user = new RegistrationUser
            {
                Code = this.GetGuid(),
                Name = name,
                Region = region
            };
            this._openRegistrations.Add(user);
            return user.Code;
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
            RegistrationUser match = this._openRegistrations.FirstOrDefault(x => x.Name.Equals(name) && x.Region.Equals(region));
            if (!match.IsExpired())
            {
                return true;
            }
            return false;
        }
    }
}
