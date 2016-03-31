namespace Nancy.JohnnyFive.Tests.Startup
{
    using System.Linq;
    using Bootstrapper;
    using JohnnyFive.Store;
    using Should;
    using Xunit;
    using Registrations = JohnnyFive.Startup.Registrations;

    public class RegistrationsTests
    {
        [Fact]
        public void Registers_Store()
        {
            new Registrations()
                .TypeRegistrations
                .FirstOrDefault(r =>
                    r.RegistrationType == typeof (IStore) &&
                    r.ImplementationType == typeof (Store) &&
                    r.Lifetime == Lifetime.Singleton)
                .ShouldNotBeNull();
        }
    }
}
