namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors;

public static class MultipleConstructors_MsDi
{
  /// <summary>
  /// In MsDi, we can, to a degree, influence which constructor
  /// is picked, by using the ActivatorUtilities.
  ///
  /// This, however, forces us into lambda registrations.
  /// </summary>
  [Test]
  public static void ShouldResolveUsingFirstConstructorFromAutofac()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<Constructor1Argument>();
    builder.AddSingleton<Constructor2Argument>();
    builder.AddSingleton<ObjectWithTwoConstructors>(
      x => ActivatorUtilities.CreateInstance<ObjectWithTwoConstructors>(
        x, x.GetRequiredService<Constructor1Argument>()));

    using var container = builder.BuildServiceProvider(new ServiceProviderOptions()
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });

    //WHEN
    var resolvedInstance = container.GetRequiredService<ObjectWithTwoConstructors>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }
}