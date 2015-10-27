namespace Nancy.Session.InProcSessionsManagement
{
    internal interface IPeriodicCacheCleaner
    {
        void Start();
        void Stop();
    }
}