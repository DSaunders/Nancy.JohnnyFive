namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using Dates;
    using Models;

    public class OnErrorCircuit : ICircuit
    {
        public CircuitState State { get; set; }

        internal Type ExceptionType { get; private set; }
        internal TimeSpan ShortCircuitTimePeriod { get; private set; }

        private DateTime _shortCirctuitDateTime;

        public OnErrorCircuit()
        {
            ExceptionType = typeof (Exception);
            ShortCircuitTimePeriod = TimeSpan.FromSeconds(10);
        }

        public void AfterRequest(Response response)
        {
        }
        
        public void BeforeRequest(Request request)
        {
            if (State != CircuitState.ShortCircuit)
                return;

            if (_shortCirctuitDateTime.Add(ShortCircuitTimePeriod) <= DateTimeProvider.Now)
                State = CircuitState.Normal;
        }

        public void OnError(Exception ex)
        {
            if (!ExceptionType.IsInstanceOfType(ex))
                return;

            State = CircuitState.ShortCircuit;
            _shortCirctuitDateTime = DateTimeProvider.Now;
        }

        public OnErrorCircuit ForExceptionType<T>() where T : Exception
        {
            this.ExceptionType = typeof (T);
            return this;
        }

        public OnErrorCircuit ShortCircuitsForSeconds(int seconds)
        {
            this.ShortCircuitTimePeriod = TimeSpan.FromSeconds(seconds);
            return this;
        }
    }
}