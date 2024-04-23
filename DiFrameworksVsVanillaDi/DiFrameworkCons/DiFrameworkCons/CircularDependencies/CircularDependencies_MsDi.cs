namespace DiFrameworkCons.CircularDependencies;

public static class CircularDependencies_MsDi
{
  /// <summary>
  /// MsDi also detects circular dependencies during runtime
  /// and throws an exception the path.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDi()
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
        "A circular dependency was detected for the service of type 'DiFrameworkCons.CircularDependencies.One'.\r\n" +
        "DiFrameworkCons.CircularDependencies.One -> DiFrameworkCons.CircularDependencies.Two -> DiFrameworkCons.CircularDependencies.Three -> DiFrameworkCons.CircularDependencies.One");
  }

  /// <summary>
  /// With lambda registration, cycles get detected later
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDiLambdaRegistration()
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
}