namespace Nancy.JohnnyFive.Tests
{
    using System.Collections.Generic;
    using Circuits;
    using Database;

    public class FakeJohnnyFiveDatabase : IJohhnyFiveDatabase
    {
        public IDictionary<string, ICircuit> CircuitBreakers { get; set; }

        public FakeJohnnyFiveDatabase()
        {
            CircuitBreakers = new Dictionary<string, ICircuit>();
        }
    }
}