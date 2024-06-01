using Scrutor;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.MsDi;

public static class _2_ScrutorScanning
{
  /// <summary>
  /// Alternatively, convention-based API can be used to register
  /// just a single type.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesWithScrutor()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<UserOfReaderAndWriter>();
    builder.Scan(scan => scan
      .FromAssemblyOf<Cache>()
      .AddClasses(classes => classes.Where(c => c == typeof(Cache)))
      .UsingRegistrationStrategy(RegistrationStrategy.Throw)
      .AsSelfWithInterfaces()
      .WithSingletonLifetime());

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