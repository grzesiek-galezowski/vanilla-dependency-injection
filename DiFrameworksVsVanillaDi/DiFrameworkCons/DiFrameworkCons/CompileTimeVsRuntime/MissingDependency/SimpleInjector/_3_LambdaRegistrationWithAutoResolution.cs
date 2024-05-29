using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.SimpleInjector;

public static class _3_LambdaRegistrationWithAutoResolution
{
  /// <summary>
  /// And the same goes for lambda registrations with resolution of concrete types.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredForLambdaRegistrationWithConcreteTypeResolution()
  {
    using var container = new Container();
    container.Register(() => new One(container.GetInstance<Two>()));

    Invoking(container.Verify).Should().ThrowExactly<InvalidOperationException>()
      .Which.ToString().Should().Contain(
        "The configuration is invalid. " +
        "Creating the instance for type One failed. " +
        "The registered delegate for type One threw an exception. " +
        "No registration for type Two could be found.");
  }

}