namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an object that can perform a periodic task.
    /// </summary>
    internal interface IPeriodicTask : IDisposable
    {
        /// <summary>
        /// Start the periodic task.
        /// </summary>
        /// <param name="initialDelay">The delay after which the first execution is performed.</param>
        /// <param name="interval">The interval between each execution.</param>
        /// <param name="cancellationToken">The token that can be used to cancel the periodic task.</param>
        /// <returns>The task that manages the periodic action.</returns>
        Task Start(TimeSpan initialDelay, TimeSpan interval, CancellationToken cancellationToken);
    }
}