
namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using Models;

    public interface ICircuit
    {
        CircuitState State { get; }

        Response BeforeRequest();
        void AfterRequest(Response response);
        Response OnError(Exception ex);
    }
}

