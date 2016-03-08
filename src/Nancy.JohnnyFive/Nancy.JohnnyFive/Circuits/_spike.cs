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
