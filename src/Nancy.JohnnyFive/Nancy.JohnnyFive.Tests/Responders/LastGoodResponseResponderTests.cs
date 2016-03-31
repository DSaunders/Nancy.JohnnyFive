namespace Nancy.JohnnyFive.Tests.Responders
{
    using JohnnyFive.Responders;
    using Should;
    using Xunit;

    public class LastGoodResponseResponderTests
    {
        [Fact]
        public void Returns_No_Content_If_No_Last_Good_Response()
        {
            // Given
            var responder = new LastGoodResponseResponder();

            // When
            var response = responder.GetResponse();

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }

        [Fact]
        public void Returns_Last_Good_Response()
        {
            // Given
            var responder = new LastGoodResponseResponder();
            var oldResponse = new Response();
            var goodResponse = new Response();

            responder.AfterRequest(oldResponse);
            responder.AfterRequest(goodResponse);

            // When
            var response = responder.GetResponse();

            // Then
            response.ShouldEqual(goodResponse);
        }
    }
}