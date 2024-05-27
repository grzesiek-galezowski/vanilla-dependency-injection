using Autofac.Core;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Autofac;

public static class _1_TypeRegistrations
{
  /// <summary>
  /// If you have circular dependency between two classes,
  /// Autofac will only tell you during the runtime resolution.
  /// You will also need to rely on runtime information while debugging
  /// and resolving the issue.
  ///
  /// There are some cases when Autofac provides some means to hack around
  /// circular dependencies (see https://autofac.readthedocs.io/en/latest/advanced/circular-dependencies.html),
  /// But so far, it doesn't support circular dependency between two types
  /// going through constructor arguments.
  /// </summary>
  [Test]
  public static void ShouldDetectErrorDuringResolution()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<One>();
    containerBuilder.RegisterType<Two>();
    containerBuilder.RegisterType<Three>();
    using var container = containerBuilder.Build();
    //WHEN
    //THEN
    Invoking(() => { container.Resolve<One>(); })
      .Should().ThrowExactly<DependencyResolutionException>()
      .Which.ToString().Should().ContainAll(
      [
        "Autofac.Core.DependencyResolutionException: An exception was thrown while activating " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three",
        "Circular component dependency detected: " +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One."
      ]);
  }
}