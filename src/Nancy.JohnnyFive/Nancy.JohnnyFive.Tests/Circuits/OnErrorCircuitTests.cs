namespace Nancy.JohnnyFive.Tests.Circuits
{
    using System;
    using System.IO;
    using Dates;
    using JohnnyFive.Circuits;
    using Models;
    using Should;
    using Xunit;

    public class OnErrorCircuitTests : IDisposable
    {
        private readonly DateTime _testDate;

        public OnErrorCircuitTests()
        {
            _testDate = new DateTime(2016, 01, 01, 10, 00, 00);
            DateTimeProvider.SetTestDateTime(_testDate);
        }

        [Fact]
        public void Begins_In_Normal_State()
        {
            // Given
            var circuit = new OnErrorCircuit();
            
            // Then
            circuit.State.ShouldEqual(CircuitState.Normal);
        }

        [Fact]
        public void Defaults_Exception_To_Base_Class()
        {
            // When
            var circuit = new OnErrorCircuit();

            // Then
            circuit.ExceptionType.ShouldEqual(typeof(Exception));
        }

        [Fact]
        public void Sets_Exception_BaseType_Using_Fluent_Interface()
        {
            // When
            var circuit = new OnErrorCircuit()
                .ForExceptionType<FileNotFoundException>();

            // Then
            circuit.ExceptionType.ShouldEqual(typeof(FileNotFoundException));
        }

        [Fact]
        public void OnError_Does_Not_ShortCircuit_If_Not_Derived_Exception()
        {
            // Given
            var circuit = new OnErrorCircuit()
                .ForExceptionType<ArithmeticException>();

            // When
            circuit.OnError(new NullReferenceException());

            // Then
            circuit.State.ShouldEqual(CircuitState.Normal);
        }

        [Fact]
        public void OnError_ShortCircuits_If_Derived_Exception()
        {
            // Given
            var circuit = new OnErrorCircuit()
                .ForExceptionType<ArithmeticException>();

            // When
            circuit.OnError(new OverflowException());

            // Then
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);
        }

        [Fact]
        public void Defaults_ShortCircuitTimePeriod_To_10_Seconds()
        {
            // Given
            var circuit = new OnErrorCircuit();

            // Then
            circuit.ShortCircuitTimePeriod.ShouldEqual(TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Sets_ShortCircuitTimePeriod_Using_Fluent_Interface()
        {
            // Given
            var circuit = new OnErrorCircuit()
                .ShortCircuitsForSeconds(90);

            // Then
            circuit.ShortCircuitTimePeriod.ShouldEqual(TimeSpan.FromSeconds(90));
        }

        [Fact]
        public void Sets_Circuit_To_Normal_After_Time_Period()
        {
            // Given
            var circuit = new OnErrorCircuit()
                .ShortCircuitsForSeconds(10);

            // When
            circuit.OnError(new OverflowException());

            // Then
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);

            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(9));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);

            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(10));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.Normal);
        }

        [Fact]
        public void OnError_Resets_Time_Period_On_Each_Error()
        {
            // Given
            var circuit = new OnErrorCircuit()
                .ShortCircuitsForSeconds(10);

            // When
            circuit.OnError(new OverflowException());

            // Then
            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(5));
            circuit.OnError(new OverflowException());
            circuit.BeforeRequest(null);

            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(10));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.ShortCircuit);

            DateTimeProvider.SetTestDateTime(_testDate.AddSeconds(15));
            circuit.BeforeRequest(null);
            circuit.State.ShouldEqual(CircuitState.Normal);
        }

        public void Dispose()
        {
            DateTimeProvider.ExitTestMode();
        }
    }
}