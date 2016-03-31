namespace Nancy.JohnnyFive.Tests.Fakes
{
    public class FakeNancyModule : NancyModule
    {
        public FakeNancyModule()
        {
            Context = new NancyContext();
        }
    }
}
