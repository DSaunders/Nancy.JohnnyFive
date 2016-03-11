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
                this.CanShortCircuit(new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>()
                    .WithCircuitOpenTimeInSeconds(10));

                this.CanShortCircuit(new NoContentOnErrorCircuit()
                    .ForException<KeyNotFoundException>()
                    .WithCircuitOpenTimeInSeconds(30));
                
                if (this.Request.Query["fileNotFound"] != null)
                    throw new FileNotFoundException();

                if (this.Request.Query["keyNotFound"] != null)
                    throw new KeyNotFoundException();

                return "Hello, World!";
            };

            Get["/underload"] = _ =>
            {
                this.CanShortCircuit(new NoContentUnderLoadCircuit()
                    .WithRequestSampleTimeInSeconds(10)
                    .WithRequestThreshold(5));

                return "Under load " + DateTime.Now;
            };
        }
    }
}