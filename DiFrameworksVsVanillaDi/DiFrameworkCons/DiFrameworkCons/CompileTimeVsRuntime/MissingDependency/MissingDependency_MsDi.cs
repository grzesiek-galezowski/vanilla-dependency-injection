namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency;

public static class MissingDependency_MsDi
{
  /// <summary>
  /// Container validation catches missing dependencies
  /// if their consumers are registered as types...
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredDuringContainerBuild()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder.AddTransient<One>();

    Invoking(() =>
      {
        using var container = containerBuilder.BuildServiceProvider(new ServiceProviderOptions
        {
          ValidateOnBuild = true,
          ValidateScopes = true
        });
      }).Should().Throw<AggregateException>()
      .Which.ToString().Should().Contain(
        "Some services are not able to be constructed " +
        "(Error while validating the service descriptor " +
        "'ServiceType: DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.One " +
        "Lifetime: Transient ImplementationType: DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.One': " +
        "Unable to resolve service for type " +
        "'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.ITwo' " +
        "while attempting to activate 'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.One'.)");
  }

  /// <summary>
  /// However, when registered as lambdas, the validation doesn't work
  /// and the error is detected at resolution time.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithLambdaRegisteredMsDi()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder
      .AddTransient(c => new One(c.GetRequiredService<ITwo>()));
    using var container = containerBuilder.BuildServiceProvider(new ServiceProviderOptions
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });
    //WHEN
    //THEN
    Invoking(container.GetRequiredService<One>)
      .Should().Throw<InvalidOperationException>().Which.ToString().Should()
      .Contain("No service for type 'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.ITwo' has been registered.");
  }
}