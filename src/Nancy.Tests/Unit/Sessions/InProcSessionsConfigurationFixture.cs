namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using Nancy.Session;
    using Xunit;

    public class InProcSessionsConfigurationFixture
    {
        private readonly InProcSessionsConfiguration config;

        public InProcSessionsConfigurationFixture()
        {
            this.config = new InProcSessionsConfiguration();
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set()
        {
            var result = new InProcSessionsConfiguration().IsValid;
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_with_null_session_identificationmethod()
        {
            this.config.SessionIdentificationMethod = null;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_empty_session_timeout()
        {
            this.config.SessionTimeout = TimeSpan.Zero;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_negative_session_timeout()
        {
            this.config.SessionTimeout = TimeSpan.FromSeconds(-1);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_valid_with_empty_cache_trim_interval()
        {
            this.config.CacheTrimInterval = TimeSpan.Zero;

            var result = this.config.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_with_negative_cache_trim_interval()
        {
            this.config.CacheTrimInterval = TimeSpan.FromSeconds(-1);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }
    }
}