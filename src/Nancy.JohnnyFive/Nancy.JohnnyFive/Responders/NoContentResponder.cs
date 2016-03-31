namespace Nancy.JohnnyFive.Responders
{
    public class NoContentResponder : IResponder
    {
        public void AfterRequest(Response response)
        {
            
        }

        public Response GetResponse()
        {
            return HttpStatusCode.NoContent;
        }
    }
}