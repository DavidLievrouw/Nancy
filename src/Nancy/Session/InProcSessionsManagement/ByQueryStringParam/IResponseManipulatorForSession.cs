namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    internal interface IResponseManipulatorForSession
    {
        void ModifyResponseToRedirectToSessionAwareUrl(NancyContext context,
            SessionIdentificationData sessionIdentificationData);
    }
}