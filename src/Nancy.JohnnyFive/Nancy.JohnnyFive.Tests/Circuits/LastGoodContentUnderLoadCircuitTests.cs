namespace Nancy.JohnnyFive.Tests.Circuits
{
    using System;
    using Fakes;
    using JohnnyFive.Circuits;
    using Models;
    using Should;
    using Xunit;

    public class LastGoodResponseUnderLoadTests
    {
        private readonly FakeCurrentDateTime _fakeDateTime;

        public LastGoodResponseUnderLoadTests()
        {
            _fakeDateTime = new FakeCurrentDateTime
            {
                FakeNow = DateTime.Now.AddSeconds(-DateTime.Now.Second)
            };
        }

        [Fact]
        public void SampleTime_Defaults_To_10_Seconds()
        {
            // When
            var circuit = new LastGoodResponseUnderLoad();

            // Then
            circuit.SampleTime.TotalSeconds.ShouldEqual(10);
        }

        [Fact]
        public void Sets_SampleTime_Using_Fluent_Interface()
        {
            // When
            var circuit = new LastGoodResponseUnderLoad()
                .WithRequestSampleTimeInSeconds(40);

            // Then
            circuit.SampleTime.TotalSeconds.ShouldEqual(40);
        }

        [Fact]
        public void RequestThreshold_Defaults_To_100_Requests()
        {
            // When
            var circuit = new LastGoodResponseUnderLoad();

            // Then
            circuit.RequestThreshold.ShouldEqual(100);
        }

        [Fact]
        public void Sets_RequestThreshold_Through_Fluent_Interface()
        {
            // When
            var circuit = new LastGoodResponseUnderLoad()
                .WithRequestThreshold(50);

            // Then
            circuit.RequestThreshold.ShouldEqual(50);
        }

        [Fact]
        public void CircuitOpenTime_Defaults_To_10_Seconds()
        {
            // When
            var circuit = new LastGoodResponseUnderLoad();

            // Then
            circuit.CircuitOpenTime.TotalSeconds.ShouldEqual(10);
        }

        [Fact]
        public void Sets_CircuitOpenTime_Through_Fluent_Interface()
        {
            // When
            var circuit = new LastGoodResponseUnderLoad()
                .WithCircuitOpenTimeInSeconds(90);

            // Then
            circuit.CircuitOpenTime.TotalSeconds.ShouldEqual(90);
        }

        [Fact]
        public void Opens_When_Requests_In_Time_Exceed_Threshold()
        {
            // Setup
            var circuit =
                new LastGoodResponseUnderLoad(_fakeDateTime)
                    .WithRequestThreshold(4)
                    .WithRequestSampleTimeInSeconds(10);


            // Make requests until circuit opens
            circuit.BeforeRequest();
            circuit.State.ShouldEqual(CircuitState.Closed);

            MakeRequestAfterSeconds(circuit, 2);
            circuit.State.ShouldEqual(CircuitState.Closed);

            MakeRequestAfterSeconds(circuit, 2);
            circuit.State.ShouldEqual(CircuitState.Closed);

            MakeRequestAfterSeconds(circuit, 2);
            circuit.State.ShouldEqual(CircuitState.Open);
        }

        [Fact]
        public void Returns_Last_Good_Response_When_First_Opened()
        {
            // Given
            var firstResponse = new Response { StatusCode = HttpStatusCode.MultipleChoices };
            var lastGoodResponse = new Response { StatusCode = HttpStatusCode.ImATeapot };

            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithRequestThreshold(1);

            // When
            circuit.AfterRequest(firstResponse);
            circuit.AfterRequest(lastGoodResponse);
            var openResponse = circuit.BeforeRequest();

            // Then
            openResponse.ShouldEqual(lastGoodResponse);
        }

        [Fact]
        public void Returns_No_Content_When_Open_And_No_Good_LastResponse()
        {
            // Given
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithRequestThreshold(1);

            // When
            var openResponse = circuit.BeforeRequest();

            // Then
            openResponse.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }

        [Fact]
        public void Returns_Last_Good_Response_When_Already_Open()
        {
            // Given
            var lastGoodResponse = new Response { StatusCode = HttpStatusCode.ImATeapot };
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithRequestThreshold(1);

            // When
            circuit.AfterRequest(lastGoodResponse);
            circuit.BeforeRequest();
            circuit.BeforeRequest();
            var response = circuit.BeforeRequest();

            // Then
            response.ShouldEqual(lastGoodResponse);
        }

        [Fact]
        public void Does_Not_Open_When_Requests_Exceed_Threshold_Outside_Of_Time_Sampling_Window()
        {
            // Setup
            var circuit =
                new LastGoodResponseUnderLoad(_fakeDateTime)
                    .WithRequestThreshold(4)
                    .WithRequestSampleTimeInSeconds(10);


            circuit.BeforeRequest();
            circuit.State.ShouldEqual(CircuitState.Closed);

            MakeRequestAfterSeconds(circuit, 4);
            circuit.State.ShouldEqual(CircuitState.Closed);

            MakeRequestAfterSeconds(circuit, 4);
            circuit.State.ShouldEqual(CircuitState.Closed);

            MakeRequestAfterSeconds(circuit, 4);
            circuit.State.ShouldEqual(CircuitState.Closed);
        }

        [Fact]
        public void Returns_Null_When_Closed()
        {
            // Given
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime);

            // When
            var response = circuit.BeforeRequest();

            // Then
            response.ShouldBeNull();
        }
        
        [Fact]
        public void ReOpens_After_Circuit_Closed_Time()
        {
            // Set up a circuit
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithRequestThreshold(3)
                .WithRequestSampleTimeInSeconds(5)
                .WithCircuitOpenTimeInSeconds(15);

            // Open the circuit
            MakeRequestAfterSeconds(circuit, 0);
            MakeRequestAfterSeconds(circuit, 0);
            MakeRequestAfterSeconds(circuit, 0);
            circuit.State.ShouldEqual(CircuitState.Open);

            MakeRequestAfterSeconds(circuit, 14);
            circuit.State.ShouldEqual(CircuitState.Open);

            MakeRequestAfterSeconds(circuit, 1);
            circuit.State.ShouldEqual(CircuitState.Closed);
        }

        [Fact]
        public void Ignores_Requests_When_Closed()
        {
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithRequestThreshold(3)
                .WithRequestSampleTimeInSeconds(15)
                .WithCircuitOpenTimeInSeconds(10);

            // Open the circuit
            MakeRequestAfterSeconds(circuit, 0);
            MakeRequestAfterSeconds(circuit, 5);
            MakeRequestAfterSeconds(circuit, 10);
            circuit.State.ShouldEqual(CircuitState.Open);

            // Make requests when open, should not be counted as are just 
            // returned immediately
            MakeRequestAfterSeconds(circuit, 2);
            MakeRequestAfterSeconds(circuit, 2);
            MakeRequestAfterSeconds(circuit, 1);

            // Time up, should be closed again (regardless of requests made when open)
            MakeRequestAfterSeconds(circuit, 10);
            circuit.State.ShouldEqual(CircuitState.Closed);
        }

        [Fact]
        public void Returns_Null_When_First_ReClosed()
        {
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithCircuitOpenTimeInSeconds(10)
                .WithRequestThreshold(2);

            // Open circuit
            circuit.BeforeRequest();
            circuit.BeforeRequest();

            // Re-open
            var response = MakeRequestAfterSeconds(circuit, 10);
            response.ShouldBeNull();
        }

        [Fact]
        public void Can_Open_More_Than_Once()
        {
            // Set up a circuit
            var circuit = new LastGoodResponseUnderLoad(_fakeDateTime)
                .WithRequestThreshold(2)
                .WithRequestSampleTimeInSeconds(10)
                .WithCircuitOpenTimeInSeconds(10);

            // Open the circuit
            MakeRequestAfterSeconds(circuit, 0);
            MakeRequestAfterSeconds(circuit, 5);
            circuit.State.ShouldEqual(CircuitState.Open);

            // Time up, should be closed again
            MakeRequestAfterSeconds(circuit, 10);
            circuit.State.ShouldEqual(CircuitState.Closed);

            // Trigger it again and open it
            MakeRequestAfterSeconds(circuit, 0);
            MakeRequestAfterSeconds(circuit, 5);
            circuit.State.ShouldEqual(CircuitState.Open);
        }

        [Fact]
        public void On_Error_Does_Nothing()
        {
            // Given
            var circuit = new LastGoodResponseUnderLoad();

            // When
            var response = circuit.OnError(new Exception());

            // Then
            response.ShouldBeNull();
        }


        private Response MakeRequestAfterSeconds(ICircuit circuit, int seconds)
        {
            _fakeDateTime.FakeNow = _fakeDateTime.FakeNow.AddSeconds(seconds);
            return circuit.BeforeRequest();
        }
    }
}
