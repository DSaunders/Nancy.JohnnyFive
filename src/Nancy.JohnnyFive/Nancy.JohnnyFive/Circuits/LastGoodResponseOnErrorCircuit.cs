namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using Dates;
    using Models;

    public class LastGoodResponseOnErrorCircuit : ICircuit
    {
        private readonly ICurrentDateTime _dateTime;
        private DateTime _lastErrorTime;
        private Response _lastGoodResponse;


        public CircuitState State { get; private set; }
        public Type TrackedException { get; private set; }
        public TimeSpan OpenTime { get; set; }

        public LastGoodResponseOnErrorCircuit() : this(new CurrentDateTime())
        {
        }

        // Internal constructor for testing datetime
        internal LastGoodResponseOnErrorCircuit(ICurrentDateTime dateTime)
        {
            _dateTime = dateTime;
            TrackedException = typeof(Exception);
            OpenTime = TimeSpan.FromSeconds(10);
        }

        public Response BeforeRequest()
        {
            if (State != CircuitState.Open)
                return null;

            // Return 'NoContent' until time is up
            if (_lastErrorTime.Add(OpenTime) >= _dateTime.Now)
                return _lastGoodResponse ?? HttpStatusCode.NoContent;
            
            // Time is up, close the circuit again
            State = CircuitState.Closed;
            return null;
        }

        public void AfterRequest(Response response)
        {
            _lastGoodResponse = response;
        }

        public Response OnError(Exception ex)
        {
            // If the exception derives from the type specified then open
            if (TrackedException.IsInstanceOfType(ex))
            {
                State = CircuitState.Open;
                _lastErrorTime = _dateTime.Now;
            }
                
            return null;
        }


        // Fluent builders
        public LastGoodResponseOnErrorCircuit ForException<T>() where T : Exception
        {
            TrackedException = typeof(T);
            return this;
        }

        public LastGoodResponseOnErrorCircuit WithOpenTimeInSeconds(int seconds)
        {
            OpenTime = TimeSpan.FromSeconds(seconds);
            return this;
        }
    }
}
