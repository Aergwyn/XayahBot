using System.Collections.Generic;
using System.Linq;

namespace XayahBot.API
{
    public class ApiRequest
    {
        public string CacheId { get; private set; }
        public string Resource { get; private set; }
        public List<string> Arguments { get; private set; }

        public ApiRequest(string resource, params string[] arguments)
        {
            this.CacheId = resource + string.Join("-", arguments);
            this.Resource = resource;
            this.Arguments = arguments?.ToList() ?? new List<string>();
        }
    }
}
