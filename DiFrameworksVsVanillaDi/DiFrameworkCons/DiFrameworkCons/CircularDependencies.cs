using Autofac.Core;
using SimpleInjector;

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
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDi()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder
      .AddTransient<One>()
      .AddTransient<Two>()
      .AddTransient<Three>();
    var dependencyResolutionException = Assert.Throws<AggregateException>(() =>
    {
      using var container = containerBuilder.BuildServiceProvider(
        new ServiceProviderOptions
        {
          ValidateOnBuild = true,
          ValidateScopes = true,
        });
    });
    //WHEN
    //THEN

    StringAssert.Contains(
      "A circular dependency was detected for the service of type 'DiFrameworkCons.CircularDependencies+One'.\r\n" +
      "DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three -> DiFrameworkCons.CircularDependencies+One",
      dependencyResolutionException!.ToString());
  }

  /// <summary>
  /// With lambda registration, cycles get detected later
  /// </summary>
  [Test]
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDiLambdaRegistration()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();

    containerBuilder
      .AddTransient<One>()
      .AddTransient(c => new Two(c.GetRequiredService<Three>()))
      .AddTransient<Three>();

    using var container = containerBuilder.BuildServiceProvider(
      new ServiceProviderOptions
      {
        ValidateOnBuild = true,
        ValidateScopes = true,
      });
    //WHEN
    //THEN
    //uncomment to hang this test: var one = container.GetRequiredService<One>();
  }

  /// <summary>
  /// Simple injector has a Verify method that among others, tries to resolve
  /// all rules, and can thus detect misconfigurations.
  ///
  /// If this method is not used, then circular dependencies cause exceptions
  /// at dependency resolution time (though unlikely someone would not use Verify).
  /// </summary>
  [Test]
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithSimpleInjector()
  {
    //GIVEN
    var container = new SimpleInjector.Container();
    container.Register<One>(Lifestyle.Transient);
    container.Register(() => new Two(container.GetInstance<Three>()));
    container.Register<Three>();

    Assert.Throws<InvalidOperationException>(() =>
    {
      container.Verify(VerificationOption.VerifyAndDiagnose);
    });

    //WHEN
    //THEN
    Assert.Catch<ActivationException>(() =>
    {
      var instance = container.GetInstance<One>();
    });
  }

  /// <summary>
  /// Vanilla Dependency Injection makes circular dependencies very hard to pull off.
  /// You have to really try and ignore modern C# diagnostics like nullable reference types.
  /// </summary>
  [Test]
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithVanillaDI()
  {
    // This will not compile
    // var one = new One(new Two(new Three(one)));

    // This potentially could happen but very unlikely when using nullable reference types as errors + var
    One one = null!;
    var two = new Two(new Three(one));
    one = new One(two);
    Assert.IsNull(one.Two.Three.One);
  }

  public record One(Two Two);
  public record Two(Three Three);
  public record Three(One One);
}