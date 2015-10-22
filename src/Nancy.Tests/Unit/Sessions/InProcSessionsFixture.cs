using System.Linq;

namespace Nancy.Tests.Unit.Sessions
{
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
    }
}