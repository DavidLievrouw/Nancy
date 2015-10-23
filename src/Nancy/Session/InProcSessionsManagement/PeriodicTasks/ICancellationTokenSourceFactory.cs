namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    using System.Threading;

    public interface ICancellationTokenSourceFactory
    {
        CancellationTokenSource Create();
    }
}