namespace Nancy.Session.InProcSessionsManagement.Cache
{
    using System;

    /// <summary>
    /// The default implementation of the SystemClock interface.
    /// </summary>
    public class RealSystemClock : ISystemClock
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        public DateTime NowUtc
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}