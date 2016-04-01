namespace Nancy.JohnnyFive.Sample
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using Circuits;
    using Responders;
    using Samples;

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ =>
            {
                this.CanShortCircuit()
                    .WithCircuit(new UnderLoadCircuit().WithRequestLimit(3).InSeconds(10))
                    .WithResponder(new TextResponder())
                    .WithCallback(() => Debug.WriteLine("Request limit hit!"));

                return "Hello, World!";
            };

            Get["/canerror"] = _ =>
            {
                this.CanShortCircuit()
                    .WithCircuit(new OnErrorCircuit()
                        .ForExceptionType<SqlException>()
                        .ShortCircuitsForSeconds(20))
                    .WithResponder(new LastGoodResponseResponder());

                if (this.Request.Query["error"] != null)
                    throw new Exception("It broke");

                return "Hello, World! " + DateTime.Now;
            };
        }
    }
}