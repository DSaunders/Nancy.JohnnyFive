namespace Nancy.JohnnyFive.Models
{
    using Circuits;
    using Responders;

    internal class RouteConfig
    {
        public ICircuit Circuit { get; set; }
        public IResponder Responder { get; set; }
    }
}
