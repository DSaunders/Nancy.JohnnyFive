﻿namespace Nancy.JohnnyFive.Circuits
{
    using Base;
    using Dates;

    public class LastGoodResponseOnErrorCircuit : OnErrorCircuitBase
    {
        private Response _lastGoodResponse;

        public LastGoodResponseOnErrorCircuit() : this(new CurrentDateTime())
        {
        }

        internal LastGoodResponseOnErrorCircuit(ICurrentDateTime dateTime) : base(dateTime)
        {
        }

        public override void AfterRequest(Response response)
        {
            _lastGoodResponse = response;
        }

        protected override Response ResponseWhenOpen()
        {
            return _lastGoodResponse ?? HttpStatusCode.NoContent;
        }
    }
}
