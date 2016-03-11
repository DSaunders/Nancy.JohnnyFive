namespace Nancy.JohnnyFive.Circuits.Base
{
    using System;
    using System.Collections.Generic;
    using Dates;
    using Models;

    public abstract class UnderLoadCircuitBase : ICircuit
    {
        private readonly ICurrentDateTime _dateTime;
        private readonly Queue<DateTime> _requestsRollingWindow;
        private DateTime _circtuitOpenedTime;

        public CircuitState State { get; private set; }
        public TimeSpan CircuitOpenTime { get; set; }

        public TimeSpan SampleTime { get; set; }
        public int RequestThreshold  { get; set; }

        protected UnderLoadCircuitBase() : this(new CurrentDateTime())
        {
        }

        // Internal constructor for testing datetime
        internal UnderLoadCircuitBase(ICurrentDateTime dateTime)
        {
            _dateTime = dateTime;
            _requestsRollingWindow = new Queue<DateTime>();

            CircuitOpenTime = TimeSpan.FromSeconds(10);
            SampleTime = TimeSpan.FromSeconds(10);
            RequestThreshold = 100;
        }

        public Response BeforeRequest()
        {
            var now = _dateTime.Now;

            // Already open, just return (or close the circuit if time is up)
            if (State == CircuitState.Open)
            {
                if (_circtuitOpenedTime.Add(CircuitOpenTime) <= now)
                {
                    State = CircuitState.Closed;
                    return null;
                }

                return OpenResponse();
            }
            
            // Queue this request
            _requestsRollingWindow.Enqueue(now);

            // Clear out the buffer of anything outside of the sliding time window
            while (_requestsRollingWindow.Peek() < (now - SampleTime))
                _requestsRollingWindow.Dequeue();

            // Requests under threshold
            if (_requestsRollingWindow.Count < RequestThreshold) 
                return null;

            // Requests over threshold
            _circtuitOpenedTime = now;
            State = CircuitState.Open;
            _requestsRollingWindow.Clear();
            return OpenResponse();
        }

        public virtual void AfterRequest(Response response)
        {
        }

        public Response OnError(Exception ex)
        {
            return null;
        }

        // Fluent builders
        public UnderLoadCircuitBase WithRequestSampleTimeInSeconds(int sampleTime)
        {
            SampleTime = TimeSpan.FromSeconds(sampleTime);
            return this;
        }

        public UnderLoadCircuitBase WithRequestThreshold(int threshold)
        {
            RequestThreshold = threshold;
            return this;
        }

        public UnderLoadCircuitBase WithCircuitOpenTimeInSeconds(int seconds)
        {
            CircuitOpenTime = TimeSpan.FromSeconds(seconds);
            return this;
        }

        protected abstract Response OpenResponse();
    }
}
