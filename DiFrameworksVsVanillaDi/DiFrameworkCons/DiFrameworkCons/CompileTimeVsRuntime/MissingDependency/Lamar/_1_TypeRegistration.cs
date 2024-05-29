using Lamar;
using Lamar.IoC;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Lamar;

public static class _1_TypeRegistration
{
  /// <summary>
  /// Container validation catches missing dependencies during validation
  /// if their consumers are registered as types...
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsForConsumerRegisteredAsType()
  {
    using var container = new Container(builder => { builder.AddTransient<One>(); });

    Invoking(() => { container.AssertConfigurationIsValid(); }).Should().Throw<ContainerValidationException>()
      .Which.ToString().Should().Contain(
        "Lamar.IoC.ContainerValidationException: new One()\r\n" +
        "Cannot fill the dependencies of any of the public constructors\r\n" +
        "Available constructors:new One(ITwo Two)\r\n" +
        "* ITwo is not registered within this container and cannot be auto discovered by any missing family policy");
  }
}