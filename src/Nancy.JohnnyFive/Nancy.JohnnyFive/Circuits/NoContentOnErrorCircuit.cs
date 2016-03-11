namespace Nancy.JohnnyFive.Circuits
{
    using Base;
    using Dates;

    public class NoContentOnErrorCircuit : OnErrorCircuitBase
    {
        public NoContentOnErrorCircuit()
        {
        }

        internal NoContentOnErrorCircuit(ICurrentDateTime dateTime) : base(dateTime)
        {
        }

        protected override Response ResponseWhenOpen()
        {
            return HttpStatusCode.NoContent;
        }
    }
}
