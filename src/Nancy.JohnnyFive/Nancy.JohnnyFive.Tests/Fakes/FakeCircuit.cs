namespace Nancy.JohnnyFive.Tests.Fakes
{
    using System;
    using JohnnyFive.Circuits;
    using Models;

    public class FakeCircuit : ICircuit
    {
        public CircuitState State { get; set; }

        public Response AfterRequestCall { get; set; }
        public Request BeforeRequestCall { get; set; }
        public Exception OnErrorCall { get; set; }
        
        public void AfterRequest(Response response)
        {
            AfterRequestCall = response;
        }

        public void BeforeRequest(Request request)
        {
            BeforeRequestCall = request;
        }

        public void OnError<T>(T ex) where T: Exception
        {
            OnErrorCall = ex;
        }
    }
}
