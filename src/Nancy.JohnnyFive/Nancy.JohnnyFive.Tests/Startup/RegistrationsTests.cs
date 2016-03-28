namespace Nancy.JohnnyFive.Tests.Startup
{
    using System.Linq;
    using Bootstrapper;
    using JohnnyFive.Startup;
    using JohnnyFive.Store;
    using Should;
    using Xunit;

    public class RegistrationsTests
    {
        [Fact]
        public void Registers_Store()
        {
            new JohhnyFiveRegistrations()
                .TypeRegistrations
                .FirstOrDefault(r =>
                    r.RegistrationType == typeof (IStore) &&
                    r.ImplementationType == typeof (Store) &&
                    r.Lifetime == Lifetime.Singleton)
                .ShouldNotBeNull();
        }
    }
}
