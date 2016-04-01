#Nancy.JohnnyFive [![NuGet Version](https://img.shields.io/nuget/v/Nancy.JohnnyFive.svg?style=flat)](https://www.nuget.org/packages/Nancy.JohnnyFive/)
Adds fault tollerance to your Nancy routes.

```csharp
Get["/"] = _ =>
{
    // Watch this route for exceptions. If an exception is thrown, stop the route action being called and 
    // return the last successsful response for 10 seconds, before allowing the route to be hit again
    this.CanShortCircuit();
    
    return "Hello, World!";
};
```

**Nancy.JohnnyFive** allows you to short-circuit your Nancy routes by preventing the code in your route from being hit when certain conditions are met (and returning something else instead).

Some examples of things that **Nancy.JohnnyFive** can do are:
- If your route throws an exception, return the last successful response for a period of time, before allowing your code to be hit again
- If an excessive number of requests hit a given route, avoid executing the code in the route action entirely and return a specified status code instead. When traffic has died down, open the route again
- and more..
  
  
## Installation

Install via NuGet:

```
PM > Install-Package Nancy.JohnnyFive
```

## Circuits and Responders

JohhnyFive uses *Circuits* to determine whether a given route action should be hit or bypassed (short-circuited), and *responders* to decide what to return when in short-circuit mode. 

```csharp
this.CanShortCircuit()
    .WithCircuit(new UnderLoadCircuit())
    .WithResponder(new NoContentStatusCodeResponder());
```

## Circuits

'Circuits' are what **Nancy.JohnnyFive** uses to decide whether to allow the route to be hit or not.

A circuit can be in one of the following states:

| State         |  Description                                                                                  |   
| :------------ |:----------------------------------------------------------------------------------------------|
| Normal        | The route action will be called as normal                                                     |
| ShortCircuit  | The route action *will not be executed*, JohnnyFive will return something else instead       |

The following circuits are provided:

#### OnErrorCircuit (default)

When an exception is thrown in the route action, short-circuits for a period - before opening again and allowing the route action to be hit.

This is the default, so specifying no circuit will give you the an 'OnErrorCircuit' with the default configuration.

```csharp

// Short-circuits (and returns something else) for 10 seconds if
// any exception is thrown in this route action
this.CanShortCircuit();
```

OnErrorCircuit can also be configured as follows:

```csharp
// Short-circuits (and returns something else) for 20 seconds if
// any SqlException (or derived exception) is thrown in this route action
this.CanShortCircuit()
    .WithCircuit(new OnErrorCircuit()
        .ForExceptionType<SqlException>() 
        .ShortCircuitsForSeconds(20));
```

#### UnderLoadCircuit

If this route receives more 10 or more requests in a 1-second period, the route will 'short-circuit' and return something else.
When the number of requests falls back below this threshold, the route will be opened again

```csharp
// Short-circuits if 10 requests are received in a 1-second period
this.CanShortCircuit()
    .WithCircuit(new UnderLoadCircuit());
```

UnderLoadCircuit can be configured as follows:

```csharp
// Short-circuits  if 20 requests are received in a 5-second period
this.CanShortCircuit()
    .WithCircuit(new UnderLoadCircuit()
            .WithRequestLimit(20)
            .InSeconds(5));
```

## Responders

Responders determine what **Nancy.JohnnyFive** should return if the route is in 'ShortCircuit' mode in place of executing your route action.

The following responders are provided:

#### LastGoodResponseResponder (default)

This is the default, so specifying no responder will give you the a 'LastGoodResponseResponder'.

Returns the last successful response from the route action when a circuit is in 'short-circuit' mode.

```csharp
this.CanShortCircuit()
    .WithResponder(new LastGoodResponseResponder());
```

This returns the *entire* last good response (status code, body etc.), so should be indistinguishable from a 'real' response from the route action to anything that hits the route.

#### NoContentStatusCodeResponder

Returns a 'HTTP 204 - No Content' status code when the circuit is in 'short-circuit' mode.

```csharp
this.CanShortCircuit()
    .WithResponder(new NoContentStatusCodeResponder());
```

## Callbacks

**Nancy.JohnnyFive** allows you to specify a callback that occurs when a route is short-circuited. Any code can be executed in this action, but it is commonly used for logging that the short-circuit has occured:

```csharp
this.CanShortCircuit()
    .WithCallback(() => Debug.WriteLine("Route was short-circuited!"));
```


## Implementing your own Circuits and Responders

To implement your own Ciruit or Responder, simply implement ``ICircuit`` or ``IResponder``. You can then use your custom Circuit/Responder in the ``CanShortCircuit`` method.

```csharp
this.CanShortCircuit()
    .WithCircuit(new MyCustomCircuit())
    .WithResponder(new MyCustomResponder());
```

#### ICircuit

This is an example of a Circuit that alternates between 'Normal' and 'ShortCircuit' state for every other request.

```csharp
public class FlipFlopCircuit : ICircuit
{
    // This is the state of the Circuit. Either 'Normal' or 'ShortCircuit'.
    // Setting this will determine whether the circuit allows the route action to be hit, or 
    // short-circuits it entirely and returns the response from the Responder
    public CircuitState State { get; set; }
    
    // Called for every succesful response to a route action that uses 
    // this circuit
    public void AfterRequest(Response response)
    {
    }
    
    // Called before every request to a route action that uses 
    // this circuit
    public void BeforeRequest(Request request)
    {
        if (State == CircuitState.Normal)
            State = CircuitState.ShortCircuit;
        else
            State = CircuitState.Normal;
    }
    
    // Called when an exception is thrown in a route action that uses 
    // this circuit
    public void OnError(Exception ex)
    {
    }
}
```

### IResponder

This is an example of a Responder that returns the text 'Oops!' when the route is closed.

```csharp
public class TextResponder : IResponder
{
    // This is called after every request. It could be used, for example, to save details of 
    // successful requests for use later
    public void AfterRequest(Response response)
    {   
    }

    // When a Circuit is in the ShortCircuit state, the result of this method is returned
    // in place of the real route action
    public Response GetResponse()
    {
        return "Oops!";
    }
}
```
    
