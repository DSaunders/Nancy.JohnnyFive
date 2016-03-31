namespace Nancy.JohnnyFive.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Fakes;
    using JohnnyFive.Circuits;
    using JohnnyFive.Responders;
    using Models;
    using Should;
    using Xunit;

    public class NancyExtensionTests
    {
        private readonly FakeNancyModule _fakeModule;

        public NancyExtensionTests()
        {
            _fakeModule = new FakeNancyModule();
        }

        [Fact]
        public void Creates_List_Of_Circuits_On_First_Call()
        {
            // Given
            var circuit = new FakeCircuit();
            var responder = new FakeResponder();

            // When
            var config = _fakeModule.CanShortCircuit()
                .WithCircuit(circuit)
                .WithResponder(responder);

            // Then
            _fakeModule.Context.Items[Constants.ContextItemName].ShouldBeType<List<RouteConfig>>();
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName]).Count.ShouldEqual(1);

            var contextConfig = (RouteConfig) ((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0];
            contextConfig.ShouldEqual(config);
            contextConfig.Circuit.ShouldEqual(circuit);
            contextConfig.Responder.ShouldEqual(responder);
        }

        [Fact]
        public void Adds_To_List_Of_Circuits_On_Subsequent_Calls()
        {
            // When
            var config1 = _fakeModule.CanShortCircuit();
            var config2 = _fakeModule.CanShortCircuit();

            // Then
            _fakeModule.Context.Items[Constants.ContextItemName].ShouldBeType<List<RouteConfig>>();
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName]).Count.ShouldEqual(2);
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0].ShouldEqual(config1);
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName])[1].ShouldEqual(config2);
        }

        [Fact]
        public void Adds_Default_Circuit_If_None_Specified()
        {
            // When
            _fakeModule.CanShortCircuit();

            // Then
            var contextConfig = (RouteConfig)((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0];
            contextConfig.Circuit.ShouldBeType(typeof (OnErrorCircuit));
        }

        [Fact]
        public void Adds_Default_Responder_If_None_Specified()
        {
            // When
            _fakeModule.CanShortCircuit();

            // Then
            var contextConfig = (RouteConfig)((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0];
            contextConfig.Responder.ShouldBeType(typeof(LastGoodResponseResponder));
        }

        [Fact]
        public void Stores_Callback_Action_In_Config()
        {
            // Given
            var callback = new Action(() => { });

            // When
            _fakeModule.CanShortCircuit()
                .WithCallback(callback);

            // Then
            var contextConfig = (RouteConfig)((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0];
            contextConfig.OnShortCircuitCallback.ShouldEqual(callback);
        }
    }
}
