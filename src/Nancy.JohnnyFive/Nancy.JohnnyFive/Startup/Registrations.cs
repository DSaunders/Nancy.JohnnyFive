namespace Nancy.JohnnyFive.Startup
{
    using System.Collections.Generic;
    using Bootstrapper;

    public class Registrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return new List<TypeRegistration>
                {
                    //new TypeRegistration(typeof (IJohhnyFiveStore), typeof (JohhnyFiveStore), Lifetime.Singleton)
                };
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }
    }
}
