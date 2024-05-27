using Lamar;
using Lamar.IoC;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Lamar;

public static class MissingDependency_Lamar
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

  /// <summary>
  /// The same goes for lambda registrations because Lamar's validation
  /// tries to resolve all dependencies, contrary to MsDi's.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredForLambdaRegistration()
  {
    using var container = new Container(builder =>
    {
      builder.AddTransient(x => new One(x.GetRequiredService<ITwo>()));
    });

    Invoking(() => { container.AssertConfigurationIsValid(); })
      .Should().ThrowExactly<ContainerValidationException>()
      .WithMessage("*Error in Lambda Factory of One\r\n" +
                   "Lamar.IoC.LamarMissingRegistrationException: No service registrations " +
                   "exist or can be derived for " +
                   "DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.ITwo*");
  }

  /// <summary>
  /// However, when the missing dependency is resolved inside the lambda
  /// using a concrete type, the automatic resolution of unregistered dependencies
  /// kicks in, and we get a transient instance injected, which might or might not be
  /// what we want.
  /// </summary>
  [Test]
  public static void ShouldShowNoFailureWhenMissingDependencyIsDiscoveredForLambdaRegistration()
  {
    using var container = new Container(builder =>
    {
      // note that this time, we resolve the concrete type.
      builder.AddTransient(x => new One(x.GetRequiredService<Two>()));
    });

    Invoking(() => container.AssertConfigurationIsValid())
      .Should().NotThrow();

    Invoking(() => container.GetRequiredService<One>())
      .Should().NotThrow();
  }

}