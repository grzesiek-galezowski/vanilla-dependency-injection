using Lamar;
using Lamar.IoC;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Lamar;

public static class _2_LambdaRegistrationWithInterfaceArg
{
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
}