namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement
{
    using System;
    using Nancy.Session.InProcSessionsManagement;
    using Xunit;

    public class SessionIdFactoryFixture
    {
        private readonly SessionIdFactory sessionIdFactory;

        public SessionIdFactoryFixture()
        {
            this.sessionIdFactory = new SessionIdFactory();
            ;
        }

        public class CreateNew : SessionIdFactoryFixture
        {
            [Fact]
            public void Creates_non_empty_session_id()
            {
                var actual = this.sessionIdFactory.CreateNew();
                Assert.NotEqual(Guid.Empty, actual);
            }

            [Fact]
            public void Creates_new_id_every_time()
            {
                var newId1 = this.sessionIdFactory.CreateNew();
                var newId2 = this.sessionIdFactory.CreateNew();

                Assert.NotEqual(Guid.Empty, newId1);
                Assert.NotEqual(Guid.Empty, newId2);
                Assert.NotEqual(newId1, newId2);
            }
        }

        public class CreateFrom : SessionIdFactoryFixture
        {
            public void Given_null_session_id_string_then_returns_null()
            {
                var actual = this.sessionIdFactory.CreateFrom(null);
                Assert.False(actual.HasValue);
            }

            public void Given_empty_session_id_string_then_returns_null()
            {
                var emptySessionIdString = string.Empty;
                var actual = this.sessionIdFactory.CreateFrom(emptySessionIdString);
                Assert.False(actual.HasValue);
            }

            public void Given_invalid_session_id_string_then_returns_null()
            {
                const string invalidSessionIdString = "[ThisIsNotAGuid]";
                var actual = this.sessionIdFactory.CreateFrom(invalidSessionIdString);
                Assert.False(actual.HasValue);
            }

            public void Given_valid_session_id_string_then_returns_session_id()
            {
                var expectedSessionId = Guid.NewGuid();
                var sessionIdString = expectedSessionId.ToString();

                var actual = this.sessionIdFactory.CreateFrom(sessionIdString);

                Assert.True(actual.HasValue);
                Assert.Equal(expectedSessionId, actual.Value);
            }
        }
    }
}