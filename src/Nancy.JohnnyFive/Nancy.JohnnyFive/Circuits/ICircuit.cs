namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using Models;

    public interface ICircuit
    {
        CircuitState State { get; set; }
        void AfterRequest(Response response);
        void BeforeRequest(Request request);
        void OnError(Exception ex);
    }
}