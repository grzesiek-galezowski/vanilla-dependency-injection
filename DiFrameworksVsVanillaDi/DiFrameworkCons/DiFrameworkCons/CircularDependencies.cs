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
    Invoking(() => { container.Resolve<One>(); })
      .Should().ThrowExactly<DependencyResolutionException>()
      .Which.ToString().Should().ContainAll(
      [
        "Autofac.Core.DependencyResolutionException: An exception was thrown while activating DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three",
        "Circular component dependency detected: DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three -> DiFrameworkCons.CircularDependencies+One."
      ]);
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

    //WHEN
    //THEN
    Invoking(() =>
      {
        using var container = containerBuilder.BuildServiceProvider(
          new ServiceProviderOptions
          {
            ValidateOnBuild = true,
            ValidateScopes = true,
          });
      }).Should().ThrowExactly<AggregateException>()
      .Which.ToString().Should().Contain(
        "A circular dependency was detected for the service of type 'DiFrameworkCons.CircularDependencies+One'.\r\n" +
        "DiFrameworkCons.CircularDependencies+One -> DiFrameworkCons.CircularDependencies+Two -> DiFrameworkCons.CircularDependencies+Three -> DiFrameworkCons.CircularDependencies+One");
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
    //TODO: uncomment to hang this test: var one = container.GetRequiredService<One>();
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

    Invoking(() => container.Verify(VerificationOption.VerifyAndDiagnose))
    .Should().Throw<InvalidOperationException>();

    //WHEN
    //THEN
    Invoking(() => container.GetInstance<One>()).Should().Throw<ActivationException>();
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
    one.Two.Three.One.Should().BeNull();
  }

  public record One(Two Two);
  public record Two(Three Three);
  public record Three(One One);
}