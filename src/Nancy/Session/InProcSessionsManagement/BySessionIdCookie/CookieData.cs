namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;

    internal class CookieData
    {
        public string SessionId { get; set; }
        public byte[] Hmac { get; set; }

        public override string ToString()
        {
            var base64hmac = string.Empty;
            if (this.Hmac != null) base64hmac = Convert.ToBase64String(this.Hmac);
            return string.Format("{0}{1}", base64hmac, this.SessionId);
        }
    }
}