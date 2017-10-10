using Microsoft.Azure.WebJobs;
using System.Collections.Generic;

namespace RomMaster.Server.WebJob
{
    internal class NameResolver : INameResolver
    {
        private readonly IDictionary<string, string> map;

        public NameResolver(IDictionary<string, string> map)
        {
            this.map = map;
        }

        public string Resolve(string name)
        {
            return map[name];
        }
    }
}
