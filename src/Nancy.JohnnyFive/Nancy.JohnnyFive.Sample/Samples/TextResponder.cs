namespace Nancy.JohnnyFive.Sample.Samples
{
    using System;
    using Circuits;
    using Models;
    using Responders;

    public class FlipFlopCircuit : ICircuit
    {
        public CircuitState State { get; set; }

        public void AfterRequest(Response response)
        {
        
        }

        public void BeforeRequest(Request request)
        {
            if (State == CircuitState.Normal)
                State = CircuitState.ShortCircuit;
            else
                State = CircuitState.Normal;
        }

        public void OnError(Exception ex)
        {
        
        }
    }

    public class TextResponder : IResponder
    {
        public void AfterRequest(Response response)
        {
        }

        public Response GetResponse()
        {
            return "Short circuited";
        }
    }
}