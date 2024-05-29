using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.SimpleInjector;

public static class _2_LambdaRegistration
{
  /// <summary>
  /// The same goes for lambda registrations because SimpleInjector's validation
  /// tries to resolve all dependencies, contrary to MsDi's.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredForLambdaRegistration()
  {
    using var container = new Container();
    container.Register(() => ActivatorUtilities.CreateInstance<One>(
      container, container.GetInstance<ITwo>()));

    Invoking(container.Verify).Should().ThrowExactly<InvalidOperationException>()
      .Which.ToString().Should().Contain(
        "The configuration is invalid. " +
        "Creating the instance for type One failed. " +
        "The registered delegate for type One threw an exception. " +
        "No registration for type ITwo could be found.");
  }
}