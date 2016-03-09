namespace Nancy.JohnnyFive.Tests.Store
{
    using System.Collections.Generic;
    using Fakes;
    using JohnnyFive.Circuits;
    using JohnnyFive.Store;
    using Should;
    using Xunit;

    public class JohnnyFiveStoreTests
    {
        private readonly JohhnyFiveStore _db;

        public JohnnyFiveStoreTests()
        {
            _db = new JohhnyFiveStore();
        }

        [Fact]
        public void Adds_Items_To_Database_If_Doesnt_Exist()
        {
            // Given
            var items = new List<ICircuit> {new FakeCircuit()};

            // When
            _db.AddIfNotExists("/customer", items);

            // Then
            _db.GetForRoute("/customer").ShouldEqual(items);
        }

        [Fact]
        public void Doesnt_Replace_Existing_Items()
        {
            // Given
            var items = new List<ICircuit> { new FakeCircuit() };
            var newItems = new List<ICircuit> { new FakeCircuit() };


            // When
            _db.AddIfNotExists("/customer", items);
            _db.AddIfNotExists("/customer", newItems);

            // Then
            _db.GetForRoute("/customer").ShouldEqual(items);
        }

        [Fact]
        public void Returns_Empty_List_If_Item_Not_In_Db()
        {
            // When
            var result = _db.GetForRoute("/something");

            // Then
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Does_Not_Add_Null_Items()
        {
            // When
            _db.AddIfNotExists("/customer", null);

            // Then
            _db.GetForRoute("/customer").ShouldBeEmpty();
        }
    }
}
