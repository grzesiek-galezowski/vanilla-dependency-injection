using Lamar;
using Lamar.IoC;
using Container = Lamar.Container;

namespace DiFrameworkCons.CircularDependencies;

public static class CircularDependencies_Lamar
{
  /// <summary>
  /// Lamar also detects circular dependencies during runtime
  /// and throws an exception the path.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithLamar()
  {
    Invoking(() =>
      {
        _ = new Container(builder =>
        {
          builder
            .AddTransient<One>()
            .AddTransient<Two>()
            .AddTransient<Three>();

        });
      }).Should().ThrowExactly<InvalidOperationException>()
      .Which.ToString().Should().Contain(
        "Bi-directional dependencies detected to new One(Two)");
  }

  /// <summary>
  /// With lambda registration, cycles get detected later
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDiLambdaRegistration()
  {
    var container = new Container(builder =>
    {
      builder
        .AddTransient<One>()
        .AddTransient(c => new Two(c.GetRequiredService<Three>()))
        .AddTransient<Three>();

    });

    //Configuration check does not unveil the dependency
    container.AssertConfigurationIsValid(AssertMode.ConfigOnly);

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