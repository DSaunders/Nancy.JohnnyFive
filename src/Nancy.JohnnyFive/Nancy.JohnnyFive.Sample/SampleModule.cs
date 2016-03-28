namespace Nancy.JohnnyFive.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ =>
            {
                //this.CanShortCircuit(new NoContentOnErrorCircuit()
                //    .ForException<FileNotFoundException>()
                //    .WithCircuitOpenTimeInSeconds(10));

                //this.CanShortCircuit(new NoContentOnErrorCircuit()
                //    .ForException<KeyNotFoundException>()
                //    .WithCircuitOpenTimeInSeconds(30));
                
                //if (this.Request.Query["fileNotFound"] != null)
                //    throw new FileNotFoundException();

                //if (this.Request.Query["keyNotFound"] != null)
                //    throw new KeyNotFoundException();

                return "Hello, World!"; 
            };

            Get["/underload"] = _ =>
            {

                //// Something like this would be nicer
                //this.CanShortCircuit()
                //    .UsingHandler(new ExceptionHandler<FileNotFoundException>())
                //    .Returning(new LastGoodResponseReturner());

                //this.CanShortCircuit()
                //    .UsingHandler(new UnderLoadHandler().ForRequests()
                //    .Returning(new StatusCodeReturner(HttpStatusCode.NoContent))
                //    .ForSeconds(10);




                //this.CanShortCircuit()
                //    // Each one of these could get called for each before/after/error and returns the latest
                //    // This would implement an interface that you could derive from to create custom
                //    .UsingHandler(new ExceptionHandler<FileNotFoundException>())
                //    // Something in the before pipeline (or abstracted to the store) returns this when the circuit is closed
                //    // This would also have to be called for above pipelines in order to get last response etc
                //    .Returning(new LastGoodResponseReturner());



                //this.CanShortCircuit(new NoContentUnderLoadCircuit()
                //    .WithRequestSampleTimeInSeconds(10)
                //    .WithRequestThreshold(5));

                return "Under load " + DateTime.Now;
            };
        }
    }
}