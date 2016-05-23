namespace Nancy.JohnnyFive.Circuits
{
    using System;
    using System.Linq.Expressions;
    using Dates;
    using Models;

    public class OnErrorCircuit : ICircuit
    {
        public CircuitState State { get; set; }

        internal Type ExceptionType { get; private set; }
        internal TimeSpan ShortCircuitTimePeriod { get; private set; }

        private dynamic ExceptionFunction { get; set; }
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

        public void OnError<T>(T ex) where T : Exception
        {
            if (!ExceptionType.IsInstanceOfType(ex))
                return;

            if (ExceptionFunction != null && 
                ExceptionFunction.Invoke(ex) == false)
                return;
            
            State = CircuitState.ShortCircuit;
            _shortCirctuitDateTime = DateTimeProvider.Now;
        }

        public OnErrorCircuit ForExceptionType<T>() where T : Exception
        {
            this.ExceptionType = typeof (T);
            return this;
        }

        public OnErrorCircuit ForExceptionType<T>(Expression<Func<T, bool>> func) where T : Exception
        {
            this.ExceptionType = typeof(T);
            this.ExceptionFunction = func.Compile();
            return this;
        }

        public OnErrorCircuit ShortCircuitsForSeconds(int seconds)
        {
            this.ShortCircuitTimePeriod = TimeSpan.FromSeconds(seconds);
            return this;
        }
    }
}