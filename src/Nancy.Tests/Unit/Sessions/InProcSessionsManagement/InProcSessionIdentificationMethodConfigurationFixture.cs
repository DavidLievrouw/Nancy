using System;

namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement
{
    using Nancy.Session.InProcSessionsManagement;
    using Xunit;

    public class InProcSessionIdentificationMethodConfigurationFixture
    {
        [Fact]
        public void Given_null_identification_method_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSessionIdentificationMethodConfiguration(null));
        }

        [Fact]
        public void Default_configuration_uses_cookie_identification_method()
        {
            var actual = InProcSessionIdentificationMethodConfiguration.Default.SessionIdentificationMethod;
            Assert.IsAssignableFrom<BySessionIdCookieIdentificationMethod>(actual);
        }
    }
}
