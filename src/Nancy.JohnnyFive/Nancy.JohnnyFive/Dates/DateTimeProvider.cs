namespace Nancy.JohnnyFive.Dates
{
    using System;

    public static class DateTimeProvider
    {
        private static DateTime? _testMode;

        public static DateTime Now
        {
            get
            {
                if (_testMode.HasValue)
                    return _testMode.Value;

                return DateTime.Now;
            }
        }

        public static void SetTestDateTime(DateTime testTime)
        {
            _testMode = testTime;
        }

        public static void ExitTestMode()
        {
            _testMode = null;
        }
    }
}
