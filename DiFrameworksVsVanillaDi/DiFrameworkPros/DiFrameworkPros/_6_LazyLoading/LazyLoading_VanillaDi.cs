using System.Diagnostics;

namespace DiFrameworkPros._6_LazyLoading;

public class LazyLoading_VanillaDi
{
  /// <summary>
  /// With Vanilla DI, you have to be much more deliberate and explicit if
  /// you really want to have lazy creation (see Lazy usage in <see cref="VanillaCompositionRoot"/>).
  /// If you rely on laziness a lot, that can be a deal-breaker.
  ///
  /// I don't like the idea of "lazy by default". I consider it a value added
  /// that in case of Vanilla DI, the "singleton" dependencies are created up-front, because
  /// it gives me more confidence that when a service is started, the long-running objects
  /// are initialized correctly, without having to wait for a piece of code to "wake them up".
  /// As for the tests, I typically rely more on lower levels of testing pyramid
  /// and on test parallelization.
  /// </summary>
  [Test]
  public void ShouldLazyLoadUsingVanillaDi()
  {
    //GIVEN
    var compositionRoot = new VanillaCompositionRoot();

    //WHEN
    var stopWatch = new Stopwatch();
    var controllerINeed = compositionRoot.CreateControllerINeed();
    stopWatch.Stop();

    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means control never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }

  public class VanillaCompositionRoot
  {
    private readonly Lazy<IVerySlowDependency> _slowDependency;

    public VanillaCompositionRoot()
    {
      _slowDependency = new Lazy<IVerySlowDependency>(() => new VerySlowDependency());
    }

    public IControllerINeed CreateControllerINeed() => new ControllerINeed();
    public IControllerIDoNotNeed CreateControllerIDoNotNeed()
      => new ControllerIDoNotNeedButItNeedsSlowDependency(_slowDependency.Value);

  }
}
