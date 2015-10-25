namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.ByQueryStringParam;
    using Xunit;

    public class SessionIdentificationDataProviderFixture
    {
        private readonly IByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod;
        private readonly string encryptedSessionIdString;

        private readonly SessionIdentificationData expectedResult;
        private readonly IHmacProvider hmacProvider;
        private readonly string hmacString;
        private readonly string parameterName;
        private readonly SessionIdentificationDataProvider sessionIdentificationDataProvider;
        private readonly Request validRequest;

        public SessionIdentificationDataProviderFixture()
        {
            this.parameterName = "TheParamName";
            this.hmacProvider = A.Fake<IHmacProvider>();
            this.byQueryStringParamIdentificationMethod = A.Fake<IByQueryStringParamIdentificationMethod>();
            this.sessionIdentificationDataProvider = new SessionIdentificationDataProvider(this.hmacProvider,
                this.byQueryStringParamIdentificationMethod);

            this.hmacString = "01HMAC98";
            this.encryptedSessionIdString = "s%26%c2%a7%c2%a7ionId";
            this.validRequest = new Request("GET", string.Format("http://www.google.be?{0}={1}{2}",
                this.parameterName,
                this.hmacString,
                this.encryptedSessionIdString));

            this.expectedResult = new SessionIdentificationData
            {
                SessionId = "s&§§ionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };

            A.CallTo(() => this.hmacProvider.HmacLength)
                .Returns(6);
            A.CallTo(() => this.byQueryStringParamIdentificationMethod.ParameterName)
                .Returns(this.parameterName);
        }

        [Fact]
        public void Given_null_hmac_provider_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SessionIdentificationDataProvider(null, this.byQueryStringParamIdentificationMethod));
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
                () => this.sessionIdentificationDataProvider.ProvideDataFromQuery(null));
        }

        [Fact]
        public void Given_request_without_session_parameter_then_returns_null()
        {
            var requestWithoutSessionParameter = new Request("GET", "http://test/api");
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromQuery(requestWithoutSessionParameter);
            Assert.Null(actual);
        }

        [Fact]
        public void Decodes_parameter_value()
        {
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromQuery(this.validRequest);
            Assert.Equal(this.expectedResult.SessionId, actual.SessionId);
        }

        [Fact]
        public void Given_session_data_is_completele_nonsense_then_returns_null()
        {
            this.SetParameterValue("BS");
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromQuery(this.validRequest);
            Assert.Null(actual);
        }

        [Fact]
        public void Given_session_hmac_is_invalid_base64_string_then_returns_input_without_hmac()
        {
            var parameterData = "A" + this.encryptedSessionIdString;
            var expectedResult = HttpUtility.UrlDecode(parameterData);
            this.SetParameterValue("A" + this.encryptedSessionIdString);

            var actual = this.sessionIdentificationDataProvider.ProvideDataFromQuery(this.validRequest);
            Assert.Equal(expectedResult, actual.SessionId);
            Assert.NotNull(actual.Hmac);
            Assert.Empty(actual.Hmac);
        }

        [Fact]
        public void Given_valid_session_parameter_then_returns_expected_result()
        {
            var actual = this.sessionIdentificationDataProvider.ProvideDataFromQuery(this.validRequest);
            Assert.Equal(this.expectedResult.SessionId, actual.SessionId);
            Assert.True(HmacComparer.Compare(actual.Hmac, this.expectedResult.Hmac, this.hmacProvider.HmacLength));
        }

        private void SetParameterValue(string newValue)
        {
            if (newValue == null)
            {
                ((IDictionary<string, object>)this.validRequest.Query).Clear();
            }
            else
            {
                ((IDictionary<string, object>)this.validRequest.Query)[this.parameterName] = newValue;
            }
        }
    }
}