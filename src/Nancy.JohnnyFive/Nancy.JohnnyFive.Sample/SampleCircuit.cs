namespace Nancy.JohnnyFive.Sample
{
    using System;
    using Circuits;
    using Models;

    public class SampleCircuit : ICircuit
    {
        public CircuitState State { get; private set; }
        public Response BeforeRequest()
        {
            if (State == CircuitState.Open)
                return "Closed";

            return null;
        }

        public void AfterRequest(Response response)
        {
            
        }

        public Response OnError(Exception ex)
        {
            State = CircuitState.Open;
            return null;
        }
    }
}