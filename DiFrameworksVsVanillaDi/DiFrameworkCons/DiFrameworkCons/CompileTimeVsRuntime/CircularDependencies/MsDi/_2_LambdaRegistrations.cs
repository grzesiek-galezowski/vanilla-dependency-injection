namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.MsDi;

public static class _2_LambdaRegistrations
{
  /// <summary>
  /// When at least one dependency is registered with lambda,
  /// we get a stack overflow (!!).
  ///
  /// I expect this to be fixed someday, but for now it serves as a good
  /// illustration that MsDi can get us into hard to diagnose issues.
  /// </summary>
  [Test]
  public static void ShouldDetectCircularDependencyButDoesNotAndLeadsToStackOverflow()
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

#if NCRUNCH
    Invoking(() =>
      {
        var one = container.GetRequiredService<One>();
      })
      .Should().Throw<Exception>()
      .WithMessage("*NCrunch has detected a stack overflow*");
#endif
  }
}