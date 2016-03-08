namespace Nancy.JohnnyFive.Tests.Fakes
{
    public class FakeNancyModule : NancyModule
    {
        public FakeNancyModule()
        {
            this.Context = new NancyContext();
        }
    }
}