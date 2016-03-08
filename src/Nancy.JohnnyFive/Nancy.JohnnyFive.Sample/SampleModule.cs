namespace Nancy.JohnnyFive.Sample
{
    using System;

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ =>
            {
                this.HasCircuitBreaker(new SampleCircuit());

                if (this.Request.Query["error"] != null)
                    throw new Exception();

                return "Hello, World!";
            };

            Get["/empty"] = _ =>
            {
                if (this.Request.Query["error"] != null)
                    throw new Exception();

                return "Hello, World!";
            };
        }
    }
}