namespace Nancy.JohnnyFive.Circuits
{
    using Base;
    using Dates;

    public class NoContentUnderLoadCircuit : UnderLoadCircuitBase
    {
        public NoContentUnderLoadCircuit()
        {
            
        }

        public NoContentUnderLoadCircuit(ICurrentDateTime dateTime) : base(dateTime)
        {
            
        }

        protected override Response OpenResponse()
        {
            return HttpStatusCode.NoContent;
        }
    }
}
