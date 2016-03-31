namespace Nancy.JohnnyFive.Store
{
    using System.Collections.Generic;
    using Models;

    public interface IStore
    {
        void AddIfNotExists(string route, IEnumerable<RouteConfig> configs);
        IEnumerable<RouteConfig> GetForRoute(string route);
    }
}
