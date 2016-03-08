namespace Nancy.JohnnyFive
{
    using System.Collections.Generic;
    using Bootstrapper;
    using Database;

    public class Registrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return new List<TypeRegistration>
                {
                    new TypeRegistration(typeof (IJohhnyFiveDatabase), typeof (JohhnyFiveDatabase), Lifetime.Singleton)
                };
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }
    }
}
