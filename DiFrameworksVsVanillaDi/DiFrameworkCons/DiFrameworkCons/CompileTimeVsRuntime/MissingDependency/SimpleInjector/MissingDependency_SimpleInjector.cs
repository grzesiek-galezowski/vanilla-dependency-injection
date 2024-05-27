using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.SimpleInjector;

public static class MissingDependency_SimpleInjector
{
  /// <summary>
  /// Container validation catches missing dependencies during validation
  /// if their consumers are registered as types...
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsForConsumerRegisteredAsType()
  {
    using var container = new Container();
    container.Register<One>();

    Invoking(container.Verify).Should().ThrowExactly<InvalidOperationException>()
      .Which.ToString().Should().Contain(
        "Creating the instance for type One failed. " +
        "The constructor of type One contains the parameter with name " +
        "'Two' and type ITwo, but ITwo is not registered. " +
        "For ITwo to be resolved, it must be registered in the container.");
  }

  /// <summary>
  /// The same goes for lambda registrations because SimpleInjector's validation
  /// tries to resolve all dependencies, contrary to MsDi's.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredForLambdaRegistration()
  {
    using var container = new Container();
    container.Register(() => new One(container.GetInstance<ITwo>()));

    Invoking(container.Verify).Should().ThrowExactly<InvalidOperationException>()
      .Which.ToString().Should().Contain(
        "The configuration is invalid. " +
        "Creating the instance for type One failed. " +
        "The registered delegate for type One threw an exception. " +
        "No registration for type ITwo could be found.");
  }

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