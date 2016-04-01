namespace Nancy.JohnnyFive.Responders
{
    public class NoContentStatusCodeResponder : IResponder
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