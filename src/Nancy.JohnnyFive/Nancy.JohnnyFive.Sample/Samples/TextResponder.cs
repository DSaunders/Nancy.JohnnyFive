namespace Nancy.JohnnyFive.Sample.Samples
{
    using Responders;

    public class TextResponder : IResponder
    {
        public void AfterRequest(Response response)
        {
        }

        public Response GetResponse()
        {
            return "Short circuited";
        }
    }
}