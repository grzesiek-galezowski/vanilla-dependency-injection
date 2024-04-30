namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency;

public static class MissingDependency_MsDi
{
  [Test]
  //see https://autofac.readthedocs.io/en/latest/faq/container-analysis.html
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithMsDiDuringContainerBuild()
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
        "'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Two' " +
        "while attempting to activate 'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.One'.)");
  }

  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithLambdaRegisteredMsDi()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder
      .AddTransient(c => new One(c.GetRequiredService<Two>()));
    using var container = containerBuilder.BuildServiceProvider(new ServiceProviderOptions
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });
    //WHEN
    //THEN
    Invoking(container.GetRequiredService<One>)
      .Should().Throw<InvalidOperationException>().Which.ToString().Should()
      .Contain("No service for type 'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Two' has been registered.");
  }
}