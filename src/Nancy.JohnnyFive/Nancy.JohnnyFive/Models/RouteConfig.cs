namespace Nancy.JohnnyFive.Models
{
    using System;
    using Circuits;
    using Responders;

    public class RouteConfig
    {
        public ICircuit Circuit { get; set; }
        public IResponder Responder { get; set; }
        public Action OnShortCircuitCallback { get; set; }

        public RouteConfig WithCircuit(ICircuit circuit)
        {
            this.Circuit = circuit;
            return this;
        }

        public RouteConfig WithResponder(IResponder responder)
        {
            this.Responder = responder;
            return this;
        }

        public RouteConfig WithCallback(Action callback)
        {
            this.OnShortCircuitCallback = callback;
            return this;
        }
    }
}
