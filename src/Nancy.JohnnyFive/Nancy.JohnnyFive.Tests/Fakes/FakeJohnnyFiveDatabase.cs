namespace Nancy.JohnnyFive.Tests.Fakes
{
    using System.Collections.Generic;
    using Database;
    using JohnnyFive.Circuits;

    public class FakeJohnnyFiveDatabase : IJohhnyFiveDatabase
    {
        public IDictionary<string, IList<ICircuit>> CircuitBreakers { get; set; }

        public FakeJohnnyFiveDatabase()
        {
            CircuitBreakers = new Dictionary<string, IList<ICircuit>>();
        }
    }
}