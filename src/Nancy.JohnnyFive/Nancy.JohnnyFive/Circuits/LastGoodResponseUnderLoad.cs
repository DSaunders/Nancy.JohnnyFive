namespace Nancy.JohnnyFive.Circuits
{
    using Base;
    using Dates;

    public class LastGoodResponseUnderLoad : UnderLoadCircuitBase
    {
        private Response _lastGoodResponse;

        public LastGoodResponseUnderLoad()
        {
            
        }

        public LastGoodResponseUnderLoad(ICurrentDateTime dateTime) : base(dateTime)
        {
            
        }

        public override void AfterRequest(Response response)
        {
            _lastGoodResponse = response;
        }

        protected override Response OpenResponse()
        {
            return _lastGoodResponse ?? HttpStatusCode.NoContent;
        }
    }
}
