namespace Nancy.JohnnyFive.Database
{
    using System.Collections.Generic;
    using Circuits;

    public class JohhnyFiveDatabase : IJohhnyFiveDatabase
    {
        public IDictionary<string, IList<ICircuit>> CircuitBreakers { get; set; }

        public JohhnyFiveDatabase()
        {
            CircuitBreakers = new Dictionary<string, IList<ICircuit>>();
        }
    }
}