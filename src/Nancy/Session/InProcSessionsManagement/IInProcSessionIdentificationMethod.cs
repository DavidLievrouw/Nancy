namespace Nancy.Session.InProcSessionsManagement
{
    public interface IInProcSessionIdentificationMethod
    {
        ISession LoadSession(NancyContext context);
        void SaveSession(NancyContext context);
    }
}
