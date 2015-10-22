namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement
{
    using Nancy.Session.InProcSessionsManagement;
    using Xunit;

    public class BySessionIdCookieIdentificationMethodFixture
    {
        [Fact]
        public void On_creation_sets_default_cookie_name()
        {
            var actual = new BySessionIdCookieIdentificationMethod().CookieName;
            Assert.Equal(BySessionIdCookieIdentificationMethod.DefaultCookieName, actual);
        }
    }
}
