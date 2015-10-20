namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using FakeItEasy;
    using Nancy.Session;
    using Xunit;

    public class SystemClockAmbientContextFixture : IDisposable
    {
        public void Dispose()
        {
            SystemClockAmbientContext.Current = null;
        }

        [Fact]
        public void Given_unitialized_ambient_context_Returns_new_real_clock()
        {
            var actual = SystemClockAmbientContext.Current;
            Assert.IsAssignableFrom<RealSystemClock>(actual);
        }

        [Fact]
        public void Given_fake_clock_Returns_that_fake_clock()
        {
            var fakeSystemClock = A.Fake<ISystemClock>();
            SystemClockAmbientContext.Current = fakeSystemClock;

            var actual = SystemClockAmbientContext.Current;

            Assert.Equal(fakeSystemClock, actual);
        }
    }
}