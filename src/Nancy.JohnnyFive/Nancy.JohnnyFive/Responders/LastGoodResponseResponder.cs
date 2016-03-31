namespace Nancy.JohnnyFive.Responders
{
    public class LastGoodResponseResponder : IResponder
    {
        private Response _lastGoodResponse;

        public void AfterRequest(Response response)
        {
            _lastGoodResponse = response;
        }

        public Response GetResponse()
        {
            return _lastGoodResponse ?? HttpStatusCode.NoContent;
        }
    }
}