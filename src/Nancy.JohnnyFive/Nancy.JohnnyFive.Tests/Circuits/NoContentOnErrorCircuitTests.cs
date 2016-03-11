﻿namespace Nancy.JohnnyFive.Tests.Circuits
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
                FakeNow = DateTime.Now.AddSeconds(-DateTime.Now.Second)
            };
        }

        [Fact]
        public void Opens_For_All_Derived_Exceptions_By_Default()
        {
            // Given
            var openOnError = new NoContentOnErrorCircuit();

            // When
            openOnError.OnError(new AccessViolationException());

            // Then
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
            openOnError.OnError(new FileNotFoundException());

            // Then
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
            openOnError.OnError(new SystemException());

            // Then
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
        public void Closes_After_10_Seconds_By_Default()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit();

            // Then
            openOnError.CircuitOpenTime.ShouldEqual(TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Closes_After_Time_Specified_By_Fluent_Interface()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit()
                .WithCircuitOpenTimeInSeconds(90);

            // Then
            openOnError.CircuitOpenTime.ShouldEqual(TimeSpan.FromSeconds(90));
        }

        [Fact]
        public void Closes_Again_After_Open_Time()
        {
            // Setup and trigger an error
            var openOnError =
                new NoContentOnErrorCircuit(_fakeDateTime)
                    .WithCircuitOpenTimeInSeconds(5);

            openOnError.OnError(new FileNotFoundException());

            // Hit route once..
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Open);

            // 4 seconds since error
            _fakeDateTime.FakeNow = _fakeDateTime.FakeNow.AddSeconds(4);
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Open);

            // 5 seconds since error
            _fakeDateTime.FakeNow = _fakeDateTime.FakeNow.AddSeconds(1);
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Closed);
        }

        [Fact]
        public void Returns_Null_When_First_ReClosed()
        {
            // Setup and trigger an error
            var openOnError =
                new NoContentOnErrorCircuit(_fakeDateTime)
                    .WithCircuitOpenTimeInSeconds(5);

            openOnError.OnError(new FileNotFoundException());

            // Hit route once..
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Open);

            // 4 seconds since error
            _fakeDateTime.FakeNow = _fakeDateTime.FakeNow.AddSeconds(4);
            openOnError.BeforeRequest();
            openOnError.State.ShouldEqual(CircuitState.Open);

            // 5 seconds since error
            _fakeDateTime.FakeNow = _fakeDateTime.FakeNow.AddSeconds(1);
            var response = openOnError.BeforeRequest();
            response.ShouldBeNull();
        }

        [Fact]
        public void Exposes_Open_Time_For_Testability()
        {
            // When
            var openOnError = new NoContentOnErrorCircuit()
                .WithCircuitOpenTimeInSeconds(1234);

            // Then
            openOnError.CircuitOpenTime.ShouldEqual(TimeSpan.FromSeconds(1234));
        }

        [Fact]
        public void Returns_No_Content_When_Open()
        {
            // Given
            var openOnError = new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>();

            var firstResponse = new Response { StatusCode = HttpStatusCode.PartialContent };
            var lastGoodResponse = new Response { StatusCode = HttpStatusCode.ImATeapot };

            openOnError.AfterRequest(firstResponse);
            openOnError.AfterRequest(lastGoodResponse);

            // When
            var result = openOnError.OnError(new FileNotFoundException());

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }

        [Fact]
        public void Returns_No_Content_When_Already_Open()
        {
            // Given
            var openOnError = new NoContentOnErrorCircuit()
                    .ForException<FileNotFoundException>();

            var firstResponse = new Response { StatusCode = HttpStatusCode.PartialContent };
            var lastGoodResponse = new Response { StatusCode = HttpStatusCode.ImATeapot };

            openOnError.AfterRequest(firstResponse);
            openOnError.AfterRequest(lastGoodResponse);

            // When
            openOnError.OnError(new FileNotFoundException());
            var result = openOnError.BeforeRequest();

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }
    }
}
