using System.Linq;

namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Session;
    using Xunit;

    public class InProcSessionsFixture
    {
        [Fact]
        public void Should_add_pre_and_post_hooks_when_enabled()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);

            InProcSessions.Enable(hooks);

            beforePipeline.PipelineDelegates.Count().ShouldEqual(1);
            afterPipeline.PipelineItems.Count().ShouldEqual(1);
        }

        [Fact]
        public void Given_null_config_then_throws()
        {
            var hooks = A.Fake<IPipelines>();
            Assert.Throws<ArgumentNullException>(() => hooks.Enable((InProcSessionsConfiguration)null));
        }

        [Fact]
        public void Given_invalid_config_then_throws()
        {
            var hooks = A.Fake<IPipelines>();
            var invalidConfiguration = new InProcSessionsConfiguration
            {
                CryptographyConfiguration = null,
                SessionIdentificationMethodConfiguration = null,
                SessionTimeout = TimeSpan.FromSeconds(-5)
            };
            Assert.Throws<ArgumentException>(() => hooks.Enable(invalidConfiguration));
        }

        [Fact]
        public void Given_null_pipelines_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => InProcSessions.Enable(null));
        }
    }
}