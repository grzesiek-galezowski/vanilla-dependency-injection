using Lamar;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Lamar;

public class _2_CircularDependenciesBetweenNotRegisteredTypes
{
  /// <summary>
  /// However, when using auto resolution, container validation does not work
  /// and the dependency is only detected at resolution time.
  /// </summary>
  [Test]
  public static void ShouldThrowExceptionWhenResolvingCircularDependency()
  {
    using var container = new Container(_ => { });

    //configuration validation passes since there is no configuration
    container.AssertConfigurationIsValid();

    Invoking(() => { container.GetRequiredService<One>(); })
      .Should().ThrowExactly<InvalidOperationException>()
      .WithMessage(
        "Detected some kind of bi-directional dependency while trying " +
        "to discover and plan a missing service registration. " +
        "Examining types: " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One, " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two, " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three");
  }
}