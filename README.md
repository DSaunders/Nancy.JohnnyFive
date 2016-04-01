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

## Getting started


## Circuits and Responders

### Circuits

'Circuits' are what **Nancy.JohnnyFive** uses to decide whether to allow the route to be hit or not.

A circuit can be in one of the following states:

| State         |  Description                                                                                  |   
| :------------ |:----------------------------------------------------------------------------------------------|
| Normal        | The route action will be called as normal                                                     |
| ShortCircuit  | The route actions *will not be executed*, JohnnyFive will return something else instead       |

### Responders

Responders determine what **Nancy.JohnnyFive** should return if the route is in 'ShortCircuit' mode in place of executing your route action.

### Configuration




