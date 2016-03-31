namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using System.Collections.Generic;
    using Dates;
    using Models;

    public class UnderLoadCircuit : ICircuit
    {
        public CircuitState State { get; set; }

        internal int RequestThreshold { get; set; }
        internal TimeSpan RequestThresholdWindow { get; set; }
        
        private readonly Queue<DateTime> _slidingWindow;

        public UnderLoadCircuit()
        {
            RequestThreshold = 10;
            RequestThresholdWindow = TimeSpan.FromSeconds(1);
            _slidingWindow = new Queue<DateTime>();
        }

        public void AfterRequest(Response response)
        {
        }

        public void BeforeRequest(Request request)
        {
            _slidingWindow.Enqueue(DateTimeProvider.Now);

            while (_slidingWindow.Peek() <= DateTimeProvider.Now.Subtract(RequestThresholdWindow))
                _slidingWindow.Dequeue();

            State = _slidingWindow.Count >= RequestThreshold 
                ? CircuitState.ShortCircuit 
                : CircuitState.Normal;
        }

        public void OnError(Exception ex)
        {
        }


        // Fluent interface for configuring
        public UnderLoadCircuitWithRequestCount WithRequestLimit(int requestLimit)
        {
            this.RequestThreshold = requestLimit;
            return new UnderLoadCircuitWithRequestCount(this);
        }

        public class UnderLoadCircuitWithRequestCount
        {
            private readonly UnderLoadCircuit _circuit;

            public UnderLoadCircuitWithRequestCount(UnderLoadCircuit circuit)
            {
                _circuit = circuit;
            }

            public UnderLoadCircuit InSeconds(int seconds)
            {
                _circuit.RequestThresholdWindow = TimeSpan.FromSeconds(seconds);
                return _circuit;
            }

            public UnderLoadCircuit InMinutes(int minutes)
            {
                _circuit.RequestThresholdWindow = TimeSpan.FromMinutes(minutes);
                return _circuit;
            }

            public UnderLoadCircuit InTimeSpan(TimeSpan timeSpan)
            {
                _circuit.RequestThresholdWindow = timeSpan;
                return _circuit;
            }
        }
    }
}