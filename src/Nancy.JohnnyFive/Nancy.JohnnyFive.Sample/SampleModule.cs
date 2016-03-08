namespace Nancy.JohnnyFive.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Circuits;

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ =>
            {
                this.HasCircuit(new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>()
                    .WithOpenTimeInSeconds(10));

                this.HasCircuit(new NoContentOnErrorCircuit()
                    .ForException<KeyNotFoundException>()
                    .WithOpenTimeInSeconds(30));
                
                if (this.Request.Query["fileNotFound"] != null)
                    throw new FileNotFoundException();

                if (this.Request.Query["keyNotFound"] != null)
                    throw new KeyNotFoundException();

                return "Hello, World!";
            };
        }
    }
}