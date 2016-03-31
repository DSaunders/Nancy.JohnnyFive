namespace Nancy.JohnnyFive.Store
{
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    internal class Store : IStore
    {
        private readonly Dictionary<string, IEnumerable<RouteConfig>> _db;

        public Store()
        {
            _db = new Dictionary<string, IEnumerable<RouteConfig>>();
        }

        public void AddIfNotExists(string route, IEnumerable<RouteConfig> configs)
        {
            if (!_db.ContainsKey(route) && configs != null)
                _db[route] = configs;
        }

        public IEnumerable<RouteConfig> GetForRoute(string route)
        {
            return _db.ContainsKey(route) 
                ? _db[route] 
                : Enumerable.Empty<RouteConfig>();
        }
    }
}