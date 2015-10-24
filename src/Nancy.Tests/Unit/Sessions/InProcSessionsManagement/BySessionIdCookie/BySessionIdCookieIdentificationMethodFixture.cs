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

        public class Load : BySessionIdCookieIdentificationMethodFixture
        {
            private readonly NancyContext context;
            private readonly Guid newSessionId;

            public Load()
            {
                this.context = new NancyContext()
                {
                    Request = new Request("GET", "http://www.google.be")
                };

                this.newSessionId = Guid.NewGuid();
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .Returns(this.newSessionId);
            }

            [Fact]
            public void Given_null_context_then_throws()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(null));
            }

            [Fact]
            public void When_context_contains_no_session_cookie_data_then_returns_new_session_id()
            {
                A.CallTo(() => this.fakeCookieDataProvider.ProvideCookieData(this.context.Request))
                    .Returns(null);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_cookie_does_not_have_a_valid_hmac_then_returns_new_session_id()
            {
                var cookieData = new CookieData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = "1Hmac2"
                };

                A.CallTo(() => this.fakeCookieDataProvider.ProvideCookieData(this.context.Request))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(false);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_not_valid_then_returns_new_session_id()
            {
                var cookieData = new CookieData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = "1Hmac2"
                };

                A.CallTo(() => this.fakeCookieDataProvider.ProvideCookieData(this.context.Request))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(true);
                A.CallTo(() => this.fakeCookieDataDecrypter.DecryptCookieData(cookieData.SessionId))
                    .Returns(null);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(A<string>._))
                    .MustNotHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_not_a_valid_guid_then_returns_new_session_id()
            {
                const string invalidDecryptedSessionId = "This is not a valid guid!";
                var cookieData = new CookieData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = "1Hmac2"
                };

                A.CallTo(() => this.fakeCookieDataProvider.ProvideCookieData(this.context.Request))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(true);
                A.CallTo(() => this.fakeCookieDataDecrypter.DecryptCookieData(cookieData.SessionId))
                    .Returns(invalidDecryptedSessionId);
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(invalidDecryptedSessionId))
                    .Returns(null);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(this.newSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(invalidDecryptedSessionId))
                    .MustHaveHappened();
            }

            [Fact]
            public void When_decrypted_session_id_is_valid_then_returns_session_id_from_cookie()
            {
                var expectedSessionId = Guid.NewGuid();
                var decryptedSessionId = expectedSessionId.ToString();
                var cookieData = new CookieData
                {
                    SessionId = "ABCSomeEncryptedSessionIdXYZ",
                    Hmac = "1Hmac2"
                };

                A.CallTo(() => this.fakeCookieDataProvider.ProvideCookieData(this.context.Request))
                    .Returns(cookieData);
                A.CallTo(() => this.fakeHmacValidator.IsValidHmac(cookieData))
                    .Returns(true);
                A.CallTo(() => this.fakeCookieDataDecrypter.DecryptCookieData(cookieData.SessionId))
                    .Returns(decryptedSessionId);
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(decryptedSessionId))
                    .Returns(expectedSessionId);

                var actual = this.bySessionIdCookieIdentificationMethod.GetCurrentSessionId(this.context);

                Assert.Equal(expectedSessionId, actual);
                A.CallTo(() => this.fakeSessionIdFactory.CreateNew())
                    .MustNotHaveHappened();
                A.CallTo(() => this.fakeSessionIdFactory.CreateFrom(decryptedSessionId))
                    .MustHaveHappened();
            }
        }

        public class Save : BySessionIdCookieIdentificationMethodFixture
        {
            
        }
    }
}