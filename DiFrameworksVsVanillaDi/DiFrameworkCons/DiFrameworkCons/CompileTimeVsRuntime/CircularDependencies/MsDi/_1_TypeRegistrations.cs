namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.MsDi;

public static class _1_TypeRegistrations
{
  /// <summary>
  /// MsDi also detects circular dependencies during runtime
  /// and throws an exception the path.
  /// </summary>
  [Test]
  public static void ShouldDetectCircularDependencyDuringContainerValidation()
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
        "A circular dependency was detected for the service of type 'DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One'.\r\n" +
        "DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Two -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Three -> DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.One");
  }
}