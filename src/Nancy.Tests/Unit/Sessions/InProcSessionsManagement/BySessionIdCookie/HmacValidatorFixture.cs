namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class HmacValidatorFixture
    {
        private readonly CookieData cookieData;
        private readonly IHmacProvider fakeHmacProvider;
        private readonly byte[] hmac;
        private readonly HmacValidator hmacValidator;

        public HmacValidatorFixture()
        {
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.hmacValidator = new HmacValidator(this.fakeHmacProvider);
            this.hmac = new byte[] {1, 2, 3};
            this.cookieData = new CookieData
            {
                SessionId = "TheSessionId",
                Hmac = this.hmac
            };
        }

        [Fact]
        public void Given_null_hmac_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new HmacValidator(null));
        }

        [Fact]
        public void Given_null_cookie_data_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.hmacValidator.IsValidHmac(null));
        }

        [Fact]
        public void Given_cookie_data_without_hmac_then_returns_false()
        {
            this.cookieData.Hmac = null;
            var actual = this.hmacValidator.IsValidHmac(this.cookieData);
            Assert.False(actual);
        }

        [Fact]
        public void Given_hmac_is_invalid_then_returns_false()
        {
            A.CallTo(() => this.fakeHmacProvider.GenerateHmac(this.cookieData.SessionId))
                .Returns(new byte[] {7, 8, 9, 10});
            A.CallTo(() => this.fakeHmacProvider.HmacLength)
                .Returns(4);
            var actual = this.hmacValidator.IsValidHmac(this.cookieData);
            Assert.False(actual);
        }

        [Fact]
        public void Given_hmac_is_valid_then_returns_true()
        {
            A.CallTo(() => this.fakeHmacProvider.GenerateHmac(this.cookieData.SessionId))
                .Returns(this.hmac);
            A.CallTo(() => this.fakeHmacProvider.HmacLength)
                .Returns(this.hmac.Length);
            var actual = this.hmacValidator.IsValidHmac(this.cookieData);
            Assert.True(actual);
        }
    }
}