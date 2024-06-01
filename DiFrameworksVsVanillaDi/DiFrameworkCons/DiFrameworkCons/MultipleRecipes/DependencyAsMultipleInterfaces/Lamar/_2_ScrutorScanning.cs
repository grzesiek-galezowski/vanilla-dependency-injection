using Lamar;
using Scrutor;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.Lamar;

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
    using var container = new Container(builder =>
    {
      builder.AddSingleton<UserOfReaderAndWriter>();
      builder.Scan(scan => scan
        .FromAssemblyOf<Cache>()
        .AddClasses(classes => classes.Where(c => c == typeof(Cache)))
        .UsingRegistrationStrategy(RegistrationStrategy.Throw)
        .AsSelfWithInterfaces()
        .WithSingletonLifetime());
    });

    container.AssertConfigurationIsValid();

    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}