namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.BySessionIdCookie
{
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;
    using Xunit;

    public class CookieDataFixture
    {
        private readonly CookieData cookieData;
        private readonly string hmacString;

        public CookieDataFixture()
        {
            this.cookieData = new CookieData
            {
                SessionId = "TheSessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };
            this.hmacString = "01HMAC98";
        }

        [Fact]
        public void ToString_returns_expected_string_representation()
        {
            var actual = this.cookieData.ToString();
            var expected = string.Format("{0}{1}", this.hmacString, this.cookieData.SessionId);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_returns_value_without_hmac_if_there_is_no_hmac()
        {
            this.cookieData.Hmac = null;
            var actual = this.cookieData.ToString();
            Assert.Equal(this.cookieData.SessionId, actual);
        }
    }
}