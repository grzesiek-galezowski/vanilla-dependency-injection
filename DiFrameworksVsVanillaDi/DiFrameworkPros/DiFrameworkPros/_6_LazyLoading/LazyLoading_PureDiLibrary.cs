using System.Diagnostics;
using Pure.DI;

namespace DiFrameworkPros._6_LazyLoading;

public class LazyLoading_PureDiLibrary
{
  [Test]
  public void ShouldLazyLoad()
  {
    //GIVEN
    var compositionRoot = new Composition18();

    //WHEN
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var controllerINeed = compositionRoot.ControllerINeed;
    stopWatch.Stop();

    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means control never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }
}

public partial class Composition18
{
  public void Setup()
  {
    DI.Setup(nameof(Composition18))
      .RootBind<IControllerINeed>("ControllerINeed")
      .As(Lifetime.Transient)
      .To<ControllerINeed>()
      .RootBind<IControllerIDoNotNeed>("ControllerIDoNotNeed")
      .As(Lifetime.Transient)
      .To<ControllerIDoNotNeedButItNeedsSlowDependency>()
      .Bind<IVerySlowDependency>()
      .As(Lifetime.Singleton)
      .To<VerySlowDependency>();
  }
}

