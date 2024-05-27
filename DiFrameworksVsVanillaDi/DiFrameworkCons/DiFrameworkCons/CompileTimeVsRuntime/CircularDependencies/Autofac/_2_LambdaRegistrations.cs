using Autofac.Core;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Autofac;

public static class _2_LambdaRegistrations
{
  /// <summary>
  /// Autofac doesn't have issues with lambda registrations
  /// when detecting circular dependencies.
  /// However, the detection is only at resolution time.
  ///
  /// Any flavor of container validation needs to be custom-implemented
  /// by the user.
  /// </summary>
  [Test]
  public static void ShouldDetectErrorDuringResolution()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<One>();
    containerBuilder.Register(x => new Two(x.Resolve<Three>()));
    containerBuilder.RegisterType<Three>();
    using var container = containerBuilder.Build();
    //WHEN
    //THEN
    Invoking(() => { container.Resolve<One>(); })
      .Should().ThrowExactly<DependencyResolutionException>()
    .Which.ToString().Should().ContainAll(
    [
      "Autofac.Core.DependencyResolutionException: An exception was thrown while activating " +
      "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One -> λ:DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three",
      "Circular component dependency detected: " +
      "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One -> λ:DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One."
    ]);
  }
}