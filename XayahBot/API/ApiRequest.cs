using System.Collections.Generic;
using System.Linq;

namespace XayahBot.API
{
    public class ApiRequest
    {
        public IEnumerable<string> Arguments { get; private set; }
        public string CacheId { get; private set; }
        public string Resource { get; private set; }

        public ApiRequest(string resource, params string[] arguments)
        {
            this.CacheId = resource + arguments?.Count();
            this.Resource = resource;
            this.Arguments = arguments?.ToList() ?? new List<string>();
        }
    }
}
