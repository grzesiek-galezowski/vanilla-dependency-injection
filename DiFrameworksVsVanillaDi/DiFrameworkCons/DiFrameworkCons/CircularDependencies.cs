using Autofac.Core;

namespace DiFrameworkCons;

public class CircularDependencies
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
  //9.3.3 Constructor/Constructor dependencies
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithAutofac()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<One>();
    containerBuilder.RegisterType<Two>();
    containerBuilder.RegisterType<Three>();
    using var container = containerBuilder.Build();
    //WHEN
    //THEN
    var dependencyResolutionException = Assert.Throws<DependencyResolutionException>(() =>
    {
      var one = container.Resolve<One>();
    });

    StringAssert.Contains(
      "Autofac.Core.DependencyResolutionException: An exception was thrown while activating DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three",
      dependencyResolutionException!.ToString());
    StringAssert.Contains(
      "Circular component dependency detected: DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three -> DiFrameworkCons.CircularDependencies+One.",
      dependencyResolutionException!.ToString());
  }

  /// <summary>
  /// MsDi also detects circular dependencies during runtime
  /// and throws an exception the path.
  /// </summary>
  [Test]
  //9.3.3 Constructor/Constructor dependencies
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDi()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder
      .AddTransient<One>()
      .AddTransient<Two>()
      .AddTransient<Three>();
    using var container = containerBuilder.BuildServiceProvider(true);
    //WHEN
    //THEN
    var dependencyResolutionException = Assert.Throws<InvalidOperationException>(() =>
    {
      var one = container.GetRequiredService<One>();
    });

    StringAssert.Contains(
      "A circular dependency was detected for the service of type 'DiFrameworkCons.CircularDependencies+One'.\r\n" +
      "DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three -> DiFrameworkCons.CircularDependencies+One",
      dependencyResolutionException!.ToString());
  }

  //bug add vanilla DI example

  public record One(Two Two);
  public record Two(Three Three);
  public record Three(One One);
}