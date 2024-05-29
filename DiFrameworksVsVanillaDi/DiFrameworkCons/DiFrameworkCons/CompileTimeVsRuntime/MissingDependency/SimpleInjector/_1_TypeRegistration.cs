using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.SimpleInjector;

public static class _1_TypeRegistration
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
}