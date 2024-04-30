using Lamar;
using Container = Lamar.Container;
#if NCRUNCH
using Lamar.IoC;
#endif

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies;

public class CircularDependencies_Lamar
{
  /// <summary>
  /// Lamar also detects circular dependencies during runtime
  /// and throws an exception. When each type is
  /// registered, it can detect circular dependencies during
  /// container build.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithLamar()
  {
    Invoking(() =>
      {
        _ = new Container(builder =>
        {

          builder.AddTransient<One>()
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
  /// However, when using auto wiring, container validation does not work
  /// and the dependency is only detected at resolution time.
  /// </summary>
  [Test]
  public static void ShouldThrowExceptionWhenResolvingCircularDependency()
  {
    var container = new Container(_ =>
    {

    });

    //configuration validation passes since there is no configuration
    container.AssertConfigurationIsValid();

    Invoking(() =>
      {
        container.GetRequiredService<One>();
      })
      .Should().ThrowExactly<InvalidOperationException>()
      .WithMessage(
        "Detected some kind of bi-directional dependency while trying " +
        "to discover and plan a missing service registration. " +
        "Examining types: " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One, " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two, " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three");
  }

  /// <summary>
  /// With lambda registration, cycles get detected later.
  /// Interesting thing about Lamar is that even though it has
  /// a verification method similar to SimpleInjector, it does not
  /// detect circular dependencies but rather falls into stack overflow.
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