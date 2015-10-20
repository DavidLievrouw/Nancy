namespace Nancy.Session.Cache
{
    /// <summary>
    /// Represents the ambient context of the global system clock.
    /// </summary>
    public class SystemClockAmbientContext
    {
        private static ISystemClock _currentSystemClock;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the current SystemClock, or returns a default one, if none has been created yet.
        /// </summary>
        public static ISystemClock Current
        {
            get
            {
                lock (_lock)
                {
                    return _currentSystemClock ?? (_currentSystemClock = new RealSystemClock());
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentSystemClock = value;
                }
            }
        }
    }
}