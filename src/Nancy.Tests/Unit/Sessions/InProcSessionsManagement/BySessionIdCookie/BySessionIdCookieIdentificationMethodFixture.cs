namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using FakeItEasy;
    using Nancy.Session;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class BySessionIdCookieIdentificationMethodFixture
    {
        private readonly BySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod;
        private readonly ICookieDataDecrypter fakeCookieDataDecrypter;
        private readonly ICookieDataProvider fakeCookieDataProvider;
        private readonly IHmacValidator fakeHmacValidator;
        private readonly ISessionIdFactory fakeSessionIdFactory;
        private readonly InProcSessionsConfiguration validConfiguration;

        public BySessionIdCookieIdentificationMethodFixture()
        {
            this.validConfiguration = new InProcSessionsConfiguration();
            this.fakeCookieDataProvider = A.Fake<ICookieDataProvider>();
            this.fakeHmacValidator = A.Fake<IHmacValidator>();
            this.fakeCookieDataDecrypter = A.Fake<ICookieDataDecrypter>();
            this.fakeSessionIdFactory = A.Fake<ISessionIdFactory>();
            this.bySessionIdCookieIdentificationMethod = new BySessionIdCookieIdentificationMethod(
                this.validConfiguration,
                this.fakeCookieDataProvider,
                this.fakeHmacValidator,
                this.fakeCookieDataDecrypter,
                this.fakeSessionIdFactory);
        }

        [Fact]
        public void Given_null_configuration_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(null));
        }

        [Fact]
        public void Given_null_cookie_data_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.validConfiguration,
                null,
                this.fakeHmacValidator,
                this.fakeCookieDataDecrypter,
                this.fakeSessionIdFactory));
        }

        [Fact]
        public void Given_null_hmac_validator_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.validConfiguration,
                this.fakeCookieDataProvider,
                null,
                this.fakeCookieDataDecrypter,
                this.fakeSessionIdFactory));
        }

        [Fact]
        public void Given_null_cookie_data_decrypter_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.validConfiguration,
                this.fakeCookieDataProvider,
                this.fakeHmacValidator,
                null,
                this.fakeSessionIdFactory));
        }

        [Fact]
        public void Given_null_session_id_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BySessionIdCookieIdentificationMethod(
                this.validConfiguration,
                this.fakeCookieDataProvider,
                this.fakeHmacValidator,
                this.fakeCookieDataDecrypter,
                null));
        }

        [Fact]
        public void On_creation_sets_default_cookie_name()
        {
            var actual = this.bySessionIdCookieIdentificationMethod.CookieName;
            Assert.Equal(BySessionIdCookieIdentificationMethod.DefaultCookieName, actual);
        }
    }
}