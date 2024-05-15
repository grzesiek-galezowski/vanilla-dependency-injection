using Scrutor;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces;

public static class DependencyAsMultipleInterfaces_MsDi
{
  /// <summary>
  /// In MsDi, it's worse than Vanilla DI or even Autofac as we need to explicitly register each interface using lambdas,
  /// which are not subject to container validation. Hence, this approach is slightly more
  /// error-prone than Vanilla DI.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesUsingMsDi()
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

  /// <summary>
  /// This is a test adapted from
  /// https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
  /// only to show that in MsDi, registering the same implementation twice
  /// as singleton, each time as a different interface is not equal to
  /// resolving the same instance from each registration
  /// </summary>
  [Test]
  public static void WhenRegisteredAsSeparateSingleton_InstancesAreNotTheSame()
  {
    //GIVEN
    var builder = new ServiceCollection();

    builder.AddSingleton<IReadCache, Cache>();
    builder.AddSingleton<IWriteCache, Cache>();
    builder.AddSingleton<UserOfReaderAndWriter>();

    //WHEN
    using var container = builder.BuildServiceProvider();
    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().NotBeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().NotBe(cacheUser.ReadCache.Number);
  }
}