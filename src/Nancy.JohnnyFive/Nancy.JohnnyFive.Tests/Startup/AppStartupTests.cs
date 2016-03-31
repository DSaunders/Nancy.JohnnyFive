namespace Nancy.JohnnyFive.Tests.Startup
{
    using System;
    using System.IO;
    using System.Threading;
    using Bootstrapper;
    using Fakes;
    using JohnnyFive.Startup;
    using Models;
    using Should;
    using Xunit;

    public class AppStartupTests
    {
        private readonly IApplicationStartup _startup;
        private readonly FakeStore _store;
        private readonly FakePipelines _pipelines;

        public AppStartupTests()
        {
            _store = new FakeStore();
            _pipelines = new FakePipelines();
            _startup = new AppStartup(_store);
        }

        [Fact]
        public void AfterRequest_Registers_With_Store_If_Item_In_Context()
        {
            // Given
            var configs = new[] { new RouteConfig(), new RouteConfig(), };
            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http"),
            };
            context.Items.Add(Constants.ContextItemName, configs);

            // When
            _startup.Initialize(_pipelines);
            _pipelines.AfterRequest.Invoke(context, new CancellationToken());

            // Then
            _store.Db["/hello/world"].ShouldEqual(configs);
        }

        [Fact]
        public void AfterRequest_Does_Not_Register_With_Store_If_Item_Not_In_Context()
        {
            // Given
            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http"),
            };

            // When
            _startup.Initialize(_pipelines);
            _pipelines.AfterRequest.Invoke(context, new CancellationToken());

            // Then
            _store.Db.ContainsKey("/hello/world").ShouldBeFalse();
        }

        [Fact]
        public void OnError_Registers_With_Store_If_Item_In_Context()
        {
            // Given
            var configs = new[] { 
                new RouteConfig { Circuit = new FakeCircuit()}, 
                new RouteConfig { Circuit = new FakeCircuit()}
            };
            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http"),
            };
            context.Items.Add(Constants.ContextItemName, configs);

            // When
            _startup.Initialize(_pipelines);
            _pipelines.OnError.Invoke(context, new Exception());

            // Then
            _store.Db["/hello/world"].ShouldEqual(configs);
        }

        [Fact]
        public void OnError_Does_Not_Register_With_Store_If_Item_Not_In_Context()
        {
            // Given
            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http"),
            };

            // When
            _startup.Initialize(_pipelines);
            _pipelines.OnError.Invoke(context, new Exception());

            // Then
            _store.Db.ContainsKey("/hello/world").ShouldBeFalse();
        }

        [Fact]
        public void AfterRequest_Runs_AfterRequest_On_Circuits()
        {
            // Given
            var circuit1 = new FakeCircuit();
            var circuit2 = new FakeCircuit();

            var configs = new[] { 
                new RouteConfig { Circuit = circuit1, Responder = new FakeResponder()},
                new RouteConfig { Circuit = circuit2, Responder = new FakeResponder()}
            };

            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http"),
                Response = new Response()
            };
            context.Items.Add(Constants.ContextItemName, configs);

            // When
            _startup.Initialize(_pipelines);
            _pipelines.AfterRequest.Invoke(context, new CancellationToken());

            // Then
            circuit1.AfterRequestCall.ShouldEqual(context.Response);
            circuit2.AfterRequestCall.ShouldEqual(context.Response);
        }

        [Fact]
        public void AfterRequest_Runs_AfterRequest_On_Responders()
        {
            // Given
            var responder1 = new FakeResponder();
            var responder2 = new FakeResponder();

            var configs = new[] { 
                new RouteConfig { Circuit = new FakeCircuit(), Responder = responder1 },
                new RouteConfig { Circuit = new FakeCircuit(), Responder = responder2 }
            };

            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http"),
                Response = new Response()
            };
            context.Items.Add(Constants.ContextItemName, configs);

            // When
            _startup.Initialize(_pipelines);
            _pipelines.AfterRequest.Invoke(context, new CancellationToken());

            // Then
            responder1.AfterRequestCall.ShouldEqual(context.Response);
            responder2.AfterRequestCall.ShouldEqual(context.Response);
        }

        [Fact]
        public void BeforeRequest_Runs_BeforeRequest_On_Circuits()
        {
            // Given
            var circuit1 = new FakeCircuit();
            var circuit2 = new FakeCircuit();

            var configs = new[] {
                new RouteConfig { Circuit = circuit1, Responder = new FakeResponder()},
                new RouteConfig { Circuit = circuit2, Responder = new FakeResponder()}
            };

            var context = new NancyContext
            {
                Request = new Request("GET", "/hello/world", "http")
            };
            _store.AddIfNotExists("/hello/world", configs);

            // When
            _startup.Initialize(_pipelines);
            _pipelines.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            circuit1.BeforeRequestCall.ShouldEqual(context.Request);
            circuit2.BeforeRequestCall.ShouldEqual(context.Request);
        }
        
        [Fact]
        public void BeforeRequest_Returns_Responder_If_Circuit_In_ShortCircuit_State()
        {
            // Given
            var fakeCircuit = new FakeCircuit { State = CircuitState.ShortCircuit };
            var fakeResponder = new FakeResponder { FakeResponse = new Response() };

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig
                {
                    Circuit = fakeCircuit,
                    Responder = fakeResponder
                }
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            // When
            _startup.Initialize(_pipelines);
            var response = _pipelines.BeforeRequest.Invoke(context, new CancellationToken()).Result;

            // Then
            response.ShouldEqual(fakeResponder.FakeResponse);
        }

        [Fact]
        public void BeforeRequest_Calls_Callback_If_Circuit_In_Shortcircuit_State()
        {
            // Given
            var fakeCircuit = new FakeCircuit { State = CircuitState.ShortCircuit };
            var fakeResponder = new FakeResponder { FakeResponse = new Response() };
            var callbackCount = 0;
            
            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig
                {
                    Circuit = fakeCircuit,
                    Responder = fakeResponder,
                    OnShortCircuitCallback = () => callbackCount++
                }
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            // When
            _startup.Initialize(_pipelines);
            _pipelines.BeforeRequest.Invoke(context, new CancellationToken()).Wait();

            // Then
            callbackCount.ShouldEqual(1);
        }

        [Fact]
        public void BeforeRequest_Does_Not_Call_Callback_If_Circuit_Not_In_Shortcircuit_State()
        {
            // Given
            var fakeCircuit = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder = new FakeResponder { FakeResponse = new Response() };
            var callbackCount = 0;
            
            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig
                {
                    Circuit = fakeCircuit,
                    Responder = fakeResponder,
                    OnShortCircuitCallback = () => callbackCount++
                }
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            // When
            _startup.Initialize(_pipelines);
            _pipelines.BeforeRequest.Invoke(context, new CancellationToken()).Wait();

            // Then
            callbackCount.ShouldEqual(0);
        }
        
        [Fact]
        public void BeforeRequest_Returns_Null_If_Circuit_In_Normal_State()
        {
            // Given
            var fakeCircuit = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder = new FakeResponder { FakeResponse = new Response() };

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig
                {
                    Circuit = fakeCircuit,
                    Responder = fakeResponder
                }
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            // When
            _startup.Initialize(_pipelines);
            var response = _pipelines.BeforeRequest.Invoke(context, new CancellationToken()).Result;

            // Then
            response.ShouldEqual(null);
        }

        [Fact]
        public void BeforeRequest_Returns_Correct_Responder_If_One_Circuit_Of_Many_In_ShortCircuit_State()
        {
            // Given
            var fakeCircuit1 = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder1 = new FakeResponder { FakeResponse = new Response() };

            var fakeCircuit2 = new FakeCircuit { State = CircuitState.ShortCircuit };
            var fakeResponder2 = new FakeResponder { FakeResponse = new Response() };

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig { Circuit = fakeCircuit1, Responder = fakeResponder1 },
                new RouteConfig { Circuit = fakeCircuit2, Responder = fakeResponder2 },
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            // When
            _startup.Initialize(_pipelines);
            var response = _pipelines.BeforeRequest.Invoke(context, new CancellationToken()).Result;

            // Then
            response.ShouldEqual(fakeResponder2.FakeResponse);
        }

        [Fact]
        public void OnError_Calls_OnError_In_Circuits()
        {
            // Given
            var fakeCircuit1 = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder1 = new FakeResponder { FakeResponse = new Response() };

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig { Circuit = fakeCircuit1, Responder = fakeResponder1 },
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };
            
            var exception = new FileNotFoundException();

            // When
            _startup.Initialize(_pipelines);
            _pipelines.OnError.Invoke(context, exception);

            // Then
            fakeCircuit1.OnErrorCall.ShouldEqual(exception);
        }

        [Fact]
        public void OnError_Returns_Response_If_Any_Circuit_In_ShortCircuit_State()
        {
            // Given
            var fakeCircuit1 = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder1 = new FakeResponder { FakeResponse = new Response() };

            var fakeCircuit2 = new FakeCircuit { State = CircuitState.ShortCircuit };
            var fakeResponder2 = new FakeResponder { FakeResponse = new Response() };

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig { Circuit = fakeCircuit1, Responder = fakeResponder1 },
                new RouteConfig { Circuit = fakeCircuit2, Responder = fakeResponder2 },
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            var exception = new FileNotFoundException();

            // When
            _startup.Initialize(_pipelines);
            Response result = _pipelines.OnError.Invoke(context, exception);

            // Then
            result.ShouldEqual(fakeResponder2.FakeResponse);
        }

        [Fact]
        public void OnError_Calls_Callback_If_Circuit_In_ShortCircuit_State()
        {
            // Given
            var fakeCircuit = new FakeCircuit { State = CircuitState.ShortCircuit };
            var fakeResponder = new FakeResponder { FakeResponse = new Response() };
            var hitCount = 0;

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig { Circuit = fakeCircuit,
                    Responder = fakeResponder,
                    OnShortCircuitCallback = () => hitCount++
                }
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            var exception = new FileNotFoundException();

            // When
            _startup.Initialize(_pipelines);
            Response result = _pipelines.OnError.Invoke(context, exception);

            // Then
            hitCount.ShouldEqual(1);
        }

        [Fact]
        public void OnError_Does_Not_Call_Callback_If_Circuit_Not_In_ShortCircuit_State()
        {
            // Given
            var fakeCircuit = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder = new FakeResponder { FakeResponse = new Response() };
            var hitCount = 0;

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig { Circuit = fakeCircuit,
                    Responder = fakeResponder,
                    OnShortCircuitCallback = () => hitCount++
                }
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            var exception = new FileNotFoundException();

            // When
            _startup.Initialize(_pipelines);
            _pipelines.OnError.Invoke(context, exception);

            // Then
            hitCount.ShouldEqual(0);
        }

        [Fact]
        public void OnError_Returns_Null_If_No_Circuit_In_ShortCircuit_State()
        {
            // Given
            var fakeCircuit1 = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder1 = new FakeResponder { FakeResponse = new Response() };

            var fakeCircuit2 = new FakeCircuit { State = CircuitState.Normal };
            var fakeResponder2 = new FakeResponder { FakeResponse = new Response() };

            _store.Db.Add("/hello/world", new[]
            {
                new RouteConfig { Circuit = fakeCircuit1, Responder = fakeResponder1 },
                new RouteConfig { Circuit = fakeCircuit2, Responder = fakeResponder2 },
            });

            var context = new NancyContext { Request = new Request("GET", "/hello/world", "http") };

            var exception = new FileNotFoundException();

            // When
            _startup.Initialize(_pipelines);
            Response result = _pipelines.OnError.Invoke(context, exception);

            // Then
            result.ShouldBeNull();
        }
    }
}
