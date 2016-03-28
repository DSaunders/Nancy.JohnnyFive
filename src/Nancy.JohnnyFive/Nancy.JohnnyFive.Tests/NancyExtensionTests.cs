namespace Nancy.JohnnyFive.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using Fakes;
    using JohnnyFive.Circuits;
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
            _fakeModule.CanShortCircuit()
                ;

            // Then
            _fakeModule.Context.Items[Constants.ContextItemName].ShouldBeType<List<ICircuit>>();
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName]).Count.ShouldEqual(1);
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0].ShouldEqual(fakeCircuit);
        }

        [Fact]
        public void Adds_To_List_Of_Circuits_On_Subsequent_Calls()
        {
            // Given
            _fakeModule.CanShortCircuit(new FakeCircuit());
            var fakeCircuit = new FakeCircuit();

            // When
            _fakeModule.CanShortCircuit(fakeCircuit);

            // Then
            _fakeModule.Context.Items[Constants.ContextItemName].ShouldBeType<List<ICircuit>>();
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName]).Count.ShouldEqual(2);
            ((IList)_fakeModule.Context.Items[Constants.ContextItemName])[1].ShouldEqual(fakeCircuit);
        }

        [Fact(Skip = "Nothing to use as default yet")]
        public void Adds_Default_Circuit_If_None_Specified()
        {
            // When
            //_fakeModule.CanShortCircuit();

            //// Then
            //_fakeModule.Context.Items[Constants.ContextItemName].ShouldBeType<List<ICircuit>>();
            //((IList)_fakeModule.Context.Items[Constants.ContextItemName]).Count.ShouldEqual(1);
            //((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0].ShouldBeType<LastGoodResponseOnErrorCircuit>();
        }

        [Fact(Skip = "Nothing to use as default yet")]
        public void Adds_Default_Responder_If_None_Specified()
        {
            //// When
            //_fakeModule.CanShortCircuit();

            //// Then
            //_fakeModule.Context.Items[Constants.ContextItemName].ShouldBeType<List<ICircuit>>();
            //((IList)_fakeModule.Context.Items[Constants.ContextItemName]).Count.ShouldEqual(1);
            //((IList)_fakeModule.Context.Items[Constants.ContextItemName])[0].ShouldBeType<LastGoodResponseOnErrorCircuit>();
        }
    }
}
