namespace Nancy.Session.InProcSessionsManagement
{
    public interface IPeriodicCacheCleaner
    {
        void Start();
        void Stop();
    }
}
