namespace Nancy.JohnnyFive.Tests.Fakes
{
    using System.Collections.Generic;
    using System.Linq;
    using JohnnyFive.Store;
    using Models;

    public class FakeStore : IStore
    {
        public Dictionary<string, IEnumerable<RouteConfig>> Db;

        public FakeStore()
        {
            Db = new Dictionary<string, IEnumerable<RouteConfig>>();
        }

        public void AddIfNotExists(string route, IEnumerable<RouteConfig> configs)
        {
            if (!Db.ContainsKey(route) && configs != null)
                Db[route] = configs;
        }

        public IEnumerable<RouteConfig> GetForRoute(string route)
        {
            return Db.ContainsKey(route)
                ? Db[route]
                : Enumerable.Empty<RouteConfig>();
        }
    }
}