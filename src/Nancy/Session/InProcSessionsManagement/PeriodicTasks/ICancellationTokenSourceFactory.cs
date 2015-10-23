namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    using System.Threading;

    internal interface ICancellationTokenSourceFactory
    {
        CancellationTokenSource Create();
    }
}