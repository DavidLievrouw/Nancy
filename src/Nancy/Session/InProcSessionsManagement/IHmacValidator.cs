namespace Nancy.Session.InProcSessionsManagement
{
    internal interface IHmacValidator
    {
        bool IsValidHmac(SessionIdentificationData sessionIdentificationData);
    }
}