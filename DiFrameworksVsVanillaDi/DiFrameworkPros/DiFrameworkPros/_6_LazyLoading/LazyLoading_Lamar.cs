using System.Diagnostics;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._6_LazyLoading;

public static class LazyLoading_Lamar //BUG: add other containers
{
  [Test]
  public static void ShouldLazyLoad()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddSingleton<IVerySlowDependency, VerySlowDependency>();
      builder.AddTransient<IControllerINeed, ControllerINeed>();
      builder.AddTransient<IControllerIDoNotNeed, ControllerIDoNotNeedButItNeedsSlowDependency>();
    });

    //WHEN
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var controllerINeed = container.GetRequiredService<IControllerINeed>();
    stopWatch.Stop();

    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means runtime execution never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }
}