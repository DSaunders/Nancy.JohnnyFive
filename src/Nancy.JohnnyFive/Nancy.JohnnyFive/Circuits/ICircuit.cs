namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using Models;

    public interface ICircuit
    {
        CircuitState State { get; set; }
        void AfterRequest(Response response);
        void BeforeRequest(Request request);
        void OnError<T>(T ex) where T : Exception;
    }
}