namespace Nancy.JohnnyFive.Store
{
    using System.Collections.Generic;
    using Circuits;

    public interface IJohhnyFiveStore
    {
        void AddIfNotExists(string route, IList<ICircuit> circuits);
        IEnumerable<ICircuit> GetForRoute(string route);
    }
}
