# Nancy.JohnnyFive
Allows you to [short circuit](https://en.wikipedia.org/wiki/Short_Circuit_(1986_film)) Nancy routes in the event of an error or excess load.

```csharp

Get["/"] = _ => 
{
    // If an exception is thrown in this route, the route is 'short circuited' for
    //  a period of 10 seconds (returning 204-NoContent). After 10 seconds, the route
    //  can be hit again
    this.HasCircuit();
    doSomething();
}
```

Multiple circtuits are provided, and all are customisable:

```csharp
Get["/"] = _ => 
{
    // Return 'No Content' for 30 seconds in the event of a SQL Exception
    this.HasCircuit(new NoContentOnErrorCircuit()
        .ForException<KeyNotFoundException>()
        .WithOpenTimeInSeconds(30));
    
    doSomething();
}
```
