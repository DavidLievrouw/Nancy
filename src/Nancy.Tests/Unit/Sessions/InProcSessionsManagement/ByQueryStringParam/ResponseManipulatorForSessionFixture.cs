using System;

namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.ByQueryStringParam
{
    using System.Linq;
    using FakeItEasy;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.ByQueryStringParam;
    using Xunit;

    public class ResponseManipulatorForSessionFixture
    {
        private readonly IByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod;
        private readonly ResponseManipulatorForSession responseManipulatorForSession;
        private readonly string parameterName;
        private readonly NancyContext context;
        private readonly SessionIdentificationData sessionIdentificationData;

        public ResponseManipulatorForSessionFixture()
        {
            this.byQueryStringParamIdentificationMethod = A.Fake<IByQueryStringParamIdentificationMethod>();
            this.responseManipulatorForSession =
                new ResponseManipulatorForSession(this.byQueryStringParamIdentificationMethod);

            this.context = new NancyContext
            {
                Response = new Response(),
                Request = new Request("GET", "http://www.google.be")
            };
            this.sessionIdentificationData = new SessionIdentificationData
            {
                SessionId = "01SessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };
            this.parameterName = "SID";
            A.CallTo(() => this.byQueryStringParamIdentificationMethod.ParameterName)
                .Returns(this.parameterName);
        }

        [Fact]
        public void Given_null_identification_method_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ResponseManipulatorForSession(null));
        }

        [Fact]
        public void Given_null_context_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(null,
                        this.sessionIdentificationData));
        }

        [Fact]
        public void Given_null_session_identification_data_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(this.context,
                        null));
        }

        [Fact]
        public void Given_context_without_request_then_throws()
        {
            var contextWithoutRequest = new NancyContext
            {
                Response = new Response()
            };
            Assert.Throws<ArgumentException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(contextWithoutRequest,
                        this.sessionIdentificationData));
        }

        [Fact]
        public void Given_context_without_response_then_throws()
        {
            var contextWithoutResponse = new NancyContext
            {
                Request = new Request("GET", "http://www.google.be")
            };
            Assert.Throws<ArgumentException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(contextWithoutResponse,
                        this.sessionIdentificationData));
        }

        [Fact]
        public void Changes_http_status_code_of_reponse_to_302()
        {
            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(this.context,
                this.sessionIdentificationData);
            Assert.Equal(this.context.Response.StatusCode, HttpStatusCode.Found);
        }

        [Fact]
        public void Given_request_without_query_then_creates_query_for_location_header()
        {
            var expectedLocationHeaderValue = this.context.Request.Url +
                                              "?" + this.parameterName + 
                                              "=" + this.sessionIdentificationData;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(this.context,
                this.sessionIdentificationData);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.Equal(expectedLocationHeaderValue, locationHeader.Value, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Given_request_with_query_then_creates_query_for_location_header()
        {
            this.context.Request = new Request("GET", "http://www.google.be?value=test&process=3");
            var expectedLocationHeaderValue = this.context.Request.Url +
                                              "&" + this.parameterName +
                                              "=" + this.sessionIdentificationData;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(this.context,
                this.sessionIdentificationData);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.Equal(expectedLocationHeaderValue, locationHeader.Value, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Given_request_already_has_a_session_id_then_replaces_that_session_id()
        {
            this.context.Request = new Request("GET", "http://www.google.be:624?value=test&process=3&" + this.parameterName + "=ABC123&page=14");
            var expectedLocationHeaderValue = "http://www.google.be:624?value=test&process=3" +
                                              "&" + this.parameterName +
                                              "=" + this.sessionIdentificationData +
                                              "&page=14";

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(this.context,
                this.sessionIdentificationData);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.Equal(expectedLocationHeaderValue, locationHeader.Value, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Given_session_identifier_is_url_encoded()
        {
            this.sessionIdentificationData.SessionId = "/bOu§¨";
            const string encodedSessionId = "%2FbOu%C2%A7%C2%A8";
            var expectedParameterValue = "01HMAC98" + encodedSessionId;
            var expectedLocationHeaderValue = this.context.Request.Url +
                                              "?" + this.parameterName +
                                              "=" + expectedParameterValue;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionUrl(this.context,
                this.sessionIdentificationData);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.Equal(expectedLocationHeaderValue, locationHeader.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}