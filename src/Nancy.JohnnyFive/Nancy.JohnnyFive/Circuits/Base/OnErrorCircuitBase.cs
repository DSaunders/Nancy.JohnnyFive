namespace Nancy.JohnnyFive.Circuits.Base
{
    using System;
    using Dates;
    using Models;

    /// <summary>
    /// Opens the circuit when a given exception (or derived exception) is thrown.
    /// Closes after a configurable period of time
    /// </summary>
    public abstract class OnErrorCircuitBase : ICircuit
    {
        protected ICurrentDateTime DateTime;
        protected DateTime LastErrorTime;
        
        public CircuitState State { get; private set; }
        public Type TrackedException { get; set; }
        public TimeSpan CircuitOpenTime { get; set; }

        protected OnErrorCircuitBase() : this(new CurrentDateTime())
        {
        }

        // Internal constructor for testing datetime
        protected OnErrorCircuitBase(ICurrentDateTime dateTime)
        {
            DateTime = dateTime;
            TrackedException = typeof(Exception);
            CircuitOpenTime = TimeSpan.FromSeconds(10);
        }

        public Response BeforeRequest()
        {
            if (State != CircuitState.Open)
                return null;

            // Return 'NoContent' until time is up
            if (LastErrorTime.Add(CircuitOpenTime) > DateTime.Now)
                return ResponseWhenOpen();
            
            // Time is up, close the circuit again
            State = CircuitState.Closed;
            return null;
        }

        public virtual void AfterRequest(Response response) { }

        public Response OnError(Exception ex)
        {
            // If the exception does not derive from the type specified, ignore it
            if (!TrackedException.IsInstanceOfType(ex)) 
                return null;

            State = CircuitState.Open;
            LastErrorTime = DateTime.Now;
            return ResponseWhenOpen();
        }

        // Fluent builders
        public OnErrorCircuitBase ForException<T>() where T : Exception
        {
            TrackedException = typeof(T);
            return this;
        }

        public OnErrorCircuitBase WithCircuitOpenTimeInSeconds(int seconds)
        {
            CircuitOpenTime = TimeSpan.FromSeconds(seconds);
            return this;
        }

        protected abstract Response ResponseWhenOpen();
    }
}
