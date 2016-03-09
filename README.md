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

Multiple circuits are provided, and all are customisable:

```csharp
Get["/"] = _ => 
{
    // Return 'No Content' for 30 seconds in the event of a SQL Exception
    this.CanShortCircuit(new NoContentOnErrorCircuit()
        .ForException<SqlException>()
        .WithOpenTimeInSeconds(30));
    
    doStuff();
}
```
