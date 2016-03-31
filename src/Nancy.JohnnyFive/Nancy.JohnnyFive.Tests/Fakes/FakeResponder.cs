namespace Nancy.JohnnyFive.Tests.Fakes
{
    using JohnnyFive.Responders;

    public class FakeResponder : IResponder
    {
        public Response AfterRequestCall { get; set; }
        public Response FakeResponse { get; set; }

        public void AfterRequest(Response response)
        {
            AfterRequestCall = response;
        }

        public Response GetResponse()
        {
            return FakeResponse;
        }
    }
}