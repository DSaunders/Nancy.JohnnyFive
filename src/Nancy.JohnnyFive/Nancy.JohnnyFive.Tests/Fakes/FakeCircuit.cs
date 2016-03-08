namespace Nancy.JohnnyFive.Tests.Fakes
{
    using System;
    using Circuits;
    using Models;

    public class FakeCircuit : ICircuit
    {
        public CircuitState State { get; private set; }
        public Response BeforeRequest()
        {
            return null;
        }

        public void AfterRequest(Response response)
        {
            
        }

        public Response OnError(Exception ex)
        {
            return null;
        }
    }
}