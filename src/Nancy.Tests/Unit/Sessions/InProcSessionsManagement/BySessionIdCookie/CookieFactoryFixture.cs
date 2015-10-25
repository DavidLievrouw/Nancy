namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using FakeItEasy;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class CookieFactoryFixture
    {
        private readonly IBySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod;
        private readonly SessionIdentificationData sessionIdentificationData;
        private readonly string cookieDomain;
        private readonly CookieFactory cookieFactory;
        private readonly string cookieName;
        private readonly string cookiePath;
        private readonly string cookieValue;
        private readonly string cookieValueEncoded;

        public CookieFactoryFixture()
        {
            this.bySessionIdCookieIdentificationMethod = A.Fake<IBySessionIdCookieIdentificationMethod>();
            this.cookieFactory = new CookieFactory(this.bySessionIdCookieIdentificationMethod);

            this.cookieName = "TheCookieName";
            this.cookieValue = "01HMAC98%02SessionId";
            this.cookieValueEncoded = "01HMAC98%2502SessionId";
            this.sessionIdentificationData = new SessionIdentificationData
            {
                SessionId = "%02SessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };
            this.cookieDomain = ".nascar.com";
            this.cookiePath = "/schedule/";

            A.CallTo(() => this.bySessionIdCookieIdentificationMethod.CookieName)
                .Returns(this.cookieName);
            A.CallTo(() => this.bySessionIdCookieIdentificationMethod.Domain)
                .Returns(this.cookieDomain);
            A.CallTo(() => this.bySessionIdCookieIdentificationMethod.Path)
                .Returns(this.cookiePath);
        }

        [Fact]
        public void Given_null_identification_method_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CookieFactory(null));
        }

        [Fact]
        public void Given_null_cookie_data_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.cookieFactory.CreateCookie(null));
        }

        [Fact]
        public void Returns_http_only_cookie()
        {
            var actualCookie = this.cookieFactory.CreateCookie(this.sessionIdentificationData);
            Assert.True(actualCookie.HttpOnly);
        }

        [Fact]
        public void Returns_cookie_without_path_and_domain_if_none_is_set()
        {
            A.CallTo(() => this.bySessionIdCookieIdentificationMethod.Domain)
                .Returns(null);
            A.CallTo(() => this.bySessionIdCookieIdentificationMethod.Path)
                .Returns(null);

            var actualCookie = this.cookieFactory.CreateCookie(this.sessionIdentificationData);
            Assert.Null(actualCookie.Domain);
            Assert.Null(actualCookie.Path);
        }

        [Fact]
        public void Returns_cookie_with_path_and_domain_if_specified()
        {
            var actualCookie = this.cookieFactory.CreateCookie(this.sessionIdentificationData);
            Assert.Equal(this.cookieDomain, actualCookie.Domain);
            Assert.Equal(this.cookiePath, actualCookie.Path);
        }

        [Fact]
        public void Returns_cookie_with_valid_name()
        {
            var actualCookie = this.cookieFactory.CreateCookie(this.sessionIdentificationData);
            Assert.Equal(this.cookieName, actualCookie.Name);
        }

        [Fact]
        public void Returns_cookie_with_specified_data()
        {
            var actualCookie = this.cookieFactory.CreateCookie(this.sessionIdentificationData);
            Assert.Equal(this.cookieValue, actualCookie.Value);
        }

        [Fact]
        public void Returns_cookie_with_specified_encoded_data()
        {
            var actualCookie = this.cookieFactory.CreateCookie(this.sessionIdentificationData);
            Assert.Equal(this.cookieValueEncoded, actualCookie.EncodedValue);
        }
    }
}