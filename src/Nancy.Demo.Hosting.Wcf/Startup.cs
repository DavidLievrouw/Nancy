using DavidLievrouw.InvoiceGen;
using DavidLievrouw.InvoiceGen.Composition;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Nancy;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace DavidLievrouw.InvoiceGen {
  public class Startup {
    public void Configuration(IAppBuilder app) {
      var container = CompositionRoot.Compose();

      app
        .UseNancy(
          options => {
            options.Bootstrapper = new Bootstrapper(container);
            options.PerformPassThrough = nancyContext => nancyContext.Response != null && nancyContext.Response.StatusCode == HttpStatusCode.NotFound;
          })
        .UseStageMarker(PipelineStage.PostAcquireState);
    }
  }
}