namespace Nancy.JohnnyFive.Startup
{
    using System.Collections.Generic;
    using Bootstrapper;
    using Store;

    public class Registrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations => new List<TypeRegistration>
        {
            new TypeRegistration(typeof (IStore), typeof (Store), Lifetime.Singleton)
        };

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }
    }
}
