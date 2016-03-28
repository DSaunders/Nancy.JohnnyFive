namespace Nancy.JohnnyFive.Store
{
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    internal interface IStore
    {
        void AddIfNotExists(string route, IEnumerable<RouteConfig> configs);
        IEnumerable<RouteConfig> GetForRoute(string route);
    }

    internal class Store : IStore
    {
        private Dictionary<string, IEnumerable<RouteConfig>> _db = new Dictionary<string, IEnumerable<RouteConfig>>();

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
