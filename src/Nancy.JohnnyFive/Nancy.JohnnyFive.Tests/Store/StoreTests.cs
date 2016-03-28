namespace Nancy.JohnnyFive.Tests.Store
{
    using System.Collections.Generic;
    using JohnnyFive.Store;
    using Models;
    using Should;
    using Xunit;

    public class StoreTests
    {
        private readonly IStore _db;

        public StoreTests()
        {
            _db = new Store();
        }

        [Fact]
        public void Adds_Items_To_Database_If_Doesnt_Exist()
        {
            // Given
            var items = new List<RouteConfig> { new RouteConfig() };

            // When
            _db.AddIfNotExists("/customer", items);

            // Then
            _db.GetForRoute("/customer").ShouldEqual(items);
        }

        [Fact]
        public void Doesnt_Replace_Existing_Items()
        {
            // Given
            var items = new List<RouteConfig> { new RouteConfig() };
            var newItems = new List<RouteConfig> { new RouteConfig() };


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
