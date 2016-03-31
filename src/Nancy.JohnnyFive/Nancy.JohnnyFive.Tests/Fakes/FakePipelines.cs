namespace Nancy.JohnnyFive.Tests.Fakes
{
    using Bootstrapper;

    public class FakePipelines : IPipelines
    {
        public BeforePipeline BeforeRequest { get; set; }
        public AfterPipeline AfterRequest { get; set; }
        public ErrorPipeline OnError { get; set; }

        public FakePipelines()
        {
            BeforeRequest = new BeforePipeline();
            AfterRequest = new AfterPipeline();
            OnError = new ErrorPipeline();
        }
    }
}
