using Lamar;
using Container = Lamar.Container;
#if NCRUNCH
using Lamar.IoC;
#endif

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Lamar;

public class _3_LambdaRegistrations
{
  /// <summary>
  /// With lambda registration, cycles can get detected when full config assertion is done.
  /// Interesting thing about Lamar is that even though it has
  /// a verification method similar to SimpleInjector, it does not
  /// detect circular dependencies gracefully but rather falls into stack overflow.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithLamarLambdaRegistration()
  {
    using var container = new Container(builder =>
    {
      builder
        .AddTransient<One>()
        .AddTransient(c => new Two(c.GetRequiredService<Three>()))
        .AddTransient<Three>();
    });

    //Configuration-only check does not unveil the dependency
    container.AssertConfigurationIsValid(AssertMode.ConfigOnly);

    //More extensive check fails with stack overflow
#if NCRUNCH
    Invoking(() =>
      {
        container.AssertConfigurationIsValid();
      }).Should().ThrowExactly<ContainerValidationException>()
      .Which.ToString().Should().Contain(
        "NCrunch has detected a stack overflow");
#endif
  }
}