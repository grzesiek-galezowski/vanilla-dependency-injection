namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.MsDi;

public static class _2_LambdaRegistration
{
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
      .AddTransient(c => ActivatorUtilities.CreateInstance<One>(c, c.GetRequiredService<ITwo>()));
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