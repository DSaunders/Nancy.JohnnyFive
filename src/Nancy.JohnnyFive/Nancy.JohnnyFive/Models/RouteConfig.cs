namespace Nancy.JohnnyFive.Models
{
    using Circuits;
    using Responders;

    public class RouteConfig
    {
        public ICircuit Circuit { get; set; }
        public IResponder Responder { get; set; }

        public RouteConfig WithCircuit(ICircuit circuit)
        {
            this.Circuit = circuit;
            return this;
        }
    }
}
