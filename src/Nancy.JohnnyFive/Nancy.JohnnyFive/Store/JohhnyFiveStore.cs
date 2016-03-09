namespace Nancy.JohnnyFive.Store
{
    using System.Collections.Generic;
    using System.Linq;
    using Circuits;

    public class JohhnyFiveStore : IJohhnyFiveStore
    {
        private readonly IDictionary<string, IList<ICircuit>> _circuits;

        public JohhnyFiveStore()
        {
            _circuits = new Dictionary<string, IList<ICircuit>>();
        }

        public void AddIfNotExists(string route, IList<ICircuit> circuits)
        {
            if (circuits != null && !_circuits.ContainsKey(route))
                _circuits.Add(route, circuits);
        }

        public IEnumerable<ICircuit> GetForRoute(string route)
        {
            return _circuits.ContainsKey(route) 
                ? _circuits[route] 
                : Enumerable.Empty<ICircuit>();
        }
    }
}