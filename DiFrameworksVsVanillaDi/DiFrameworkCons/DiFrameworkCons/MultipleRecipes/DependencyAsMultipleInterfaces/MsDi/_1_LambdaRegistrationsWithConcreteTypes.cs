namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.MsDi;

public static class _1_LambdaRegistrationsWithConcreteTypes
{
  /// <summary>
  /// In MsDi, it's worse than Vanilla DI or even Autofac as we need to explicitly register each interface using lambdas,
  /// which are not subject to container validation. Hence, this approach is slightly more
  /// error-prone than Vanilla DI.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfaces()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<UserOfReaderAndWriter>();
    builder.AddSingleton<Cache>();
    builder.AddSingleton<IReadCache>(c => c.GetRequiredService<Cache>());
    builder.AddSingleton<IWriteCache>(c => c.GetRequiredService<Cache>());

    using var container = builder.BuildServiceProvider(new ServiceProviderOptions()
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });
    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}