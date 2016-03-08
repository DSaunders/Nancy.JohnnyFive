namespace Nancy.JohnnyFive.Database
{
    using System.Collections.Generic;
    using Circuits;

    public interface IJohhnyFiveDatabase
    {
        IDictionary<string, IList<ICircuit>> CircuitBreakers { get; set; }
    }
}
