namespace Nancy.JohnnyFive.Tests.Circuits
{
    using System;
    using Dates;
    using JohnnyFive.Circuits;
    using Models;
    using Should;
    using Xunit;

    public class UnderLoadCircuitTests
    {
        private readonly DateTime _testDate;

        public UnderLoadCircuitTests()
        {
            _testDate = new DateTime(2016, 01, 01, 10, 00, 00);
            DateTimeProvider.SetTestDateTime(_testDate);
        }

        [Fact]
        public void Begins_In_NormaL_State()
        {
            // Given
            var circuit = new UnderLoadCircuit();

            // Then
            circuit.State.ShouldEqual(CircuitState.Normal);
        }

        [Fact]
        public void Defaults_Request_Threshold_To_10()
        {
            // Given
            var circuit = new UnderLoadCircuit();

            // Then
            circuit.RequestThreshold.ShouldEqual(10);
        }

        [Fact]
        public void Defaults_Request_Threshold_TimeWindow_To_1_Second()
        {
            // Given
            var circuit = new UnderLoadCircuit();

            // Then
            circuit.RequestThresholdWindow.ShouldEqual(TimeSpan.FromSeconds(1));
        }
        
        [Fact]
        public void Sets_Request_Threshold_In_Seconds_From_Fluent_Interface()
        {
            // Given
            var circuit = new UnderLoadCircuit()
                .WithRequestLimit(20)
                .InSeconds(30);

            // Then
            circuit.RequestThreshold.ShouldEqual(20);
            circuit.RequestThresholdWindow.ShouldEqual(TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void Sets_Request_Threshold_In_Minutes_From_Fluent_Interface()
        {
            // Given
            var circuit = new UnderLoadCircuit()
                .WithRequestLimit(20)
                .InMinutes(30);

            // Then
            circuit.RequestThreshold.ShouldEqual(20);
            circuit.RequestThresholdWindow.ShouldEqual(TimeSpan.FromMinutes(30));
        }

        [Fact]
        public void Sets_Request_Threshold_From_TimeSpan_From_Fluent_Interface()
        {
            // Given
            var circuit = new UnderLoadCircuit()
                .WithRequestLimit(20)
                .InTimeSpan(TimeSpan.FromDays(34));

            // Then
            circuit.RequestThreshold.ShouldEqual(20);
            circuit.RequestThresholdWindow.ShouldEqual(TimeSpan.FromDays(34));
        }

        [Fact]
        public void ShortCircuits_When_Request_Threshold_Exceeded()
        {
            var circuit = new UnderLoadCircuit()
                .WithRequestLimit(3)
                .InSeconds(10);

            circuit.BeforeRequest(null); // One
            circuit.State.ShouldEqual(CircuitState.Normal);
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(5));
            circuit.BeforeRequest(null); // Two
            circuit.State.ShouldEqual(CircuitState.Normal);
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(9));
            circuit.BeforeRequest(null); // Three
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);
        }

        [Fact]
        public void Stays_Short_Circuited_If_Request_Continue()
        {
            var circuit = new UnderLoadCircuit()
                .WithRequestLimit(3)
                .InSeconds(10);

            // Requests every 4 seconds, so there are always 3 requests within the last 
            // 10 seconds
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.Normal);
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(4));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.Normal);
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(8));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(12));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(16));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);
        }

        [Fact]
        public void Returns_To_Notmal_Once_Requests_Are_Outside_Of_Thresold()
        {
            var circuit = new UnderLoadCircuit()
                .WithRequestLimit(3)
                .InSeconds(10);

            circuit.BeforeRequest(null);
            circuit.BeforeRequest(null);
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);
            
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(10));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.Normal);
        }
    }
}