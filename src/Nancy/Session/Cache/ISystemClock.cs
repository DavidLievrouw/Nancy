namespace Nancy.Session.Cache
{
    using System;

    /// <summary>
    /// Interface that represents the system clock.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        DateTime NowUtc { get; }
    }
}