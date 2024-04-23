using Lamar;
using Container = Lamar.Container;
#if NCRUNCH
using Lamar.IoC;
#endif

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
      .Which.ToString().Should().ContainAny([
        "Bi-directional dependencies detected to new One(Two)",
        "Bi-directional dependencies detected to new Two(Three)",
        "Bi-directional dependencies detected to new Three(One)"]
        );
  }

  /// <summary>
  /// With lambda registration, cycles get detected later
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithLamarLambdaRegistration()
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