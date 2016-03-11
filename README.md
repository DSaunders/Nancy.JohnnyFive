# Nancy.JohnnyFive
Allows you to short-circuit Nancy routes in the event of an error or excess load.

```csharp

Get["/"] = _ => 
{
    // If an exception is thrown, short-circuits the route for 10 seconds, 
    // returning the last good response instead
    this.CanShortCircuit();
    
    doStuff();
}
```

Multiple circuits are provided, and all are configurable:

```csharp
Get["/"] = _ => 
{
    // Return 204-NoContent for 30 seconds in the event of a SQL Exception
    this.CanShortCircuit(new NoContentOnErrorCircuit()
        .ForException<SqlException>()
        .WithOpenTimeInSeconds(30));
    
    doStuff();
}
```

Stack up multiple circuits to handle different eventualities

```csharp
Get["/"] = _ => 
{
    this.CanShortCircuit(new NoContentOnErrorCircuit()
        .ForException<SqlException>()
        .WithOpenTimeInSeconds(30));
    
    this.CanShortCircuit(new LastGoodResponseOnErrorCircuit()
        .ForException<FileNotFoundException>()
        .WithOpenTimeInSeconds(5));
    
    doStuff();
}
```

Return the last good response if the endpoint recieves excess load:

```csharp
Get["/"] = _ => 
{
    // Lst good content if the route gets 100 requests within 5 seconds
    this.CanShortCircuit(new LastGoodResponseUnderLoad()
        .WithRequestSampleTimeInSeconds(5)
        .WithRequestThreshold(100));
    
    doStuff();
}
```
