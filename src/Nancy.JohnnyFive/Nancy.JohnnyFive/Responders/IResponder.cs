namespace Nancy.JohnnyFive.Responders
{
    public interface IResponder
    {
        void AfterRequest(Response response);
        Response GetResponse();
    }
}