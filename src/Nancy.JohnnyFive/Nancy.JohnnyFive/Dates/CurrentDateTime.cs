namespace Nancy.JohnnyFive.Dates
{
    using System;

    public class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now
        {
            get { return DateTime.UtcNow; }
        }
    }
}