using System.Diagnostics;
using SimpleInjector;

namespace DiFrameworkPros._6_LazyLoading;

public class LazyLoading_SimpleInjector //BUG: add other containers
{
  [Test]
  public void ShouldLazyLoad()
  {
    //GIVEN
    using var container = new Container();

    // By default, SimpleInjector runs verification on first
    // resolution. To skip auto resolution of dependencies
    // outside the resolved subgraph, we need to disable it.
    container.Options.EnableAutoVerification = false;

    container.RegisterSingleton<IVerySlowDependency, VerySlowDependency>();
    container.Register<IControllerINeed, ControllerINeed>();
    container.Register<IControllerIDoNotNeed, ControllerIDoNotNeedButItNeedsSlowDependency>();

    //WHEN
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var controllerINeed = container.GetInstance<IControllerINeed>();
    stopWatch.Stop();

    //THEN
    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means runtime execution never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }
}