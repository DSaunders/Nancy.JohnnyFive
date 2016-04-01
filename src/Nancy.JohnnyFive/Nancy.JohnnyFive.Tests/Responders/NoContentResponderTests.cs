namespace Nancy.JohnnyFive.Tests.Responders
{
    using JohnnyFive.Responders;
    using Should;
    using Xunit;

    public class NoContentResponderTests
    {
        [Fact]
        public void Returns_No_Content()
        {
            // Given
            var responder = new NoContentStatusCodeResponder();

            // Then
            var response = responder.GetResponse();

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }
    }
}
