//public class DdosProtectionCircuit : ICircuit
//{
//    private int _totalHits;
//    private readonly int _hitLimit;

//    private string responseText;

//    private Response _lastGoodResponse;

//    private DateTime _openTime;

//    public CircuitState State { get; private set; }

//    public DdosProtectionCircuit(int hitLimit)
//    {
//        _hitLimit = hitLimit;
//    }

//    public Response BeforeRequest()
//    {

//        if (State == CircuitState.Open)
//        {
//            if (_openTime.AddSeconds(10) > DateTime.Now)
//            {
//                if (responseText == null)
//                    return _lastGoodResponse;
//                else
//                    return responseText;
//            }
//            else
//            {
//                State = CircuitState.Closed;
//                _totalHits = 0;
//            }
//        }

//        return null;
//    }

//    public void AfterRequest(Response response)
//    {
//        _totalHits++;

//        // Hit the limit, save the last good response and close the circuit
//        if (_totalHits == _hitLimit)
//        { 
//            if (responseText == null)
//                _lastGoodResponse = response;
//            State = CircuitState.Open;
//            _openTime = DateTime.Now;
//        }
//    }

//    public Response OnError(Exception ex)
//    {
//        return null;
//    }

//    public DdosProtectionCircuit ReturningLastGoodResponse()
//    {
//        responseText = null;
//        return this;
//    }

//    public DdosProtectionCircuit ReturningText(string text)
//    {
//        responseText = text;
//        return this;
//    }
//}

//public class OpenOnError : ICircuit
//{
//    public CircuitState State { get; private set; }

//    private TimeSpan _openTime;
//    private DateTime _lastErrorTime;

//    private readonly List<Type> _trackedExceptions = new List<Type>(); 

//    public OpenOnError()
//    {
//        _openTime = TimeSpan.FromSeconds(10);
//    }

//    public Response BeforeRequest()
//    {
//        if (State != CircuitState.Open) 
//            return null;

//        // Still open?
//        if (_lastErrorTime.Add(_openTime) > DateTime.Now)
//            return "Open Circuit " + (_lastErrorTime.Add(_openTime) - DateTime.Now).TotalSeconds + "s";

//        State = CircuitState.Closed;
//        return null;
//    }

//    public void AfterRequest(Response response)
//    {
//    }

//    public Response OnError(Exception ex)
//    {
//        if (_trackedExceptions.Contains(ex.GetType()))
//        {
//            _lastErrorTime = DateTime.Now;
//            State = CircuitState.Open;
//        }

//        return null;
//    }

//    public OpenOnError ForException<T>() where T : Exception
//    {
//        _trackedExceptions.Add(typeof(T));
//        return this;
//    }

//    public OpenOnError WithOpenTimeInSeconds(int seconds)
//    {
//        _openTime = TimeSpan.FromSeconds(seconds);
//        return this;
//    }
//}
