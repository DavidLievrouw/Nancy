﻿namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class SessionIdentificationDataProviderFixture
    {
        private readonly IBySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod;
        private readonly string cookieName;
        private readonly string encryptedSessionIdString;

        private readonly SessionIdentificationData expectedResult;
        private readonly IHmacProvider hmacProvider;
        private readonly string hmacString;
        private readonly SessionIdentificationDataProvider sessionIdentificationDataProvider;
        private readonly Request validRequest;

        public SessionIdentificationDataProviderFixture()
        {
            this.cookieName = "TheCookieName";
            this.hmacProvider = A.Fake<IHmacProvider>();
            this.bySessionIdCookieIdentificationMethod = A.Fake<IBySessionIdCookieIdentificationMethod>();
            this.sessionIdentificationDataProvider = new SessionIdentificationDataProvider(this.hmacProvider,
                this.bySessionIdCookieIdentificationMethod);

            this.validRequest = new Request("GET", "http://www.google.be");
            this.hmacString = "01HMAC98";
            this.encryptedSessionIdString = "%2502SessionId";
            this.validRequest.Cookies.Add(this.cookieName, this.hmacString + this.encryptedSessionIdString);

            this.expectedResult = new SessionIdentificationData
            {
                SessionId = "%02SessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };

            A.CallTo(() => this.hmacProvider.HmacLength)
                .Returns(6);
            A.CallTo(() => this.bySessionIdCookieIdentificationMethod.CookieName)
                .Returns(this.cookieName);
        }

        [Fact]
        public void Given_null_hmac_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SessionIdentificationDataProvider(null, this.bySessionIdCookieIdentificationMethod));
        }

        [Fact]
        public void Given_null_identification_method_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new SessionIdentificationDataProvider(this.hmacProvider, null));
        }

        [Fact]
        public void Given_null_request_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => this.sessionIdentificationDataProvider.ProvideDataFromCookie(null));
        }

        [Fact]
        public void Given_request_without_cookie_then_returns_null()
        {
            this.validRequest.Cookies.Clear();
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromCookie(this.validRequest);
            Assert.Null(actual);
        }

        [Fact]
        public void Decodes_cookie_value()
        {
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromCookie(this.validRequest);
            Assert.Equal(this.expectedResult.SessionId, actual.SessionId);
        }

        [Fact]
        public void Given_cookie_data_is_completele_nonsense_then_returns_null()
        {
            this.SetCookieValue("BS");
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromCookie(this.validRequest);
            Assert.Null(actual);
        }

        [Fact]
        public void Given_cookie_hmac_is_invalid_base64_string_then_returns_input_without_hmac()
        {
            var cookieData = "A" + this.encryptedSessionIdString;
            var expectedResult = HttpUtility.UrlDecode(cookieData);
            this.SetCookieValue("A" + this.encryptedSessionIdString);

            var actual = this.sessionIdentificationDataProvider.ProvideDataFromCookie(this.validRequest);
            Assert.Equal(expectedResult, actual.SessionId);
            Assert.NotNull(actual.Hmac);
            Assert.Empty(actual.Hmac);
        }

        [Fact]
        public void Given_valid_cookie_then_returns_expected_result()
        {
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromCookie(this.validRequest);
            Assert.Equal(this.expectedResult.SessionId, actual.SessionId);
            Assert.True(HmacComparer.Compare(actual.Hmac, this.expectedResult.Hmac, this.hmacProvider.HmacLength));
        }

        private void SetCookieValue(string newValue)
        {
            this.validRequest.Cookies.Clear();
            this.validRequest.Cookies.Add(this.cookieName, newValue);
        }
    }
}