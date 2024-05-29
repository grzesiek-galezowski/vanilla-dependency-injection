using Lamar;
using Lamar.IoC;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Lamar;

public static class _3_LambdaRegistrationWithAutoResolution
{
  /// <summary>
  /// However, when the missing dependency is resolved inside the lambda
  /// using a concrete type, the automatic resolution of unregistered dependencies
  /// kicks in, and we get a transient instance injected, which might be exactly
  /// what we want or a deadly mistake.
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