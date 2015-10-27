namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    internal interface IResponseManipulatorForSession
    {
        void ModifyResponseToRedirectToSessionUrl(NancyContext context,
            SessionIdentificationData sessionIdentificationData);
    }
}