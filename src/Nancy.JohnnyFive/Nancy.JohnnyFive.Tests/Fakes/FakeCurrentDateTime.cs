namespace Nancy.JohnnyFive.Tests.Fakes
{
    using System;
    using Dates;

    public class FakeCurrentDateTime : ICurrentDateTime
    {
        public DateTime FakeDateTime { get; set; }

        public DateTime Now
        {
            get { return FakeDateTime; }
        }
    }
}
