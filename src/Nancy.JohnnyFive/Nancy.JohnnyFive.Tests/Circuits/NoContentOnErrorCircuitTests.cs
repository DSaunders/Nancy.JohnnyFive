namespace Nancy.JohnnyFive.Tests.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Fakes;
    using JohnnyFive.Circuits;
    using Models;
    using Should;
    using Xunit;

    public class NoContentOnErrorCircuitTests
    {
        private readonly FakeCurrentDateTime _fakeDateTime;

        public NoContentOnErrorCircuitTests()
        {
            _fakeDateTime = new FakeCurrentDateTime
            {
                FakeDateTime = DateTime.Now.AddSeconds(-DateTime.Now.Second)
            };
        }

        [Fact]
        public void Opens_For_All_Derived_Exceptions_By_Default()
        {
            // Given
            var openOnError = new NoContentOnErrorCircuit();

            // When
            var result = openOnError.OnError(new AccessViolationException());

            // Then
            result.ShouldBeNull();
            openOnError.State.ShouldEqual(CircuitState.Open);
        }

        [Fact]
        public void Opens_For_Matching_Error()
        {
            // Given
            var openOnError =
                new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>();

            // When
            var result = openOnError.OnError(new FileNotFoundException());

            // Then
            result.ShouldBeNull();
            openOnError.State.ShouldEqual(CircuitState.Open);
        }

        [Fact]
        public void Does_Not_Open_For_Non_Matching_Error()
        {
            // Given
            var openOnError =
                new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>();

            // When
            var result = openOnError.OnError(new SystemException());

            // Then
            result.ShouldBeNull();
            openOnError.State.ShouldEqual(CircuitState.Closed);
        }

        [Fact]
        public void Exposes_Exception_For_Testability()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit()
                .ForException<KeyNotFoundException>();

            // Then
            openOnError.TrackedException.ShouldEqual(typeof(KeyNotFoundException));
        }

        [Fact]
        public void Returns_No_Content_When_Open()
        {
            // Given
            var openOnError =
                new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>();
            openOnError.OnError(new FileNotFoundException());

            // When
            var result = openOnError.BeforeRequest();

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }

        [Fact]
        public void Closes_After_10_Seconds_By_Default()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit();

            // Then
            openOnError.OpenTime.ShouldEqual(TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Closes_After_Time_Specified_By_Fluent_Interface()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit()
                .WithOpenTimeInSeconds(90);

            // Then
            openOnError.OpenTime.ShouldEqual(TimeSpan.FromSeconds(90));
        }

        [Fact]
        public void Closes_Again_After_Open_Time()
        {
            // Setup and trigger an error
            var openOnError =
                new NoContentOnErrorCircuit(_fakeDateTime)
                    .WithOpenTimeInSeconds(5);

            openOnError.OnError(new FileNotFoundException());

            // Hit route once..
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Open);

            // 5 seconds since error
            _fakeDateTime.FakeDateTime = _fakeDateTime.FakeDateTime.AddSeconds(5);
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Open);

            // 6 seconds since error
            _fakeDateTime.FakeDateTime = _fakeDateTime.FakeDateTime.AddSeconds(1);
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Closed);
        }

        [Fact]
        public void Exposes_Open_Time_For_Testability()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit()
                .WithOpenTimeInSeconds(1234);

            // Then
            openOnError.OpenTime.ShouldEqual(TimeSpan.FromSeconds(1234));
        }
    }
}
